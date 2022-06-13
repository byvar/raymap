using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Ubisoft.CPA.U64;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using OpenSpace;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

				var lookup = loader.Fat.FixFix.Fat?.Value?.EntriesLookup;
				if (lookup?.ContainsKey(U64_StructType.BackgroundInfo) ?? false) {
					foreach (var bkgInfoId in lookup[U64_StructType.BackgroundInfo].Keys) {
						U64_Reference<GLI_BackgroundInfo> bkgInfo = 
							new U64_Reference<GLI_BackgroundInfo>(context, bkgInfoId)
							?.Resolve(s, isInFixFixFat: true);

						for (int i = 0; i < bkgInfo.Value.PalettesCount; i++) {
							var bkg = bkgInfo?.Value?.Background?.Value;
							var pal = bkgInfo?.Value?.Palettes?.Value?[i].Entry?.Value?.Palette;

							var tex = TextureHelpers.CreateTexture2D((int)bkg.ScreenWidth, (int)bkg.ScreenHeight);
							tex.FillRegion(bkg.Bitmap, 0, pal.Select(p => p.GetColor()).ToArray(),
								BinarySerializer.Unity.Util.TileEncoding.Linear_8bpp,
								0, 0, tex.width, tex.height, flipRegionY: true);
							tex.Apply();

							tex.Export(Path.Combine(outputDir, $"{bkgInfo?.Index:X4}_{i}"));
						}
					}
				}
			}

			UnityEngine.Debug.Log("Finished exporting backgrounds");
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

			// TODO: FON

			// DscMiscInfo
			loader.DscMiscInfo = new U64_Reference<GAM_DscMiscInfo>(context, 0);
			loader.DscMiscInfo.Resolve(s);
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context) {
			// Load fix
			await LoadFix(context);

			// Load level
			await LoadLevel(context, context.GetMapViewerSettings().Map);

			// Load animations
			//await LoadAnimations(context);

			throw new NotImplementedException();
		}
	}
}
