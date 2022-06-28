using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Raymap {
	// TODO: This will be the equivalent of the R2Loader
	public class CPA_SNAManager : LegacyGameManager {

		#region Game actions
		public override GameAction[] GetGameActions(MapViewerSettings settings) => new GameAction[] {
			new GameAction("Export Relocation Table BigFile", false, true, (input, output) => ExportPTCAsync(settings, output)),
		};
		public async UniTask ExportPTCAsync(MapViewerSettings settings, string outputDir) {
			using (var context = new MapViewerContext(settings)) {
				// Get the deserializer
				var s = context.Deserializer;

				// Load the ROM
				await LoadFilesAsync(context);

				SNA_RelocationBigFile bigFile = null;
				s.DoAt(context.FilePointer(LevelsBigFile), () => {
					s.FillCacheForReadAsync(PTC_BigFileEncoder.KeysSize + SNA_RelocationBigFile.MainHeaderSize);
					bigFile = FileFactory.Read<SNA_RelocationBigFile>(context, LevelsBigFile);
				});

				for (int occurIndex = 0; occurIndex < bigFile.OccurCount; occurIndex++) {
					for (int mapIndex = 0; mapIndex < bigFile.RelocationTablesCount / 4; mapIndex++) {
						await TimeController.WaitIfNecessary();
						for (int relocType = 0; relocType < 4; relocType++) {
							SNA_RelocationTable table = await bigFile.SerializeRelocationTable(s, default, occurIndex, mapIndex, (SNA_RelocationType)relocType);
							var relPath = Path.Combine($"Occur_{occurIndex}", $"Map_{mapIndex}", $"Map_{mapIndex}.{SNA_RelocationTable.GetExtension((SNA_RelocationType)relocType)}");
							var filePath = Path.Combine(outputDir, relPath);

							// Create and open the output file
							Directory.CreateDirectory(Path.GetDirectoryName(filePath));
							using (var outputStream = File.Create(filePath)) {
								// Create a context
								using (var writeContext = new MapViewerContext(outputDir, settings, log: false)) {
									// Create a key
									const string writeKey = "rt";

									// Add the file to the context
									writeContext.AddFile(new StreamFile(writeContext, writeKey, outputStream));

									// Write the data
									FileFactory.Write<SNA_RelocationTable>(writeContext, writeKey, table);
								}
							}
						}
					}
				}

			}

			UnityEngine.Debug.Log("Finished exporting PTC");
		}
		#endregion

		// TODO: Replace with GAMEDATA / GAMEDATABIN / ... later. This exists so we can use the top directory instead of GAMEDATA later on.
		public string GetGameDataFolder(CPA_Settings settings) => "/";

		public string GameDSCAlias => "game.dsc";

		public string LevelsBigFile => "World/Levels/LEVELS0.DAT";

		protected override List<string> FindFiles(MapViewerSettings settings) {
			return base.FindFiles(settings);
			// TODO
		}

		public override async UniTask LoadFilesAsync(Context context) {
			Endian endian = context.GetCPASettings().GetEndian;
			var cpaSettings = context.GetCPASettings();
			string GameDataFolder = GetGameDataFolder(cpaSettings);

			// At this point we can only load the DSB file
			string gameDscPath;
			if (cpaSettings.EngineVersionTree.HasParent(EngineVersion.CPA_Montreal)) {
				gameDscPath = Path.Combine(GameDataFolder, cpaSettings.ApplyPathCapitalization("gamedsc.bin", PathCapitalizationType.DSB));
			} else if (cpaSettings.EngineVersion == EngineVersion.TonicTroubleSE) {
				gameDscPath = Path.Combine(GameDataFolder, cpaSettings.ApplyPathCapitalization("GAME.DSC", PathCapitalizationType.DSB));
			} else {
				gameDscPath = Path.Combine(GameDataFolder, cpaSettings.ApplyPathCapitalization("Game.dsb", PathCapitalizationType.DSB));
			}
			var gameDsc = await context.AddLinearFileAsync(gameDscPath, endian);
			gameDsc.Alias = GameDSCAlias;

			// TEST: Load bigfile
			var bigFile = await context.AddLinearFileAsync(LevelsBigFile, endian, bigFileCacheLength: 0);
		}
		public async UniTask LoadBigFile(Context context) {
			GlobalLoadState.DetailedState = "Loading BigFile";
			await TimeController.WaitIfNecessary();

			var s = context.Deserializer;
			s.DoAt(context.FilePointer(LevelsBigFile), () => {
				s.FillCacheForReadAsync(PTC_BigFileEncoder.KeysSize + SNA_RelocationBigFile.MainHeaderSize);
				SNA_RelocationBigFile bigFile = FileFactory.Read<SNA_RelocationBigFile>(context, LevelsBigFile);
			});
			throw new NotImplementedException();
		}

		public virtual async UniTask LoadDSC(Context context) {
			// The DSC file contains information about directories which we need to load the files
			GlobalLoadState.DetailedState = "Loading DSC";
			await TimeController.WaitIfNecessary();

			SNA_File<SNA_Description> DSB = FileFactory.Read<SNA_File<SNA_Description>>(context, GameDSCAlias);

			throw new NotImplementedException();
		}

		public async UniTask LoadFix(Context context) {
			GlobalLoadState.DetailedState = "Loading fix";
			await TimeController.WaitIfNecessary();

			throw new NotImplementedException();
		}

		public async UniTask LoadLevel(Context context, string levelName) {
			GlobalLoadState.DetailedState = "Loading level";
			await TimeController.WaitIfNecessary();

			throw new NotImplementedException();
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context) {
			// Load DSC
			await LoadDSC(context);

			// Load fix
			await LoadFix(context);

			// Load level
			await LoadLevel(context, context.GetMapViewerSettings().Map);

			throw new NotImplementedException();
		}
	}
}
