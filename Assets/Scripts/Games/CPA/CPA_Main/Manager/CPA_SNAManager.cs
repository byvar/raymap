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
						Util.ByteArrayToFile(Path.Combine(outputDir, mapName, $"{blockIndex} - {blk.Module}_{blk.Block} - {blk.ModuleTranslation?.ToString() ?? ("Undefined")}.bin"), blk.Data);
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

		#region Memory
		public async UniTask InitFixMemory(Context context) {
			GlobalLoadState.DetailedState = "Loading fix memory";
			await TimeController.WaitIfNecessary();

			// Load SNA and relocation table
			SNA_File<SNA_MemorySnapshot> sna = FileFactory.Read<SNA_File<SNA_MemorySnapshot>>(context, CPA_Path.FixSNA.ToString());
			SNA_PointerFile<SNA_TemporaryMemoryBlock> gpt = FileFactory.Read<SNA_PointerFile<SNA_TemporaryMemoryBlock>>(context, CPA_Path.FixGPT.ToString());
			SNA_PointerFile<SNA_TemporaryMemoryBlock> ptx = FileFactory.Read<SNA_PointerFile<SNA_TemporaryMemoryBlock>>(context, CPA_Path.FixPTX.ToString());
			SNA_PointerFile<SNA_TemporaryMemoryBlock> snd = FileFactory.Read<SNA_PointerFile<SNA_TemporaryMemoryBlock>>(context, CPA_Path.FixSND.ToString());
			
			ProcessSNA(context, sna?.Value, LoadRelocationFile(CPA_Path.FixRTB)); // SNA
			ProcessTMP(context, sna?.Value, ContentID(CPA_Path.FixGPT), gpt?.Value, LoadRelocationFile(CPA_Path.FixRTP)); // GlobalPointers
			ProcessTMP(context, sna?.Value, ContentID(CPA_Path.FixPTX), ptx?.Value, LoadRelocationFile(CPA_Path.FixRTT)); // Textures
			ProcessTMP(context, sna?.Value, ContentID(CPA_Path.FixSND), snd?.Value, LoadRelocationFile(CPA_Path.FixRTS)); // Sound

			SNA_RelocationTable LoadRelocationFile(CPA_Path path) {
				SNA_PointerFile<SNA_RelocationTable> rt = FileFactory.Read<SNA_PointerFile<SNA_RelocationTable>>(context, path.ToString());
				return rt?.Value;
			}
		}

		public async UniTask InitLevelMemory(Context context, string levelName) {
			GlobalLoadState.DetailedState = "Loading level memory";
			await TimeController.WaitIfNecessary();

			var cpaGlobals = (CPA_Globals_SNA)context.GetCPAGlobals();
			cpaGlobals.Map = levelName;
			SNA_File<SNA_MemorySnapshot> sna = FileFactory.Read<SNA_File<SNA_MemorySnapshot>>(context, CPA_Path.LevelSNA.ToString());
			SNA_PointerFile<SNA_TemporaryMemoryBlock> gpt = FileFactory.Read<SNA_PointerFile<SNA_TemporaryMemoryBlock>>(context, CPA_Path.LevelGPT.ToString());
			SNA_PointerFile<SNA_TemporaryMemoryBlock> ptx = FileFactory.Read<SNA_PointerFile<SNA_TemporaryMemoryBlock>>(context, CPA_Path.LevelPTX.ToString());
			SNA_PointerFile<SNA_TemporaryMemoryBlock> snd = FileFactory.Read<SNA_PointerFile<SNA_TemporaryMemoryBlock>>(context, CPA_Path.LevelSND.ToString());

			ProcessSNA(context, sna?.Value, await LoadRelocationFile(CPA_Path.LevelRTB, SNA_RelocationType.SNA));
			ProcessTMP(context, sna?.Value, ContentID(CPA_Path.LevelGPT), gpt?.Value, await LoadRelocationFile(CPA_Path.LevelRTP, SNA_RelocationType.GlobalPointers));
			ProcessTMP(context, sna?.Value, ContentID(CPA_Path.LevelPTX), ptx?.Value, await LoadRelocationFile(CPA_Path.LevelRTT, SNA_RelocationType.Textures));
			ProcessTMP(context, sna?.Value, ContentID(CPA_Path.LevelSND), snd?.Value, await LoadRelocationFile(CPA_Path.LevelRTS, SNA_RelocationType.Sound));

			async UniTask<SNA_RelocationTable> LoadRelocationFile(CPA_Path path, SNA_RelocationType type) {
				if (cpaGlobals.RelocationBigFile != null) {
					var mapIndex = cpaGlobals.MapIndex;
					if (!mapIndex.HasValue)
						throw new Exception($"Map {cpaGlobals?.Map} does not occur in Game.DSB and cannot be loaded");
					return await cpaGlobals.RelocationBigFile.SerializeRelocationTable(context.Deserializer, default, 0, mapIndex.Value, type);
				} else {
					SNA_PointerFile<SNA_RelocationTable> rt = FileFactory.Read<SNA_PointerFile<SNA_RelocationTable>>(context, path.ToString());
					return rt?.Value;
				}
			}

			//LogIncorrectReloctaion(context);
		}

		public void LogIncorrectReloctaion(Context context) {
			foreach (var f in context.MemoryMap.Files) {
				if (f is SNA_BlockFile bf) {
					var relocBlock = bf.RelocationBlock;
					if (relocBlock == null) continue;
					foreach (var ptr in relocBlock.Pointers) {
						var absoluteValue = ptr.Pointer;
						var relocFile = bf?.Snapshot?.Blocks?.LastOrDefault(
								x => x.Module == ptr.TargetModule && x.Block == ptr.TargetBlock);
						var memoryMappedFile = bf?.Snapshot?.Blocks?.LastOrDefault(b =>
							b.BeginBlock != SNA_MemoryBlock.InvalidBeginBlock
							&& absoluteValue >= b.BeginBlock
							&& absoluteValue < b.EndBlock + 1);
						if (relocFile?.BlockName != memoryMappedFile?.BlockName) {
							context.SerializerLog?.Log($"{bf.FilePath} - Incorrect relocation: {absoluteValue:X8} - relocated to {relocFile?.BlockName} (base: {relocFile?.BeginBlock:X8}), should be {memoryMappedFile?.BlockName} (base: {memoryMappedFile?.BeginBlock:X8})");
						}
					}
				}
			}
		}

		public void ProcessSNA(Context context, SNA_MemorySnapshot snapshot, SNA_RelocationTable relocationTable) {
			foreach (var block in snapshot.Blocks) {
				if (block.BlockSize == 0) continue;
				var relBlock = relocationTable.Blocks.FirstOrDefault(r => r.Block == block.Block && r.Module == block.Module && !r.IsInvalidBlock);
				SNA_DataBlockFile bf = context.AddFile(new SNA_DataBlockFile(context, snapshot, block, relBlock));
			}
			// Also add empty blocks, those can be pointed to but never read
			foreach (var block in snapshot.Blocks.Reverse()) {
				if (block.BlockSize == 0 && !context.FileExists(block.BlockName)
					&& block.BeginBlock != block.EndBlock
					&& block.BeginBlock != SNA_MemoryBlock.InvalidBeginBlock) {
					SNA_DataBlockFile bf = context.AddFile(new SNA_DataBlockFile(context, snapshot, block, null));
				}
			}
		}
		public SNA_PointerBlockFile ProcessTMP(Context context, SNA_MemorySnapshot snapshot, string name, SNA_TemporaryMemoryBlock block, SNA_RelocationTable relocationTable) {
			var relBlock = relocationTable.Blocks[0];
			return context.AddFile(new SNA_PointerBlockFile(context, snapshot, name, block.Data, relBlock));
		}
