using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.PS1;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Ubisoft.CPA.PS1;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using PS1GameInfo = OpenSpace.PS1.PS1GameInfo; // TODO: Remove this
using Reader = BinarySerializer.Reader;
using Util = BinarySerializer.Unity.Util;

namespace Raymap
{
	public class CPA_PS1Manager : LegacyGameManager
	{
		#region Private Constants

		private const ushort SectorSize = 0x800;
		private const string LevelHeaderFileName = "level.sys";
		private const string LevelDataFileName = "level.img";

		#endregion

		#region Game actions

		public override GameAction[] GetGameActions(MapViewerSettings settings) => new GameAction[]
		{
			new GameAction("Export Big Files", false, true, (input, output) => ExportBigFiles(settings, output)),
			new GameAction("Export UI/AGO Textures", false, true, (input, output) => ExportUIAndAGOTextures(settings, output)),
		};

		public async UniTask ExportBigFiles(MapViewerSettings settings, string outputDir)
		{
			PS1GameInfo gameInfo = GetGameInfo(settings);

			using var context = new MapViewerContext(settings);

			await LoadFilesAsync(context);

			foreach (PS1GameInfo.File fileInfo in gameInfo.files)
				await ExportBigFile(context, outputDir, gameInfo, fileInfo);

			Debug.Log("Finished exporting data");
		}

		public async UniTask ExportBigFile(Context context, string outputDir, PS1GameInfo gameInfo, PS1GameInfo.File fileInfo)
		{
			BinaryDeserializer s = context.Deserializer;
			BinaryFile binaryFile = s.Context.GetFile(fileInfo.BigFilePath);
			CPA_Settings settings = context.GetCPASettings();

			// Get the DAT memory blocks to export
			PS1GameInfo.File.MemoryBlock[] memoryBlocks = fileInfo.memoryBlocks;

			string exportDir = Path.Combine(outputDir, fileInfo.bigfile);
			PS1GameInfo.File.Type type = fileInfo.type;

			// Enumerate every memory block
			for (int i = 0; i < memoryBlocks.Length; i++)
			{
				PS1GameInfo.File.MemoryBlock b = memoryBlocks[i];

				string lvlExportDir;

				if (type == PS1GameInfo.File.Type.Map)
				{
					lvlExportDir = Path.Combine(exportDir, i < gameInfo.maps.Length 
						? gameInfo.maps[i] 
						: $"{fileInfo.bigfile}_{i}");
				}
				else
				{
					lvlExportDir = Path.Combine(exportDir, $"{i}");
				}

				byte[][] packedFiles = await ReadPackedFilesAsync(s, binaryFile, fileInfo, b.main_compressed);
				
				PackedFileType[] fileTypes = GetPackedFileTypes(settings, fileInfo, b);

				for (int fileIndex = 0; fileIndex < packedFiles.Length; fileIndex++)
				{
					PackedFileType fileType = fileTypes[fileIndex];
					byte[] fileData = packedFiles[fileIndex];

					switch (fileType)
					{
						case PackedFileType.TIM:
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vignette.tim"), fileData);
							break;

						case PackedFileType.TIM_Demo1:
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vignette_demo1.tim"), fileData);
							break;

						case PackedFileType.TIM_Demo2:
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vignette_demo2.tim"), fileData);
							break;

						case PackedFileType.TIM_Demo3:
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vignette_demo3.tim"), fileData);
							break;

						case PackedFileType.TIM_Demo4:
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vignette_demo4.tim"), fileData);
							break;

						case PackedFileType.VB:
						{
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "sounds.vb"), fileData);
							using MemoryStream ms = new MemoryStream(fileData);
							using Reader r = new Reader(ms);
							int soundIndex = 0;
							while (r.BaseStream.Position < r.BaseStream.Length)
							{
								int numSamples = r.ReadInt32();
								byte[] bs = r.ReadBytes(numSamples * 8);
								Util.ByteArrayToFile(Path.Combine(lvlExportDir, $"sounds_{soundIndex}.vb"), bs);
								soundIndex++;
							}
							break;
						}

						case PackedFileType.XTP:
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vram.xtp"), fileData);
							break;
						
						case PackedFileType.SYS:
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "level.sys"), fileData);
							break;

