using OpenSpace.AI;
using OpenSpace.Animation;
using OpenSpace.Collide;
using OpenSpace.Object;
using OpenSpace.FileFormat;
using OpenSpace.FileFormat.Texture;
using OpenSpace.Input;
using OpenSpace.Text;
using OpenSpace.Visual;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using OpenSpace.Object.Properties;
using System.Collections;
using System.Text.RegularExpressions;
using lzo.net;
using System.IO.Compression;
using System.Threading.Tasks;
using OpenSpace.PS1;

namespace OpenSpace.Loader {
    public class R2PS1Loader : MapLoader {
		public int CurrentLevel { get; private set; } = -1;
		public PS1VRAM vram = new PS1VRAM();

		public string[] LoadLevelList() {
			if (PS1GameInfo.Games.ContainsKey(Settings.s.mode)) {
				return PS1GameInfo.Games[Settings.s.mode].maps;
			}
			return new string[] { "<no map>" };
		}

		protected override async Task Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				await FileSystem.CheckDirectory(gameDataBinFolder);
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
				if (!PS1GameInfo.Games.ContainsKey(Settings.s.mode)) throw new Exception("PS1 info wasn't defined for this mode");
				loadingState = "Initializing files";
				PS1GameInfo game = PS1GameInfo.Games[Settings.s.mode];
				CurrentLevel = Array.IndexOf(game.maps, lvlName);
				await LoadDataFromDAT(game, game.files.FirstOrDefault(f => f.bigfile == "COMBIN"));
				//await ExtractDAT(game.files.FirstOrDefault(f => f.bigfile == "COMBIN"));
				await LoadLevel();
			} finally {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) {
                        files_array[i].Dispose();
                    }
                }
                if (cnt != null) cnt.Dispose();
            }
            await WaitIfNecessary();
            InitModdables();
        }

		public async Task InitFiles(PS1GameInfo gameInfo, PS1GameInfo.File fileInfo) {
			if (CurrentLevel < 0 || CurrentLevel >= gameInfo.maps.Length) return;
			PS1GameInfo.File.MemoryBlock b = fileInfo.memoryBlocks[CurrentLevel];
			if (!b.inEngine) return;
			string levelDir = gameDataBinFolder + lvlName + "/";
			files_array[Mem.Fix] = new PS1Data(lvlName + ".gpt", levelDir + "level.gpt", Mem.Fix, 0);
			files_array[Mem.Lvl] = new PS1Data(lvlName + ".dat", levelDir + "level.dat", Mem.Lvl, b.address);
			loadingState = "Filling VRAM";
			await WaitIfNecessary();
			FillVRAM();
		}

		public async Task LoadLevel() {
			Reader reader = files_array[Mem.Fix]?.reader;
			if (reader == null) throw new Exception("Level \"" + lvlName + "\" does not exist");

			// TODO: Load header here
			vram.Export(gameDataBinFolder + "vram.png");
			await Task.CompletedTask;
		}

		#region DAT Parsing
		public async Task LoadDataFromDAT(PS1GameInfo gameInfo, PS1GameInfo.File fileInfo) {
			if (CurrentLevel < 0 || CurrentLevel >= gameInfo.maps.Length) return;
			PS1GameInfo.File.MemoryBlock b = fileInfo.memoryBlocks[CurrentLevel];
			if (!b.inEngine) return;
			string bigFile = fileInfo.bigfile;
			string bigFilePath = gameDataBinFolder + bigFile + "." + fileInfo.extension;
			string levelDir = gameDataBinFolder + lvlName + "/";
			await PrepareBigFile(bigFilePath, 2048);
			loadingState = "Extracting data from bigfile(s)";
			await WaitIfNecessary();
			if (FileSystem.FileExists(bigFilePath)) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(bigFilePath), isLittleEndian: Settings.s.IsLittleEndian)) {
					List<byte[]> mainBlock = await ExtractPackedBlocks(reader, b.compressed, fileInfo.baseLBA);
					int blockIndex = 0;
					FileSystem.AddVirtualFile(levelDir + "vignette.tim", mainBlock[blockIndex++]);
					if (b.inEngine) {
						if (b.isPlayable) {
							FileSystem.AddVirtualFile(levelDir + "unk_playable.blk", mainBlock[blockIndex++]);
						}
						FileSystem.AddVirtualFile(levelDir + "vram.bin", mainBlock[blockIndex++]);
						FileSystem.AddVirtualFile(levelDir + "level.gpt", mainBlock[blockIndex++]);
					}
					// skip exe
					byte[] exe = mainBlock[blockIndex++];
					byte[] exeData = mainBlock[blockIndex++];
					/*byte[] newData = new byte[exeHeader.Length + exeData.Length];*/
					Util.AppendArrayAndMergeReferences(ref exe, ref exeData);
					FileSystem.AddVirtualFile(levelDir + "executable.pxe", exe);
					if (b.inEngine) {
						FileSystem.AddVirtualFile(levelDir + "level.dat", mainBlock[blockIndex++]);
					}
					if (blockIndex != mainBlock.Count) {
						Debug.LogWarning("Not all blocks were extracted!");
					}
				}
			}
			await InitFiles(gameInfo, fileInfo);
		}

		public async Task ExtractDAT(PS1GameInfo.File fileInfo) {
			string bigFile = fileInfo.bigfile;
			byte[] data = new byte[0];
			using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + bigFile + "." + fileInfo.extension))) {
				PS1GameInfo.File.MemoryBlock[] memoryBlocks = fileInfo.memoryBlocks;
				for (int i = 0; i < memoryBlocks.Length; i++) {
					PS1GameInfo.File.MemoryBlock b = memoryBlocks[i];
					List<byte[]> mainBlock = await ExtractPackedBlocks(reader, b.compressed, fileInfo.baseLBA);
					int blockIndex = 0;
					Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_LoadImage.TIM", mainBlock[blockIndex++]);
					if (b.inEngine) {
						if (b.isPlayable) {
							Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_UNK_Playable.BLK", mainBlock[blockIndex++]);
						}
						Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_Textures.VRAM", mainBlock[blockIndex++]);
						Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_Level_Header.GPT", mainBlock[blockIndex++]);
					}
					byte[] exe = mainBlock[blockIndex++];
					byte[] exeData = mainBlock[blockIndex++];
					/*byte[] newData = new byte[exeHeader.Length + exeData.Length];*/
					Util.AppendArrayAndMergeReferences(ref exe, ref exeData);
					Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_Executable.PXE", exe);
					if (b.inEngine) {
						Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_Level.DAT", mainBlock[blockIndex++]);
					}
					if (blockIndex != mainBlock.Count) {
						Debug.LogWarning("Not all blocks were exported!");
					}


					Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_UNK_Anims.BLK", ExtractBlock(reader, b.filetable, fileInfo.baseLBA));
					Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_UNK_Uncompressed.BLK", ExtractBlock(reader, b.uncompressed, fileInfo.baseLBA));
					for (int j = 0; j < b.cutscenes.Length; j++) {
						string cutsceneAudioName = gameDataBinFolder + "ext/" + bigFile + "_" + i + "_CutsceneAudio_" + j + ".BLK";
						byte[] cutsceneAudioBlk = ExtractBlock(reader, b.cutscenes[j], fileInfo.baseLBA);
						if (cutsceneAudioBlk != null) {
							Util.ByteArrayToFile(cutsceneAudioName, DecompressCutsceneAudio(cutsceneAudioBlk));
						}
					}
					//ParseMainBlock(mainBlock, b, i, gameDataBinFolder + "ext/" + bigFile + "_" + i + "_main");
					await WaitIfNecessary();
				}
			}
		}

		public void ParseMainBlock(byte[] data, PS1GameInfo.File.MemoryBlock block, int index, string basename) {
			using (MemoryStream ms = new MemoryStream(data)) {
				using (Reader reader = new Reader(ms, Settings.s.IsLittleEndian)) {
					reader.ReadUInt32();
					reader.ReadUInt32();
					uint loadingImgSize = reader.ReadUInt32();
					reader.ReadUInt32();
					uint sz2 = reader.ReadUInt32();
					byte[] loadingImg = reader.ReadBytes((int)loadingImgSize - 14);
					Util.ByteArrayToFile(basename + "_loading.img", loadingImg);
					reader.ReadUInt32();
					if (index < 57) {
						if (block.isPlayable) {
							uint i = 0;
							int size = reader.ReadInt32();
							while (size != 0) {
								byte[] playableData = reader.ReadBytes(size * 4 * 2);
								Util.ByteArrayToFile(basename + "_playableData_" + i + ".img", playableData);
								print(string.Format("{0:X8}",reader.BaseStream.Position));
								size = reader.ReadInt32();
								i++;
							}
							//byte[] some
						}
						byte[] textureMem = reader.ReadBytes(0xB0000);
						Util.ByteArrayToFile(basename + "_textures.img", textureMem);
						byte[] preExeData = reader.ReadBytes(0x200);
						Util.ByteArrayToFile(basename + "_preExeData.blk", preExeData);
					}
					byte[] exe = reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position);
					Util.ByteArrayToFile(basename + ".pxe", exe);
				}
			}
		}

		public byte[] DecompressCutsceneAudio(byte[] cutsceneData) {
			List<byte[]> bytes = new List<byte[]>();
			using (MemoryStream ms = new MemoryStream(cutsceneData)) {
				using (Reader reader = new Reader(ms, Settings.s.IsLittleEndian)) {
					uint hdrSize = 1;
					while (reader.BaseStream.Position < reader.BaseStream.Length && hdrSize > 0) {
						hdrSize = reader.ReadUInt32();
						//print("HDR " + string.Format("{0:X8}", hdrSize));
						if (hdrSize != 0xFFFFFFFF) {
							reader.ReadBytes((int)hdrSize);
							bool readParts = true;
							while (readParts && reader.BaseStream.Position < reader.BaseStream.Length) {
								uint size = reader.ReadUInt32();
								//print("SIZE " + string.Format("{0:X8}", size));
								if (size == 0xFFFFFFFE) {
									readParts = false;
									if (reader.BaseStream.Position % 0x800 > 0) {
										reader.BaseStream.Position = 0x800 * ((reader.BaseStream.Position / 0x800) + 1);
									}
								} else {
									bool isNull = (size & 0x80000000) != 0;
									size = size & 0x7FFFFFFF;
									if (isNull) {
										bytes.Add(Enumerable.Repeat((byte)0x0, (int)size).ToArray());
									} else {
										bytes.Add(reader.ReadBytes((int)size));
									}
								}
							}
						}
					}
				}
			}
			return bytes.SelectMany(i => i).ToArray();
		}

		public byte[] ExtractBlock(Reader reader, PS1GameInfo.File.LBA lba, uint baseLBA) {
			byte[] data;
			if (lba.lba < baseLBA || lba.size <= 0) return null;
			reader.BaseStream.Seek((lba.lba - baseLBA) * 0x800, SeekOrigin.Begin);
			
			data = reader.ReadBytes((int)lba.size);
			return data;
		}
		public async Task<List<byte[]>> ExtractPackedBlocks(Reader reader, PS1GameInfo.File.LBA lba, uint baseLBA) {
			PartialHttpStream httpStream = reader.BaseStream as PartialHttpStream;
			List<byte[]> datas = new List<byte[]>();
			byte[] data;
			if (lba.lba < baseLBA || lba.size <= 0) return null;
			reader.BaseStream.Seek((lba.lba - baseLBA) * 0x800, SeekOrigin.Begin);

			data = new byte[0];
			uint end = (lba.lba + lba.size - baseLBA) * 0x800;
			bool previousWasZero = false;
			bool previousWasFF = false;
			while (reader.BaseStream.Position < end) {
				if (httpStream != null) await httpStream.FillCacheForRead(0x1004);
				uint decompressedSize = reader.ReadUInt32(); // 0x8000
				if (previousWasFF) {
					if (decompressedSize == 0xFFFFFFFF && reader.ReadUInt32() == 0) {
						reader.Align(0x800);
						previousWasFF = false;
					} else {
						reader.BaseStream.Position = 0x800 * (reader.BaseStream.Position / 0x800);
						byte[] uncompressedData = reader.ReadBytes(0x800);
						if (uncompressedData != null) {
							int originalDataLength = data.Length;
							Array.Resize(ref data, originalDataLength + uncompressedData.Length);
							Array.Copy(uncompressedData, 0, data, originalDataLength, uncompressedData.Length);
						}
					}
				} else {
					if (decompressedSize == 0) {
						if (previousWasZero) {
							reader.Align(0x800);
							previousWasZero = false;
							break;
						} else {
							previousWasZero = true;
						}
						previousWasFF = false;
						// If previous was zero, then padding to 0x800. If previous was not zero, then new file.
						//print(decompressedSize + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
						datas.Add(data);
						data = new byte[0];
						continue;
					} else if (decompressedSize == 0xFFFFFFFF) {
						if (previousWasZero) {
							reader.Align(0x800);
							previousWasFF = true;
						}
						previousWasZero = false;
						continue;
					} else {
						previousWasZero = false;
						previousWasFF = false;
					}
					uint compressedSize = reader.ReadUInt32();
					if (httpStream != null) await httpStream.FillCacheForRead((int)compressedSize);
					//print(decompressedSize + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
					byte[] uncompressedData = null;
					byte[] compressedData = reader.ReadBytes((int)compressedSize);
					using (var compressedStream = new MemoryStream(compressedData))
					using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
					using (Reader lzoReader = new Reader(lzo, Settings.s.IsLittleEndian)) {
						lzo.SetLength(decompressedSize);
						uncompressedData = lzoReader.ReadBytes((int)decompressedSize);
					}
					if (uncompressedData != null) {
						int originalDataLength = data.Length;
						Array.Resize(ref data, originalDataLength + uncompressedData.Length);
						Array.Copy(uncompressedData, 0, data, originalDataLength, uncompressedData.Length);
					}
				}
			}
			if (data.Length > 0) {
				datas.Add(data);
			}
			return datas;
		}
		#endregion

		public void FillVRAM() {
			vram.currentXPage = 5;
			using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + lvlName + "/vram.bin"))) {
				byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
				int width = Mathf.CeilToInt(data.Length / (float)(PS1VRAM.page_height * 2));
				vram.AddData(data, width);
			}
		}
	}
}