#endregion

		public string ContentID(CPA_Path path) => $"{path}_Content";
		public virtual void InitGlobals(Context context) => new CPA_Globals_SNA(context);

		public async UniTask LoadFix(Context context) {
			GlobalLoadState.DetailedState = "Loading fix";
			await TimeController.WaitIfNecessary();

			// Load SNA and relocation table
			var gptContents = FileFactory.Read<GAM_GlobalPointers_Fix>(context, ContentID(CPA_Path.FixGPT));
			var ptxContents = FileFactory.Read<GLI_GlobalTextures>(context, ContentID(CPA_Path.FixPTX), onPreSerialize: (_, p) => p.Pre_IsFix = true);
			var sndContents = FileFactory.Read<SND_SoundPointers>(context, ContentID(CPA_Path.FixSND));
		}
		public async UniTask LoadLevel(Context context, string mapName) {
			GlobalLoadState.DetailedState = "Loading level";
			await TimeController.WaitIfNecessary();

			// The DSC file contains information about directories which we need to load the files
			var cpaGlobals = (CPA_Globals_SNA)context.GetCPAGlobals();
			cpaGlobals.LevelDSB = FileFactory.Read<SNA_File<SNA_Description>>(context, CPA_Path.LevelDSC.ToString());

			// Load SNA and relocation table
			var gptContents = FileFactory.Read<GAM_GlobalPointers_Level>(context, ContentID(CPA_Path.LevelGPT));
			var ptxContents = FileFactory.Read<GLI_GlobalTextures>(context, ContentID(CPA_Path.LevelPTX), onPreSerialize: (_, p) => p.Pre_IsFix = false);
			var sndContents = FileFactory.Read<SND_SoundPointers>(context, ContentID(CPA_Path.LevelSND));
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context) {
			// Download all possible files
			await LoadPathsAsync(context);

			// Load DSC
			await LoadDSC(context);

			// Now that the DSC is loaded, download other files
			await LoadPathsAsync(context, mapName: context.GetMapViewerSettings().Map);

			// Load SNA files, init virtual SNA block files
			await InitFixMemory(context);
			await InitLevelMemory(context, context.GetMapViewerSettings().Map);

			// Load content of SNA files
			await LoadFix(context);
			await LoadLevel(context, context.GetMapViewerSettings().Map);

			throw new NotImplementedException();
		}
	}
}