						case PackedFileType.PXE_Code:
							// The data file always appears after the main exe file
							byte[] exe = fileData;
							fileIndex++;
							byte[] exeData = packedFiles[fileIndex];

							int exeLength = exe.Length;
							Array.Resize(ref exe, exeLength + exeData.Length);
							Array.Copy(exeData, 0, exe, exeLength, exeData.Length);

							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "executable.pxe"), exe);
							break;
						
						//case PackedFileType.PXE_1:
						//	break;
						
						case PackedFileType.IMG:
							if (type == PS1GameInfo.File.Type.Map)
								Util.ByteArrayToFile(Path.Combine(lvlExportDir, "level.img"), fileData);
							else if (type == PS1GameInfo.File.Type.Actor)
								Util.ByteArrayToFile(Path.Combine(lvlExportDir, "actor.img"), fileData);
							break;

						default:
							throw new ArgumentOutOfRangeException();
					}

					await TimeController.WaitIfNecessary();
				}

				Util.ByteArrayToFile(Path.Combine(lvlExportDir, "overlay_game.img"), 
					await ReadDataBlockAsync(s, binaryFile, fileInfo, b.overlay_game));
				Util.ByteArrayToFile(Path.Combine(lvlExportDir, "overlay_cine.img"), 
					await ReadDataBlockAsync(s, binaryFile, fileInfo, b.overlay_cine));

				await TimeController.WaitIfNecessary();

				for (int j = 0; j < b.cutscenes.Length; j++)
				{
					byte[] cutsceneAudioBlk = await ReadDataBlockAsync(s, binaryFile, fileInfo, b.cutscenes[j]);

					if (cutsceneAudioBlk == null) 
						continue;
					
					Util.ByteArrayToFile(Path.Combine(lvlExportDir, $"stream_full_{j}.blk"), cutsceneAudioBlk);
					SplitCutsceneStream(context, cutsceneAudioBlk, out byte[] cutsceneAudio, out byte[] cutsceneFrames);
					Util.ByteArrayToFile(Path.Combine(lvlExportDir, $"stream_audio_{j}.blk"), cutsceneAudio);
					Util.ByteArrayToFile(Path.Combine(lvlExportDir, $"stream_frames_{j}.blk"), cutsceneFrames);
				}

				Debug.Log($"Exported memory block {i + 1}/{memoryBlocks.Length} for {fileInfo.BigFilePath}");
				await TimeController.WaitIfNecessary();
			}
		}

		public async UniTask ExportUIAndAGOTextures(MapViewerSettings settings, string outputDir)
		{
			PS1GameInfo gameInfo = GetGameInfo(settings);

			using var context = new MapViewerContext(settings);

			await LoadFilesAsync(context);

			CPA_Settings cpaSettings = context.GetCPASettings();
			BinaryDeserializer s = context.Deserializer;
			PS1GameInfo.File mainFileInfo = gameInfo.files.First(x => x.fileID == 0);

			int memBlockIndex = 0;

			foreach (PS1GameInfo.File.MemoryBlock memBlock in mainFileInfo.memoryBlocks)
			{
				// Read the packed files
				Dictionary<PackedFileType, byte[]> packedFiles = await ReadAndCategorizePackedFilesAsync(
					s: s,
					binaryFile: s.Context.GetFile(mainFileInfo.BigFilePath),
					fileInfo: mainFileInfo,
					memBlock: memBlock);

				GAM_Level_PS1 Level = new GAM_Level_PS1(context);

				Level.VRAM = LoadVRAM(cpaSettings, packedFiles[PackedFileType.XTP]);

				// Load the level memory
				LoadLevelMemory(context, packedFiles[PackedFileType.IMG], memBlock);

				// Load the Global Pointer Table
				GAM_GlobalPointerTable gpt = LoadGlobalPointerTable(context, packedFiles[PackedFileType.SYS]);

				string levelName = gameInfo.maps[memBlockIndex];

				// Export UI textures
				for (int i = 0; i < gpt.UITexturesCount; i++)
				{
					string name = gpt.UITexturesNames[i].Value.Name;
					int width = gpt.UITexturesWidths[i];
					int height = gpt.UITexturesHeights[i];
					PS1_TSB tsb = gpt.UITexturesTSB[i];
					PS1_CBA cba = gpt.UITexturesCBA[i];
					byte x = gpt.UITexturesX[i];
					byte y = gpt.UITexturesY[i];

					Texture2D tex = Level.VRAM.GetTexture(width, height, tsb, cba, x, y);

					tex.Export(Path.Combine(outputDir, "UI", levelName, name));
				}

				// Export AGO textures
				for (int i = 0; i < gpt.AGOTexturesCount; i++)
				{
					int width = gpt.Rush_AGOTexturesWidths?[i] ?? -1;
					int height = gpt.Rush_AGOTexturesHeights?[i] ?? -1;
					PS1_TSB tsb = gpt.AGOTexturesTSB[i];
					PS1_CBA cba = gpt.AGOTexturesCBA[i];
					byte x = gpt.AGOTexturesX[i];
					byte y = gpt.AGOTexturesY[i];

					if (width != -1 && height != -1)
					{
						Texture2D tex = Level.VRAM.GetTexture(width, height, tsb, cba, x, y);
						tex.Export(Path.Combine(outputDir, "AGO", levelName, $"{i}"));
					}
					else
					{
						int pageW = tsb.TP switch
						{
							PS1_TSB.TexturePageTP.CLUT_8Bit => 128,
							PS1_TSB.TexturePageTP.CLUT_4Bit => 256,
							_ => 64
						};

						int maxW = pageW - x;
						int maxH = 256 - y;

						for (int xx = 0; xx < 5; xx++) 
						{
							ushort w = (ushort)(0x10 << xx);

							if (w > maxW) 
								continue;

							for (int yy = 0; yy < 5; yy++) 
							{
								ushort h = (ushort)(0x10 << yy);

								if (h > maxH) 
									continue;

								try
								{
									Texture2D tex = Level.VRAM.GetTexture(w, h, tsb, cba, x, y);
									tex.Export(Path.Combine(outputDir, "AGO", levelName, $"{i}_{w}x{h}"));
								} 
								catch { }
							}
						}
					}
				}

				context.RemoveFile(LevelHeaderFileName);
				context.RemoveFile(LevelDataFileName);

				memBlockIndex++;
			}

			Debug.Log("Finished exporting textures");
		}

		// TODO: Refactor to use BinarySerializer
		public void SplitCutsceneStream(Context context, byte[] cutsceneData, out byte[] cutsceneAudio, out byte[] cutsceneFrames)
		{
			List<byte[]> cutsceneAudioList = new List<byte[]>();
			List<byte[]> cutsceneFramesList = new List<byte[]>();

			using (MemoryStream ms = new MemoryStream(cutsceneData))
			{
				using Reader reader = new Reader(ms, context.GetCPASettings().GetEndian == Endian.Little);
				
				uint size_frame_packet = 1;
				while (reader.BaseStream.Position < reader.BaseStream.Length && size_frame_packet > 0)
				{
					size_frame_packet = reader.ReadUInt32();

					if (size_frame_packet == 0xFFFFFFFF) 
						continue;
					
					reader.BaseStream.Position -= 4;
					cutsceneFramesList.Add(reader.ReadBytes((int)size_frame_packet + 4));
					bool readParts = true;
					while (readParts && reader.BaseStream.Position < reader.BaseStream.Length)
					{
						uint size = reader.ReadUInt32();

						if (size == 0xFFFFFFFE)
						{
							readParts = false;
							if (reader.BaseStream.Position % SectorSize > 0)
							{
								reader.BaseStream.Position = SectorSize * ((reader.BaseStream.Position / SectorSize) + 1);
							}
						}
						else
						{
							bool isNull = (size & 0x80000000) != 0;
							size = size & 0x7FFFFFFF;
							cutsceneAudioList.Add(isNull
								? Enumerable.Repeat((byte)0x0, (int)size).ToArray()
								: reader.ReadBytes((int)size));
						}
					}
				}
			}
			cutsceneAudio = cutsceneAudioList.SelectMany(i => i).ToArray();
			cutsceneFrames = cutsceneFramesList.SelectMany(i => i).ToArray();
		}

		#endregion

		#region Manager Methods

		protected override List<string> FindFiles(MapViewerSettings settings) => GetGameInfo(settings).maps.ToList();

		public override async UniTask<Unity_Level> LoadAsync(Context context)
		{
			// Get properties
			CPA_Settings cpaSettings = context.GetCPASettings();
			MapViewerSettings settings = context.GetMapViewerSettings();
			BinaryDeserializer s = context.Deserializer;
			GAM_Level_PS1 Level = new GAM_Level_PS1(context);

			PS1GameInfo gameInfo = GetGameInfo(settings);
			PS1GameInfo.File mainFileInfo = gameInfo.files.First(x => x.fileID == 0);
			PS1GameInfo.File.MemoryBlock levelMemBlock = mainFileInfo.memoryBlocks[Array.IndexOf(gameInfo.maps, settings.Map)];
			
			GlobalLoadState.DetailedState = $"Loading files";
			await TimeController.WaitIfNecessary();

			// TODO: Load actor files

			// Read the packed files
			Dictionary<PackedFileType, byte[]> packedFiles = await ReadAndCategorizePackedFilesAsync(
				s: s, 
				binaryFile: s.Context.GetFile(mainFileInfo.BigFilePath), 
				fileInfo: mainFileInfo, 
				memBlock: levelMemBlock);

			GlobalLoadState.DetailedState = $"Loading VRAM";
			await TimeController.WaitIfNecessary();

			// Load the VRAM
			Level.VRAM = LoadVRAM(cpaSettings, packedFiles[PackedFileType.XTP]);

			GlobalLoadState.DetailedState = $"Loading level";
			await TimeController.WaitIfNecessary();

			// Load the level memory
			LoadLevelMemory(context, packedFiles[PackedFileType.IMG], levelMemBlock);

			var sw = System.Diagnostics.Stopwatch.StartNew();

			// Load the level header
			Level.GlobalPointerTable = LoadGlobalPointerTable(context, packedFiles[PackedFileType.SYS]);

			// TODO: Load overlays & cutscene data

			sw.Stop();
			Debug.Log($"Loaded level header in {sw.ElapsedMilliseconds} ms");

			Unity_Level_CPAPS1 unityLevel = new Unity_Level_CPAPS1() {
				LevelData = Level
			};

			return unityLevel;
		}

		public PS1GameInfo GetGameInfo(MapViewerSettings settings) => 
			// TODO: Re-implement game info. It will most likely have to remain hard-coded as some games like DD have most of the values
			//       set from the load function rather than data tables.
			PS1GameInfo.Games[GetLegacyMode(settings)];

		public async UniTask<byte[][]> ReadPackedFilesAsync(BinaryDeserializer s, BinaryFile binaryFile, PS1GameInfo.File fileInfo, PS1GameInfo.File.LBA lba)
		{
			if (lba.lba < fileInfo.baseLBA || lba.size <= 0)
				return null;

			// Get the length of the archive
			long length = lba.size * SectorSize;

			// Go to the archive
			s.Goto(new Pointer((lba.lba - fileInfo.baseLBA) * SectorSize, binaryFile));
			
			// Fill cache
			await s.FillCacheForReadAsync(length);

			// Read the archive
			PackedFileArchive archive = s.SerializeObject<PackedFileArchive>(default, x => x.Pre_MaxLength = length, name: "PackedFiles");

			// Return the file data
			return archive.Files.Select(x => x.GetFileBytes()).Where(x => x != null).ToArray();
		}

		public async UniTask<Dictionary<PackedFileType, byte[]>> ReadAndCategorizePackedFilesAsync(BinaryDeserializer s, BinaryFile binaryFile, PS1GameInfo.File fileInfo, PS1GameInfo.File.MemoryBlock memBlock)
		{
			byte[][] packedFileDatas = await ReadPackedFilesAsync(s, binaryFile, fileInfo, memBlock.main_compressed);
			PackedFileType[] fileTypes = GetPackedFileTypes(s.GetCPASettings(), fileInfo, memBlock);
			return packedFileDatas.
				Select((x, i) => new { Data = x, Type = fileTypes[i] }).
				ToDictionary(x => x.Type, x => x.Data);
		}

		public async UniTask<byte[]> ReadDataBlockAsync(BinaryDeserializer s, BinaryFile file, PS1GameInfo.File fileInfo, PS1GameInfo.File.LBA lba)
		{
			if (lba.lba < fileInfo.baseLBA || lba.size <= 0)
				return null;

			// Go to the data
			s.Goto(new Pointer((lba.lba - fileInfo.baseLBA) * SectorSize, file));

			// Fill cache
			await s.FillCacheForReadAsync((int)lba.size);

			// Read and return the data
			return s.SerializeArray<byte>(default, lba.size, name: "Data");
		}

		public PackedFileType[] GetPackedFileTypes(CPA_Settings settings, PS1GameInfo.File fileInfo, PS1GameInfo.File.MemoryBlock b)
		{
			var types = new List<PackedFileType>();

			if (fileInfo.type == PS1GameInfo.File.Type.Map)
			{
				// TODO: Potentially find a better, less hacky, way of dealing with multiple vignette files?
				if (settings.Mode is CPA_GameMode.Rayman2PS1Demo or CPA_GameMode.Rayman2PS1Demo_SLUS_90095)
				{
					types.Add(PackedFileType.TIM_Demo1);
					types.Add(PackedFileType.TIM_Demo2);
					types.Add(PackedFileType.TIM_Demo3);

					if (b.vignettes == 4)
						types.Add(PackedFileType.TIM_Demo4);
				}
				else if (settings.EngineVersion != EngineVersion.RaymanRush_PS1 && !b.exeOnly)
				{
					types.Add(PackedFileType.TIM);
				}

				if (!b.exeOnly && b.inEngine)
				{
					if (b.hasSoundEffects)
						types.Add(PackedFileType.VB);

					types.Add(PackedFileType.XTP);
					types.Add(PackedFileType.SYS);
				}

				types.Add(PackedFileType.PXE_Code);
				types.Add(PackedFileType.PXE_Data);

				if (!b.exeOnly && b.inEngine)
					types.Add(PackedFileType.IMG);
			}
			else if (fileInfo.type == PS1GameInfo.File.Type.Actor)
			{
				types.Add(PackedFileType.XTP);
				types.Add(PackedFileType.IMG);
			}
			else if (fileInfo.type == PS1GameInfo.File.Type.Sound)
			{
				types.Add(PackedFileType.VB);
			}

			return types.ToArray();
		}

		public PS1_VRAM LoadVRAM(CPA_Settings settings, byte[] xtp)
		{
			PS1_VRAM vram = new PS1_VRAM();

			int startXPage = settings.EngineVersion != EngineVersion.JungleBook_PS1 ? 5 : 8;
			vram.CurrentXPage = startXPage;

			int width = Mathf.CeilToInt(xtp.Length / (float)(PS1_VRAM.PageHeight * 2));
			vram.AddData(xtp, width);

			return vram;
		}

		public void LoadLevelMemory(Context context, byte[] data, PS1GameInfo.File.MemoryBlock memBlock)
		{
			// Add the level data as a memory mapped file. The header references data here.
			context.AddMemoryMappedStreamFile(LevelDataFileName, data, memBlock.address, context.GetCPASettings().GetEndian);
		}

		public GAM_GlobalPointerTable LoadGlobalPointerTable(Context context, byte[] data)
		{
			// Add the level header file as a linear file as it doesn't matter where it's allocated in memory. This will parse the level data.
			context.AddStreamFile(LevelHeaderFileName, data, context.GetCPASettings().GetEndian);
			return FileFactory.Read<GAM_GlobalPointerTable>(context, LevelHeaderFileName);
		}

		public override async UniTask LoadFilesAsync(Context context)
		{
			Endian endian = context.GetCPASettings().GetEndian;
			PS1GameInfo gameInfo = GetGameInfo(context.GetMapViewerSettings());

			// TODO: Don't always load all files
			foreach (PS1GameInfo.File fileInfo in gameInfo.files)
				await context.AddLinearFileAsync(fileInfo.BigFilePath, endian, bigFileCacheLength: SectorSize);
		}

		#endregion

		#region Data Types

		public enum PackedFileType
		{
			TIM, // Vignette
			TIM_Demo1, // Vignette
			TIM_Demo2, // Vignette
			TIM_Demo3, // Vignette
			TIM_Demo4, // Vignette
			VB, // Sound
			XTP, // VRAM
			SYS, // Level header
			PXE_Code, // Executable
			PXE_Data, // Executable data
			IMG, // Level data
		}

		#endregion
	}
}