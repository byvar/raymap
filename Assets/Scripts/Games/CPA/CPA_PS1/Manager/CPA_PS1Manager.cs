using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Ubisoft.CPA.PS1;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using OpenSpace.PS1;
using UnityEngine;
using Reader = BinarySerializer.Reader;
using Util = BinarySerializer.Unity.Util;

namespace Raymap
{
	public class CPA_PS1Manager : LegacyGameManager
	{
		#region Private Constants

		private const ushort SectorSize = 0x800;

		#endregion

		#region Game actions

		public override GameAction[] GetGameActions(MapViewerSettings settings) => new GameAction[]
		{
			new GameAction("Export Big Files", false, true, (input, output) => ExportBigFiles(settings, output))
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

			uint cineDataBaseAddress = 0;

			// Enumerate every memory block
			for (int i = 0; i < memoryBlocks.Length; i++)
			{
				int gptIndex = 0;

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

				int fileIndex = 0;

				if (type == PS1GameInfo.File.Type.Map)
				{
					if (settings.Mode != CPA_GameMode.RaymanRushPS1 && !b.exeOnly)
						Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vignette.tim"), packedFiles[fileIndex++]);

					if (!b.exeOnly && b.inEngine)
					{
						if (b.hasSoundEffects)
						{
							Util.ByteArrayToFile(Path.Combine(lvlExportDir, "sound.vb"), packedFiles[fileIndex++]);
							using MemoryStream ms = new MemoryStream(packedFiles[fileIndex - 1]);
							using Reader r = new Reader(ms);
							int soundIndex = 0;
							while (r.BaseStream.Position < r.BaseStream.Length)
							{
								int numSamples = r.ReadInt32();
								byte[] bs = r.ReadBytes(numSamples * 8);
								Util.ByteArrayToFile(Path.Combine(lvlExportDir, $"sound_{soundIndex}.vb"), bs);
								soundIndex++;
							}
						}

						Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vram.xtp"), packedFiles[fileIndex++]);
						Util.ByteArrayToFile(Path.Combine(lvlExportDir, "level.sys"), packedFiles[fileIndex++]);

						gptIndex = fileIndex - 1;
					}

					byte[] exe = packedFiles[fileIndex++];
					byte[] exeData = packedFiles[fileIndex++];

					int exeLength = exe.Length;
					Array.Resize(ref exe, exeLength + exeData.Length);
					Array.Copy(exeData, 0, exe, exeLength, exeData.Length);

					Util.ByteArrayToFile(Path.Combine(lvlExportDir, "executable.pxe"), exe);

					if (!b.exeOnly && b.inEngine)
					{
						Util.ByteArrayToFile(Path.Combine(lvlExportDir, "level.img"), packedFiles[fileIndex++]);

						int lvlIndex = fileIndex - 1;
						uint length = (uint)packedFiles[lvlIndex].Length;
						byte[] data = packedFiles[gptIndex];

						RelocatePointers(data, new Range(b.address, b.address + length));

						if (data[0x14c + 3] == 0x80)
							cineDataBaseAddress = BitConverter.ToUInt32(data, 0x14c);

						Util.ByteArrayToFile(Path.Combine(lvlExportDir, "level_relocated.sys"), data);
						data = packedFiles[lvlIndex];

						RelocatePointers(data, new Range(b.address, b.address + length));
						Util.ByteArrayToFile(Path.Combine(lvlExportDir, "level_relocated.img"), data);
					}
				}
				else if (type == PS1GameInfo.File.Type.Actor)
				{
					Util.ByteArrayToFile(Path.Combine(lvlExportDir, "vram.xtp"), packedFiles[fileIndex++]);
					Util.ByteArrayToFile(Path.Combine(lvlExportDir, "actor.img"), packedFiles[fileIndex++]);

					if (fileInfo.bigfile == "ACTOR1")
					{
						byte[] data = packedFiles[fileIndex - 1];
						uint baseAddress = gameInfo.actor1Address;
						uint length = (uint)data.Length;

						RelocatePointers(data, new Range(baseAddress, baseAddress + length));
						Util.ByteArrayToFile(Path.Combine(lvlExportDir, "actor_relocated.img"), data);
					}
				}
				else if (type == PS1GameInfo.File.Type.Sound)
				{
					Util.ByteArrayToFile(Path.Combine(lvlExportDir, "sound.vb"), packedFiles[fileIndex++]);
				}

				if (fileIndex != packedFiles.Length)
					Debug.LogWarning("Not all blocks were exported!");

				await TimeController.WaitIfNecessary();

				Util.ByteArrayToFile(Path.Combine(lvlExportDir, "overlay_game.img"), await ReadDataBlock(s, binaryFile, fileInfo, b.overlay_game));
				byte[] cineblock = await ReadDataBlock(s, binaryFile, fileInfo, b.overlay_cine);
				Util.ByteArrayToFile(Path.Combine(lvlExportDir, "overlay_cine.img"), cineblock);

				await TimeController.WaitIfNecessary();

				if (cineblock != null)
				{
					cineDataBaseAddress += 0x1f800 + 0x32 * 0xc00; // magic!

					RelocatePointers(cineblock, new Range[]
					{
						new Range(b.address, cineDataBaseAddress),
						new Range(cineDataBaseAddress, (uint)(cineDataBaseAddress + cineblock.Length)),
					});

					Util.ByteArrayToFile(Path.Combine(lvlExportDir, "overlay_cine_relocated.img"), cineblock);
				}

				await TimeController.WaitIfNecessary();

				for (int j = 0; j < b.cutscenes.Length; j++)
				{
					byte[] cutsceneAudioBlk = await ReadDataBlock(s, binaryFile, fileInfo, b.cutscenes[j]);

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

		#region Private Methods

		/// <summary>
		/// Relocates game pointers to have their address start with 0xDD to have them be easier to find in a hex editor
		/// </summary>
		private static void RelocatePointers(byte[] data, Range memoryRange) => RelocatePointers(data, memoryRange.YieldToArray());

		/// <summary>
		/// Relocates game pointers to have their address start with 0xDD (or 0xCC if not the first range)
		/// to have them be easier to find in a hex editor
		/// </summary>
		private static void RelocatePointers(byte[] data, Range[] memoryRanges)
		{
			for (int j = 0; j < data.Length; j++)
			{
				if (data[j] != 0x80)
					continue;

				int off = j - 3;
				uint ptr = BitConverter.ToUInt32(data, off);

				bool firstRange = true;

				foreach (Range range in memoryRanges)
				{
					if (ptr < range.Start || ptr >= range.End)
					{
						firstRange = false;
						continue;
					}

					ptr = ptr - range.Start + (firstRange ? 0xDD000000 : 0xCC000000); // We want to tell the ranges apart
					byte[] newData = BitConverter.GetBytes(ptr);

					for (int y = 0; y < 4; y++)
						data[off + 3 - y] = newData[y];

					break;
				}

				j += 3;
			}
		}

		#endregion

		#region Manager Methods

		protected override List<string> FindFiles(MapViewerSettings settings) => GetGameInfo(settings).maps.ToList();

		public override async UniTask<Unity_Level> LoadAsync(Context context)
		{
			// Get properties
			CPA_Settings cpaSettings = context.GetCPASettings();
			MapViewerSettings settings = context.GetMapViewerSettings();
			PS1GameInfo gameInfo = GetGameInfo(settings);
			PS1GameInfo.File fileInfo = gameInfo.files[0]; // TODO: Don't hard-code this
			PS1GameInfo.File.MemoryBlock memoryBlock = fileInfo.memoryBlocks[0]; // TODO: Don't hard-code this
			BinaryDeserializer s = context.Deserializer;

			GlobalLoadState.DetailedState = $"Loading files";
			await TimeController.WaitIfNecessary();

			// Read the packed files
			byte[][] packedFiles = await ReadPackedFilesAsync(s, s.Context.GetFile(fileInfo.BigFilePath), fileInfo, memoryBlock.main_compressed);

			// TODO: Parse files (we can skip the vignette, sounds etc.)
			// TODO: Handle memory mapping
			int index = 0;
			foreach (byte[] file in packedFiles)
			{
				var binaryFile = new StreamFile(context, $"File {index}", new MemoryStream(file), cpaSettings.GetEndian);
				context.AddFile(binaryFile);
				index++;
			}

			throw new NotImplementedException();
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

		public async UniTask<byte[]> ReadDataBlock(BinaryDeserializer s, BinaryFile file, PS1GameInfo.File fileInfo, PS1GameInfo.File.LBA lba)
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

		// TODO: Replace with System.Range once the C# version used here supports it
		private readonly struct Range
		{
			public Range(uint start, uint end)
			{
				Start = start;
				End = end;
			}

			public uint Start { get; }
			public uint End { get; }
		}

		#endregion
	}
}