using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Raymap {
	// TODO: This will be the equivalent of the R2Loader
	public class CPA_SNAManager : LegacyGameManager {

		#region Game actions
		public override GameAction[] GetGameActions(MapViewerSettings settings) => new GameAction[] {
			new GameAction("Export Relocation Table BigFile", false, true, (input, output) => ExportPTCAsync(settings, output)),
			new GameAction("Export SNA Blocks", false, true, (input, output) => ExportSNABlocksAsync(settings, output)),
		};
		public async UniTask ExportPTCAsync(MapViewerSettings settings, string outputDir) {
			using (var context = new MapViewerContext(settings)) {
				// Get the deserializer
				var s = context.Deserializer;

				// Load the ROM
				await LoadFilesAsync(context);

				// Download all possible files
				await LoadPathsAsync(context);

				// Load DSC & bigfile
				await LoadDSC(context);

				var bigFile = ((CPA_Globals_SNA)s.GetCPAGlobals()).RelocationBigFile;

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

		public async UniTask ExportSNABlocksAsync(MapViewerSettings settings, string outputDir) {
			using (var context = new MapViewerContext(settings)) {
				// Get the deserializer
				var s = context.Deserializer;

				// Load the ROM
				await LoadFilesAsync(context);

				// Download all possible files
				await LoadPathsAsync(context);

				// Load DSC & bigfile
				await LoadDSC(context);

				// Download all possible files
				await LoadPathsAsync(context);

				var levels = GetLevels(settings);

				SNA_File<SNA_MemorySnapshot> sna = FileFactory.Read<SNA_File<SNA_MemorySnapshot>>(context, CPA_Path.FixSNA.ToString());
				await ExportSNA(sna?.Value, "Fix");
				foreach (var map in levels.Children.Select(l => l.Id)) {
					// Now that the DSC is loaded, download other files
					await LoadPathsAsync(context, mapName: map);

					// Load level SNA
					sna = FileFactory.Read<SNA_File<SNA_MemorySnapshot>>(context, CPA_Path.LevelSNA.ToString());
					await ExportSNA(sna?.Value, map);
					context.RemoveFile(sna.Offset.File);

				}

				async UniTask ExportSNA(SNA_MemorySnapshot sna, string mapName) {
					if(sna == null) return;
					int blockIndex = 0;
					foreach (var blk in sna.Blocks) {
						if (blk.BlockSize == 0) continue;
						Util.ByteArrayToFile(Path.Combine(outputDir, mapName, $"{blockIndex} - {blk.Module}_{blk.Bloc} - {blk.ModuleTranslation?.ToString() ?? ("Undefined")}.bin"), blk.Block);
						blockIndex++;
					}
					await TimeController.WaitIfNecessary();
				}
			}

			UnityEngine.Debug.Log("Finished exporting SNA blocks");
		}
		#endregion

		protected override List<string> FindFiles(MapViewerSettings settings) {
			return base.FindFiles(settings);
			// TODO
		}

		public async UniTask LoadPathsAsync(Context context, string mapName = null) {
			// Init globals if it doesn't exist yet, we need that to get access to the paths
			if (context.GetCPAGlobals(throwIfNotFound: false) == null) {
				InitGlobals(context);
			}

			Endian endian = context.GetCPASettings().GetEndian;
			var cpaSettings = context.GetCPASettings();

			var cpaGlobals = context.GetCPAGlobals();
			var paths = cpaGlobals.GetPaths(mapName);

			foreach (var p in paths) {
				if(p.Value == null) continue;
				var path = HackPath(p.Value); // Temporary hack: remove "GameData" from start of path.

				if(context.FileExists(path)) continue;
				switch (p.Key) {
					case CPA_Path.RelocationBigFile:
						var bigf = await context.AddLinearFileAsync(path, endian, bigFileCacheLength: 0);
						UnityEngine.Debug.Log($"{p.Key}: {path}");
						if (bigf != null) bigf.Alias = p.Key.ToString();
						break;
					default:
						var linf = await context.AddLinearFileAsync(path, endian);
						UnityEngine.Debug.Log($"{p.Key}: {path}");
						if(linf != null) linf.Alias = p.Key.ToString();
						break;
				}
			}
		}

		private string HackPath(string path) {
			// Temporary hack: remove "GameData" from start of path.
			// Later we'll change the configured paths in raymap so they point to where the exe is.
			if (path.ToLower().StartsWith("gamedata/")) {
				path = path.Substring("gamedata/".Length);
			} else if (path.ToLower().StartsWith("/gamedata/")) {
				path = path.Substring("/gamedata/".Length);
			} else {
				path = $"../{path}";
			}
			return path;
		}

		public async UniTask LoadBigFile(Context context) {
			GlobalLoadState.DetailedState = "Loading BigFile";
			await TimeController.WaitIfNecessary();

			var cpaGlobals = (CPA_Globals_SNA)context.GetCPAGlobals();
			var relocBigfilePath = HackPath(context.NormalizePath(cpaGlobals.RelocationBigFilePath, false));
			//if (context.FileManager.FileExists(context.GetAbsoluteFilePath(relocBigfilePath))) {
			var bigf = await context.AddLinearFileAsync(relocBigfilePath, context.GetCPASettings().GetEndian, bigFileCacheLength: 0);
			UnityEngine.Debug.Log(relocBigfilePath);
			if (bigf != null) bigf.Alias = CPA_Path.RelocationBigFile.ToString();
			//}
			if (context.FileExists(CPA_Path.RelocationBigFile.ToString())) {
				var s = context.Deserializer;
				s.Goto(context.FilePointer(CPA_Path.RelocationBigFile.ToString()));
				await s.FillCacheForReadAsync(PTC_BigFileEncoder.KeysSize + SNA_RelocationBigFile.MainHeaderSize);
				
				s.DoAt(context.FilePointer(CPA_Path.RelocationBigFile.ToString()), () => {
					cpaGlobals.RelocationBigFile = FileFactory.Read<SNA_RelocationBigFile>(context, CPA_Path.RelocationBigFile.ToString());
				});
			}
		}

		public virtual async UniTask LoadDSC(Context context) {
			// The DSC file contains information about directories which we need to load the files
			GlobalLoadState.DetailedState = "Loading DSC";
			await TimeController.WaitIfNecessary();

			var cpaGlobals = (CPA_Globals_SNA)context.GetCPAGlobals();
			cpaGlobals.GameDSB = FileFactory.Read<SNA_File<SNA_Description>>(context, CPA_Path.GameDSC.ToString());

			// Also load relocation bigfile here
			await LoadBigFile(context);
		}

		public async UniTask LoadFix(Context context) {
			GlobalLoadState.DetailedState = "Loading fix";
			await TimeController.WaitIfNecessary();

			SNA_File<SNA_MemorySnapshot> sna = FileFactory.Read<SNA_File<SNA_MemorySnapshot>>(context, CPA_Path.FixSNA.ToString());
			SNA_PointerFile<SNA_RelocationTable> rtb = FileFactory.Read<SNA_PointerFile<SNA_RelocationTable>>(context, CPA_Path.FixRTB.ToString());
		}

		public async UniTask LoadLevel(Context context, string levelName) {
			GlobalLoadState.DetailedState = "Loading level";
			await TimeController.WaitIfNecessary();

			var cpaGlobals = (CPA_Globals_SNA)context.GetCPAGlobals();
			cpaGlobals.Map = levelName;
			SNA_File<SNA_MemorySnapshot> sna = FileFactory.Read<SNA_File<SNA_MemorySnapshot>>(context, CPA_Path.LevelSNA.ToString());
			if (cpaGlobals.RelocationBigFile != null) {
				var mapIndex = cpaGlobals.MapIndex;
				if(!mapIndex.HasValue)
					throw new Exception($"Map {cpaGlobals?.Map} does not occur in Game.DSB and cannot be loaded");
				SNA_RelocationTable rtb = await cpaGlobals.RelocationBigFile.SerializeRelocationTable(context.Deserializer, default, 0, mapIndex.Value, SNA_RelocationType.SNA);
			} else {
				SNA_PointerFile<SNA_RelocationTable> rtb = FileFactory.Read<SNA_PointerFile<SNA_RelocationTable>>(context, CPA_Path.LevelRTB.ToString());
			}

			throw new NotImplementedException();
		}

		public virtual void InitGlobals(Context context) => new CPA_Globals_SNA(context);

		public override async UniTask<Unity_Level> LoadAsync(Context context) {
			// Download all possible files
			await LoadPathsAsync(context);

			// Load DSC
			await LoadDSC(context);

			// Now that the DSC is loaded, download other files
			await LoadPathsAsync(context, mapName: context.GetMapViewerSettings().Map);

			// Load fix
			await LoadFix(context);

			// Load level
			await LoadLevel(context, context.GetMapViewerSettings().Map);

			throw new NotImplementedException();
		}
	}
}
