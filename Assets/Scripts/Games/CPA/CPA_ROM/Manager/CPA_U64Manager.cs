using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Ubisoft.CPA.U64;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OpenSpace;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Raymap {
	public class CPA_U64Manager : LegacyGameManager {
		public string FatFilePath => "fat.bin";
		public string DataFilePath => "data.bin";
		public string AnimsFilePath => "anims.bin";
		public string ShortAnimsFilePath => "shAnims.bin";
		public string AnimsCutTableFilePath => "cuttable.bin";

		#region Game actions
		public override GameAction[] GetGameActions(MapViewerSettings settings) => new GameAction[] {
			new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output)),
			new GameAction("Export Backgrounds", false, true, (input, output) => ExportBackgroundsAsync(settings, output)),
			new GameAction("Export Textures", false, true, (input, output) => ExportTexturesAsync(settings, output)),
			new GameAction("Export Localization", false, true, (input, output) => ExportLocalizationAsync(settings, output)),

		};
		public async UniTask ExportBlocksAsync(MapViewerSettings settings, string outputDir) {
			using (var context = new MapViewerContext(settings)) {
				// Get the deserializer
				var s = context.Deserializer;

				// Load the ROM
				await LoadFilesAsync(context);

				await LoadFix(context);

				var loader = s.GetLoader();
				foreach(var l in loader.Fat.Levels) l.SerializeFat(s);

				HashSet<Pointer> allStructPointers = new HashSet<Pointer>();
				Pointer[] orderedStructs;
				HashSet<Pointer> exportedStructs = new HashSet<Pointer>();
				void AddFat(LDR_Fat fat) {
					foreach (var e in fat.Fat.Value.Entries) {
						allStructPointers.Add(loader.GetStructPointer(e));
					}
				}
				AddFat(loader.Fat.FixFix);
				AddFat(loader.Fat.FixLevels);
				for (int i = 0; i < loader.Fat.Levels.Length; i++) {
					AddFat(loader.Fat.Levels[i]);
				}
				orderedStructs = allStructPointers.OrderBy(p => p.AbsoluteOffset).ToArray();

				async UniTask ExportFat(LDR_Fat fat, string dirName) {
					await TimeController.WaitIfNecessary();
					foreach (var e in fat.Fat.Value.Entries) {
						var ptr = loader.GetStructPointer(e);
						if(exportedStructs.Contains(ptr)) continue;
						exportedStructs.Add(ptr);
						int index = orderedStructs.FindItemIndex(p => p == ptr);
						long size = 0;
						if (index+1 < orderedStructs.Length) {
							size = orderedStructs[index+1].AbsoluteOffset - orderedStructs[index].AbsoluteOffset;
						} else {
							var f = orderedStructs[index].File;
							size = f.BaseAddress + f.Length - orderedStructs[index].AbsoluteOffset;
						}

						var type = U64_StructType_Defines.GetType(context, e.Type);
						var typeIndex = e.Type;
						var typeString = $"{typeIndex}";
						if(type.HasValue) typeString = $"{typeIndex} - {type.Value}";

						s.DoAt(ptr, () => {
							byte[] Bytes = null;
							Bytes = s.SerializeArray<byte>(Bytes, size, name: nameof(Bytes));
							BinarySerializer.Unity.Util.ByteArrayToFile(Path.Combine(outputDir, dirName, $"{typeString}", $"{e.Index:X4}.bin"), Bytes);
						});
					}
				}
				await ExportFat(loader.Fat.FixFix, "FixFix");
				await ExportFat(loader.Fat.FixLevels, "FixLevels");
				for (int i = 0; i < loader.Fat.Levels.Length; i++) {
					var dirname = $"{i}";
					var levelName = loader.Fix?.Value?.LevelsNameList?.Value?.FirstOrDefault(
						l => BitHelpers.ExtractBits(l.Level.Index,15, 0) == i)?.Name;
					if(levelName != null)
						dirname = $"{i} - {levelName}";

					await ExportFat(loader.Fat.Levels[i], dirname);
				}
			}

			UnityEngine.Debug.Log("Finished exporting structs");
		}

		public async UniTask ExportBackgroundsAsync(MapViewerSettings settings, string outputDir) {
			using (var context = new MapViewerContext(settings)) {
				// Get the deserializer
				var s = context.Deserializer;

				// Load the ROM
				await LoadFilesAsync(context);

				await LoadFix(context);

				// Load menu (level index 0)
				var loader = s.GetLoader();
				var levels = loader.Fix.Value.LevelsNameList.Value;
				GAM_LevelsNameList ChosenLevel = levels[0];
				await LoadLevel(context, ChosenLevel.Name);

				/*loader.DscMiscInfo = new U64_Reference<GAM_DscMiscInfo>(context, 0);
				loader.DscMiscInfo.Resolve(s);
				await LoadLevel(context, loader.DscMiscInfo.Value.FirstLevelName);*/

				ExportForLookup(loader.Fat.FixFix.Fat?.Value?.EntriesLookup);
				ExportForLookup(loader.Fat.Levels[loader.LevelIndex.Value]?.Fat?.Value?.EntriesLookup);
				void ExportForLookup(Dictionary<U64_StructType, Dictionary<ushort, LDR_EntryRef>> lookup) {
					if (lookup?.ContainsKey(U64_StructType.BackgroundInfo) ?? false) {
						foreach (var bkgInfoId in lookup[U64_StructType.BackgroundInfo].Keys) {
							try {
								U64_Reference<GLI_BackgroundInfo> bkgInfo =
									new U64_Reference<GLI_BackgroundInfo>(context, bkgInfoId)
									?.Resolve(s, isInFixFixFat: true);

								for (int i = 0; i < bkgInfo.Value.PalettesCount; i++) {
									var bkg = bkgInfo?.Value?.Background?.Value;
									var pal = bkgInfo?.Value?.Palettes?.Value?[i].Entry?.Value?.Palette;

									var tex = TextureHelpers.CreateTexture2D((int)bkg.ScreenWidth, (int)bkg.ScreenHeight);
									tex.FillRegion(bkg.Bitmap, 0, pal.GetColors().ToArray(),
										BinarySerializer.Unity.Util.TileEncoding.Linear_8bpp,
										0, 0, tex.width, tex.height, flipRegionY: true);
									tex.Apply();

									tex.Export(Path.Combine(outputDir, $"{bkgInfo?.Index:X4}_{i}"));
								}
							} catch (Exception ex) {
								s.LogWarning(ex.ToString());
							}
						}
					}
				}
			}

			UnityEngine.Debug.Log("Finished exporting backgrounds");
		}

		public virtual async UniTask ExportTexturesAsync(MapViewerSettings settings, string outputDir) {

			using (var context = new MapViewerContext(settings)) {
				// Get the deserializer
				var s = context.Deserializer;

				// Load the ROM
				await LoadFilesAsync(context);

				await LoadFix(context);

				var loader = s.GetLoader();
				var levels = loader.Fix.Value.LevelsNameList.Value;

				HashSet<Pointer> texturesExported = new HashSet<Pointer>();

				async UniTask ExportForFat(LDR_Fat fat, bool isFixFix = false) {
					var lookup = fat?.Fat?.Value?.EntriesLookup.TryGetItem(U64_StructType.BitmapInfo);
					if(lookup == null) return;

					foreach (var bitmapInfoRef in lookup) {
						var ptr = loader.GetStructPointer(bitmapInfoRef.Value);
						if(texturesExported.Contains(ptr)) continue;
						texturesExported.Add(ptr);
						U64_Reference<GLI_BitmapInfo> bitmapInfo = new U64_Reference<GLI_BitmapInfo>(context, bitmapInfoRef.Key);
						bitmapInfo?.Resolve(s, isInFixFixFat: isFixFix);
						Texture2D tex = bitmapInfo?.Value?.GetTexture(flip: context?.GetCPASettings().Platform != Platform.N64);
						tex.Export(Path.Combine(outputDir, $"{ptr.AbsoluteOffset:X8}"));
					}
					await TimeController.WaitIfNecessary();
				}

				// Export for fix fats
				// Set level index
				loader.LevelIndex = BitHelpers.ExtractBits(levels[0].Level.Index, 15, 0);
				// Load Level FAT
				loader.Fat.Levels[loader.LevelIndex.Value].SerializeFat(s);

				await ExportForFat(loader.Fat.FixFix, isFixFix: true);
				await ExportForFat(loader.Fat.FixLevels, isFixFix: false);

				// Export for level fats
				for (int i = 0; i < levels.Length; i++) {
					if (i > 0) {
						// Set level index
						loader.LevelIndex = BitHelpers.ExtractBits(levels[i].Level.Index, 15, 0);
						// Load Level FAT
						loader.Fat.Levels[loader.LevelIndex.Value].SerializeFat(s);
					}

					await ExportForFat(loader.Fat.Levels[loader.LevelIndex.Value]);
				}
				//
				//loader.Data.MainTablesDictionary[U64_StructType.BitmapCI4];
			}
			// Textures in data.bin
			/*texturesTableSeen = new bool[texturesTable.Length];
			for (int i = 0; i < fatTables.Length; i++) {
				for (int j = 0; j < fatTables[i].entries.Length; j++) {
					if (fatTables[i].entries[j].EntryType != FATEntry.Type.TextureInfo) continue;
					LegacyPointer ptr = new LegacyPointer(fatTables[i].entries[j].off_data, files_array[SMem.Data]);
					TextureInfo texInfo = new TextureInfo();
					texInfo.Init(ptr, fatTables[i].entries[j].index);
					texInfo.Read(reader);
				}
			}
			for (int i = 0; i < texturesTable.Length; i++) {
				if (!texturesTableSeen[i]) {
					print("Unused Texture: " + i + " - " + texturesTable[i] + ". Est. length: " + (texturesTable[i + 1].offset - texturesTable[i].offset));
					uint size = (texturesTable[i + 1].offset - texturesTable[i].offset);
					float logSize = Mathf.Log(size, 2);
					if (i < ind_textureTable_i8) {
						// I4
						for (int w = 3; w < 15; w++) {
							for (int h = 3; h < 15; h++) {
								if (w + h == (int)logSize + 1) {
									GF64 gf = new GF64(reader, texturesTable[i], 1 << w, 1 << h, GF64.Format.I4, null, 16);
									Util.ByteArrayToFile(gameDataBinFolder + "/textures/unused/" + GF64.Format.I4 + "_T" + (i - ind_textureTable_i4) + "_" + gf.texture.width + "x" + gf.texture.height + ".png", gf.texture.EncodeToPNG());
								}
							}
						}
					} else if (i < ind_textureTable_rgba) {
						// I8
						for (int w = 3; w < 15; w++) {
							for (int h = 3; h < 15; h++) {
								if (w + h == (int)logSize) {
									GF64 gf = new GF64(reader, texturesTable[i], 1 << w, 1 << h, GF64.Format.I8, null, 32);
									Util.ByteArrayToFile(gameDataBinFolder + "/textures/unused/" + GF64.Format.I8 + "_T" + (i - ind_textureTable_i8) + "_" + gf.texture.width + "x" + gf.texture.height + ".png", gf.texture.EncodeToPNG());
								}
							}
						}
					} else {
						// RGBA16
						for (int w = 3; w < 15; w++) {
							for (int h = 3; h < 15; h++) {
								if (w + h == (int)logSize - 1) {
									GF64 gf = new GF64(reader, texturesTable[i], 1 << w, 1 << h, GF64.Format.RGBA, null, 32);
									Util.ByteArrayToFile(gameDataBinFolder + "/textures/unused/" + GF64.Format.RGBA + "_T" + (i - ind_textureTable_rgba) + "_" + gf.texture.width + "x" + gf.texture.height + ".png", gf.texture.EncodeToPNG());
								}
							}
						}
					}
				}
			}*/
			await UniTask.CompletedTask;
		}

		public virtual async UniTask ExportLocalizationAsync(MapViewerSettings settings, string outputDir) {

			using (var context = new MapViewerContext(settings)) {
				// Get the deserializer
				var s = context.Deserializer;

				// Load the ROM
				await LoadFilesAsync(context);

				await LoadFix(context);

				// Load menu (level index 0)
				var loader = s.GetLoader();
				var levels = loader.Fix.Value.LevelsNameList.Value;
				GAM_LevelsNameList ChosenLevel = levels[0];
				await LoadLevel(context, ChosenLevel.Name);

				// Export loc
				string filePath = Path.Combine(outputDir, $"localization_{context.GetCPASettings().Mode}.json");
				if (loader?.Languages != null) {
					var output = Enumerable.Range(0, loader.Languages.Length).Select(ind => new {
						Language = loader.Languages?[ind]?.Value?.LanguageName ?? ("Language " + ind),
						Text = loader.Languages?[ind]?.Value?.StringList.List?.Value?.Select(str => str?.Entry?.Value?.String?.Value?.Value?.StringValue ?? ""),
						StringLength = loader.Languages?[ind]?.Value?.StringLengthList?.List?.Value?.Select(str => str?.Entry?.Value?.Length ?? 0),
					});
					string json = JsonConvert.SerializeObject(output, Formatting.Indented);
					BinarySerializer.Unity.Util.ByteArrayToFile(filePath, Encoding.UTF8.GetBytes(json));
				}
			}
		}
		#endregion

		protected override List<string> FindFiles(MapViewerSettings settings) {
			List<string> levelsList = new List<string>();
			using (var context = new MapViewerContext(settings, log: false)) {
				// Add files
				Endian endian = context.GetCPASettings().GetEndian;
				context.AddFile(new LinearFile(context, FatFilePath, endian));
				context.AddFile(new LinearFile(context, DataFilePath, endian));

				// Create loader
				LDR_Loader loader = new LDR_Loader(context);

				var s = context.Deserializer;

				// Load fat & fix
				loader.Data = FileFactory.Read<U64_DataFile>(context, DataFilePath);
				loader.Fat = FileFactory.Read<LDR_FatFile>(context, FatFilePath);
				loader.Fix = new U64_Reference<GAM_Fix>(context, 0).Resolve(s, isInFixFixFat: true);

				var levels = loader.Fix.Value.LevelsNameList.Value;
				foreach(var l in levels) levelsList.Add(l.Name);
			}
			return levelsList;
		}

		public override async UniTask LoadFilesAsync(Context context) {
			Endian endian = context.GetCPASettings().GetEndian;
			await context.AddLinearFileAsync(FatFilePath, endianness: endian);
			// TODO: change this to bigfile later?
			await context.AddLinearFileAsync(DataFilePath, endianness: endian);

			// Animations
			await context.AddLinearFileAsync(AnimsFilePath, endianness: endian);
			await context.AddLinearFileAsync(ShortAnimsFilePath, endianness: endian);
			await context.AddLinearFileAsync(AnimsCutTableFilePath, endianness: endian);
		}

		public async UniTask LoadAnimations(Context context) {
			var loader = context.GetLoader();

			var animsFile = FileFactory.Read<A3D_AnimationsFile>(context, AnimsFilePath);
			loader.AnimationsFile = animsFile;
			for (int i = 0; i < animsFile.AnimationsCount; i++) {
				if (i % 16 != 0) continue; // This is just a test
				if (i % 256 == 0) {
					GlobalLoadState.DetailedState = $"Loading animations: anims ({i + 1}/{animsFile.AnimationsCount})";
					await TimeController.WaitIfNecessary();
				}
				animsFile.LoadAnimation(context.Deserializer, i);
			}
			var shAnimsFile = FileFactory.Read<A3D_ShortAnimationsFile>(context, ShortAnimsFilePath);
			var cutTableFile = FileFactory.Read<A3D_AnimationCutTable>(context, AnimsCutTableFilePath);
		}

		public async UniTask LoadFix(Context context) {
			// Create loader
			var dataPointer = context.FilePointer(DataFilePath);
			LDR_Loader loader = new LDR_Loader(context);

			GlobalLoadState.DetailedState = "Loading fix";
			await TimeController.WaitIfNecessary();

			var s = context.Deserializer;

			// Load Data
			loader.Data = FileFactory.Read<U64_DataFile>(context, DataFilePath);

			// Load fat
			loader.Fat = FileFactory.Read<LDR_FatFile>(context, FatFilePath);

			// Load fix
			loader.Fix = new U64_Reference<GAM_Fix>(context, 0).Resolve(s, isInFixFixFat: true);

			await UniTask.CompletedTask;
		}

		public async UniTask LoadLevel(Context context, string levelName) {
			GlobalLoadState.DetailedState = "Loading level";
			await TimeController.WaitIfNecessary();

			var loader = context.GetLoader();
			var s = context.Deserializer;

			// Determine level index
			var levels = loader.Fix.Value.LevelsNameList.Value;
			GAM_LevelsNameList ChosenLevel = null;
			foreach (var level in levels) {
				if (level.Name.ToLower() == levelName.ToLower()) {
					ChosenLevel = level;
					break;
				}
			}
			if (ChosenLevel == null)
				throw new Exception($"Invalid map: {levelName}");

			// Set level index
			loader.LevelIndex = BitHelpers.ExtractBits(ChosenLevel.Level.Index, 15, 0);
			// Load Level FAT
			loader.Fat.Levels[loader.LevelIndex.Value].SerializeFat(s);

			// Serialize additional references in fix
			loader.Fix?.Value?.ResolveLevelReferences(s);

			loader.FixPreloadSection = new U64_Reference<GAM_FixPreloadSection>(context, 0);
			loader.FixPreloadSection.Resolve(s);

			loader.Level?.Resolve(s);

			// FON: Localization table
			loader.LanguagesCount = new U64_Reference<FON_LanguagesCount>(context, 0);
			loader.LanguagesCount.Resolve(s);
			loader.Languages = new U64_Reference<FON_LanguageString>[loader.LanguagesCount?.Value?.LanguagesCount ?? 0];
			for (ushort i = 0; i < loader.Languages.Length; i++) {
				GlobalLoadState.DetailedState = $"Loading language table {i+1}/{loader.Languages.Length}";
				await TimeController.WaitIfNecessary();
				loader.Languages[i] = new U64_Reference<FON_LanguageString>(context, i)?.Resolve(s);
				if (loader.Languages[i]?.Value != null) {
					context.Logger?.Log(loader.Languages[i]?.Value.LanguageName);
				}
			}

			// DscMiscInfo
			loader.DscMiscInfo = new U64_Reference<GAM_DscMiscInfo>(context, 0);
			loader.DscMiscInfo.Resolve(s);
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context) {
			// Load fix
			await LoadFix(context);

			// Load level
			await LoadLevel(context, context.GetMapViewerSettings().Map);

			// TODO: Load remaining unreferenced elements: objectTables, wayPoints, graphs, 

			// Load animations
			await LoadAnimations(context);

			throw new NotImplementedException();
		}
	}
}
