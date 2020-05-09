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
using OpenSpace.PS1.GLI;
using System.Text;

namespace OpenSpace.Loader {
    public class R2PS1Loader : MapLoader {
		public int CurrentLevel { get; private set; } = -1;
		public PS1VRAM vram = new PS1VRAM();
		public LevelHeader levelHeader;
		public PS1GameInfo game;
		public ushort maxScaleVector = 0;
		public PS1Stream[] streams;

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
				//RipRHRLoc();
				game = PS1GameInfo.Games[Settings.s.mode];
				CurrentLevel = Array.IndexOf(game.maps, lvlName);
				await LoadDataFromDAT(game, game.files.FirstOrDefault(f => f.bigfile == "COMBIN"));
				//await ExtractDAT(game, game.files.FirstOrDefault(f => f.bigfile == "COMBIN"), relocatePointers: true);
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

		/*public void RipRHRLoc() {
			string offsetsFile = gameDataBinFolder + "en_string_offsets.bin";
			string locFile = gameDataBinFolder + "rhr_loc_from_memory.bin";
			List<ushort> offsets = new List<ushort>();
			string[] strings;
			using (Reader reader = new Reader(FileSystem.GetFileReadStream(offsetsFile))) {
				while (reader.BaseStream.Position < reader.BaseStream.Length) {
					offsets.Add(reader.ReadUInt16());
				}
			}
			strings = new string[offsets.Count];
			using (Reader reader = new Reader(FileSystem.GetFileReadStream(locFile))) {
				for (int i = 0; i < strings.Length; i++) {
					reader.BaseStream.Position = offsets[i];
					strings[i] = reader.ReadNullDelimitedString(encoding: Encoding.GetEncoding(1252));
				}
			}
			var output = strings;
			string json = Newtonsoft.Json.JsonConvert.SerializeObject(output, Newtonsoft.Json.Formatting.Indented);
			Util.ByteArrayToFile(gameDataBinFolder + "rhr.json", Encoding.UTF8.GetBytes(json));
		}*/

		public async Task InitFiles(PS1GameInfo gameInfo, PS1GameInfo.File fileInfo) {
			if (CurrentLevel < 0 || CurrentLevel >= gameInfo.maps.Length) return;
			PS1GameInfo.File.MemoryBlock b = fileInfo.memoryBlocks[CurrentLevel];
			if (!b.inEngine) return;
			string levelDir = gameDataBinFolder + lvlName + "/";
			Array.Resize(ref files_array, 2 + b.cutscenes.Length + (b.cinedata.size > 0 ? 1 : 0));
			int curFile = 0;
			files_array[curFile++] = new PS1Data(lvlName + ".gpt", levelDir + "level.gpt", curFile, 0);
			files_array[curFile++] = new PS1Data(lvlName + ".dat", levelDir + "level.dat", curFile, b.address);
			for (int i = 0; i < b.cutscenes.Length; i++) {
				string cutsceneFramesName = levelDir + "stream_frames_" + i + ".blk";
				files_array[curFile++] = new LinearFile("stream_frames_" + i +".blk", cutsceneFramesName, curFile);
			}
			if (b.cinedata.size > 0) {
				curFile++;
				/*files_array[curFile++] = new PS1Data("cine.dat", levelDir + "cine.dat",
							cineDataBaseAddress + 0x1f800 + 0x32 * 0xc00,
							curFile);*/
			}
			loadingState = "Filling VRAM";
			await WaitIfNecessary();
			FillVRAM();
		}

		public async Task LoadLevel() {
			Reader reader = files_array[Mem.Fix]?.reader;
			if (reader == null) throw new Exception("Level \"" + lvlName + "\" does not exist");

			// TODO: Load header here
			vram.Export(gameDataBinFolder + "vram.png");

			loadingState = "Loading level header";
			await WaitIfNecessary();
			levelHeader = FromOffsetOrRead<LevelHeader>(reader, Pointer.Current(reader));

			loadingState = "Calculating texture bounds";
			await WaitIfNecessary();
			CalculateTextures();

			loadingState = "Loading superobject hierarchy";
			await WaitIfNecessary();
			levelHeader.fatherSector = FromOffsetOrRead<PS1.SuperObject>(reader, levelHeader.off_fatherSector, onPreRead: s => s.isDynamic = false);
			levelHeader.dynamicWorld = FromOffsetOrRead<PS1.SuperObject>(reader, levelHeader.off_dynamicWorld, onPreRead: s => s.isDynamic = true);
			levelHeader.inactiveDynamicWorld = FromOffsetOrRead<PS1.SuperObject>(reader, levelHeader.off_inactiveDynamicWorld, onPreRead: s => s.isDynamic = true);

			loadingState = "Creating gameobjects";
			await WaitIfNecessary();
			GameObject fatherSector = levelHeader.fatherSector?.GetGameObject();
			fatherSector.name = "Father Sector | " + fatherSector.name;
			GameObject dynamicWorld = levelHeader.dynamicWorld?.GetGameObject();
			dynamicWorld.name = "Dynamic World | " + dynamicWorld.name;
			GameObject inactiveDynamicWorld = levelHeader.inactiveDynamicWorld?.GetGameObject();
			inactiveDynamicWorld.name = "Inactive Dynamic World | " + inactiveDynamicWorld.name;
			GameObject always = new GameObject("Always");
			foreach (AlwaysList alw in levelHeader.always) {
				GameObject alwGao = alw.GetGameObject();
				alwGao.transform.SetParent(always.transform);
			}

			GameObject persoPartsParent = new GameObject("Perso parts");
			int i = 0;
			foreach (ObjectsTable.Entry e in levelHeader.geometricObjectsDynamic.entries) {
				GameObject g = e.GetGameObject();
				g.name = $"[{i}] {e.off_0} - {g.name}";
				g.transform.parent = persoPartsParent.transform;
				g.transform.position = new Vector3(i++ * 4, 1000, 0);
			}


			PS1GameInfo.File fileInfo = game.files.FirstOrDefault(f => f.bigfile == "COMBIN");
			PS1GameInfo.File.MemoryBlock b = fileInfo.memoryBlocks[CurrentLevel];
			streams = new PS1Stream[b.cutscenes.Length];
			for (int j = 0; j < b.cutscenes.Length; j++) {
				loadingState = $"Loading cinematic streams : {j+1}/{b.cutscenes.Length}";
				await WaitIfNecessary();
				reader = files_array[2 + j].reader;
				streams[j] = FromOffsetOrRead<PS1Stream>(reader, Pointer.Current(reader), inline: true);
			}
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
					byte[] cineblock = ExtractBlock(reader, b.cinedata, fileInfo.baseLBA);
					if (cineblock != null) {
						FileSystem.AddVirtualFile(levelDir + "cine.dat", cineblock);
					}

					for (int j = 0; j < b.cutscenes.Length; j++) {
						string cutsceneAudioName = levelDir + "stream_audio_" + j + ".blk";
						string cutsceneFramesName = levelDir + "stream_frames_" + j + ".blk";
						byte[] cutsceneAudioBlk = ExtractBlock(reader, b.cutscenes[j], fileInfo.baseLBA);
						if (cutsceneAudioBlk != null) {
							byte[] cutsceneAudio;
							byte[] cutsceneFrames;
							SplitCutsceneStream(cutsceneAudioBlk, out cutsceneAudio, out cutsceneFrames);
							FileSystem.AddVirtualFile(cutsceneFramesName, cutsceneFrames);
						}
					}
				}
			}
			await InitFiles(gameInfo, fileInfo);
		}

		public async Task ExtractDAT(PS1GameInfo gameInfo, PS1GameInfo.File fileInfo, bool relocatePointers = false) {
			string bigFile = fileInfo.bigfile;
			string bigFilePath = gameDataBinFolder + bigFile + "." + fileInfo.extension;
			uint cineDataBaseAddress = 0;
			if (FileSystem.FileExists(bigFilePath)) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(bigFilePath))) {
					PS1GameInfo.File.MemoryBlock[] memoryBlocks = fileInfo.memoryBlocks;
					for (int i = 0; i < memoryBlocks.Length; i++) {
						int gptIndex = 0, lvlIndex = 0;
						PS1GameInfo.File.MemoryBlock b = memoryBlocks[i];
						string levelDir = gameDataBinFolder + "ext/" + (i < gameInfo.maps.Length ? gameInfo.maps[i] : (bigFile + "_" + i)) + "/";
						List<byte[]> mainBlock = await ExtractPackedBlocks(reader, b.compressed, fileInfo.baseLBA);
						int blockIndex = 0;
						Util.ByteArrayToFile(levelDir + "vignette.tim", mainBlock[blockIndex++]);
						if (b.inEngine) {
							if (b.isPlayable) {
								Util.ByteArrayToFile(levelDir + "unk_playable.blk", mainBlock[blockIndex++]);
							}
							Util.ByteArrayToFile(levelDir + "vram.bin", mainBlock[blockIndex++]);
							Util.ByteArrayToFile(levelDir + "level.gpt", mainBlock[blockIndex++]);
							if (relocatePointers) {
								gptIndex = blockIndex - 1;
							}
						}
						byte[] exe = mainBlock[blockIndex++];
						byte[] exeData = mainBlock[blockIndex++];
						/*byte[] newData = new byte[exeHeader.Length + exeData.Length];*/
						Util.AppendArrayAndMergeReferences(ref exe, ref exeData);
						Util.ByteArrayToFile(levelDir + "executable.pxe", exe);
						if (b.inEngine) {
							Util.ByteArrayToFile(levelDir + "level.dat", mainBlock[blockIndex++]);
							if (relocatePointers) {
								lvlIndex = blockIndex - 1;
								uint length = (uint)mainBlock[lvlIndex].Length;
								byte[] data = mainBlock[gptIndex];
								for (int j = 0; j < data.Length; j++) {
									if (data[j] == 0x80) {
										int off = j - 3;
										uint ptr = BitConverter.ToUInt32(data, off);
										if (ptr >= b.address && ptr < b.address + length) {
											if (off == 0x14c) {
												cineDataBaseAddress = ptr;
											}
											ptr = (ptr - b.address) + 0xDD000000;
											byte[] newData = BitConverter.GetBytes(ptr);
											for (int y = 0; y < 4; y++) {
												data[off + 3 - y] = newData[y];
											}
											j += 3;
										}
									}
								}
								Util.ByteArrayToFile(levelDir + "level_relocated.gpt", data);
								data = mainBlock[lvlIndex];
								for (int j = 0; j < data.Length; j++) {
									if (data[j] == 0x80) {
										int off = j - 3;
										uint ptr = BitConverter.ToUInt32(data, off);
										if (ptr >= b.address && ptr < b.address + length) {
											ptr = (ptr - b.address) + 0xDD000000;
											byte[] newData = BitConverter.GetBytes(ptr);
											for (int y = 0; y < 4; y++) {
												data[off + 3 - y] = newData[y];
											}
											j += 3;
										}
									}
								}
								Util.ByteArrayToFile(levelDir + "level_relocated.dat", data);
							}
						}
						if (blockIndex != mainBlock.Count) {
							Debug.LogWarning("Not all blocks were exported!");
						}


						Util.ByteArrayToFile(levelDir + "unk_anims.blk", ExtractBlock(reader, b.filetable, fileInfo.baseLBA));
						byte[] cineblock = ExtractBlock(reader, b.cinedata, fileInfo.baseLBA);
						Util.ByteArrayToFile(levelDir + "cine.dat", cineblock);
						if(cineblock != null) {
							byte[] data = cineblock;
							cineDataBaseAddress += 0x1f800 + 0x32 * 0xc00; // magic!
							for (int j = 0; j < data.Length; j++) {
								if (data[j] == 0x80) {
									int off = j - 3;
									uint ptr = BitConverter.ToUInt32(data, off);
									if (ptr >= b.address && ptr < cineDataBaseAddress) {
										ptr = (ptr - b.address) + 0xDD000000;
										byte[] newData = BitConverter.GetBytes(ptr);
										for (int y = 0; y < 4; y++) {
											data[off + 3 - y] = newData[y];
										}
										j += 3;
									}
									if (ptr >= cineDataBaseAddress && ptr < cineDataBaseAddress + data.Length) {
										ptr = (ptr - cineDataBaseAddress) + 0xCC000000;
										byte[] newData = BitConverter.GetBytes(ptr);
										for (int y = 0; y < 4; y++) {
											data[off + 3 - y] = newData[y];
										}
										j += 3;
									}
								}
							}
							Util.ByteArrayToFile(levelDir + "cine_relocated.dat", data);
						}
						for (int j = 0; j < b.cutscenes.Length; j++) {
							string cutsceneAudioName = levelDir + "stream_audio_" + j + ".blk";
							string cutsceneFramesName = levelDir + "stream_frames_" + j + ".blk";
							byte[] cutsceneAudioBlk = ExtractBlock(reader, b.cutscenes[j], fileInfo.baseLBA);
							if (cutsceneAudioBlk != null) {
								//Util.ByteArrayToFile(levelDir + "stream_full_" + j + ".blk", cutsceneAudioBlk);
								byte[] cutsceneAudio;
								byte[] cutsceneFrames;
								SplitCutsceneStream(cutsceneAudioBlk, out cutsceneAudio, out cutsceneFrames);
								Util.ByteArrayToFile(cutsceneAudioName, cutsceneAudio);
								Util.ByteArrayToFile(cutsceneFramesName, cutsceneFrames);
							}
						}
						//ParseMainBlock(mainBlock, b, i, gameDataBinFolder + "ext/" + bigFile + "_" + i + "_main");
						await WaitIfNecessary();
					}
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

		public void SplitCutsceneStream(byte[] cutsceneData, out byte[] cutsceneAudio, out byte[] cutsceneFrames) {
			List<byte[]> cutsceneAudioList = new List<byte[]>();
			List<byte[]> cutsceneFramesList = new List<byte[]>();
			using (MemoryStream ms = new MemoryStream(cutsceneData)) {
				using (Reader reader = new Reader(ms, Settings.s.IsLittleEndian)) {
					uint hdrSize = 1;
					while (reader.BaseStream.Position < reader.BaseStream.Length && hdrSize > 0) {
						hdrSize = reader.ReadUInt32();
						//print("HDR " + string.Format("{0:X8}", hdrSize));
						if (hdrSize != 0xFFFFFFFF) {
							cutsceneFramesList.Add(reader.ReadBytes((int)hdrSize));
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
										cutsceneAudioList.Add(Enumerable.Repeat((byte)0x0, (int)size).ToArray());
									} else {
										cutsceneAudioList.Add(reader.ReadBytes((int)size));
									}
								}
							}
						}
					}
				}
			}
			cutsceneAudio = cutsceneAudioList.SelectMany(i => i).ToArray();
			cutsceneFrames = cutsceneFramesList.SelectMany(i => i).ToArray();
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

		#region Textures
		public void FillVRAM() {
			vram.currentXPage = 5;
			using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + lvlName + "/vram.bin"))) {
				byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
				int width = Mathf.CeilToInt(data.Length / (float)(PS1VRAM.page_height * 2));
				vram.AddData(data, width);
			}
		}

		private List<TextureBounds> textureBounds = new List<TextureBounds>();

		public void RegisterTexture(ushort pageInfo, ushort palette, int xMin, int xMax, int yMin, int yMax) {
			TextureBounds b = new TextureBounds() {
				pageInfo = pageInfo,
				paletteInfo = palette,
				xMin = xMin,
				xMax = xMax,
				yMin = yMin,
				yMax = yMax
			};

            bool newTexture = true;
            foreach(TextureBounds u in textureBounds) {
                if (u.HasOverlap(b)) {
                    u.ExpandWithBounds(b);
                    newTexture = false;
                    break;
                }
            }

            if (newTexture) {
                textureBounds.Add(b);
            }
		}

        public void CalculateTextures() {
            int i = 0;
            foreach (TextureBounds b in textureBounds) {
                int w = b.xMax - b.xMin;
                int h = b.yMax - b.yMin;
                Texture2D tex = vram.GetTexture((ushort)w, (ushort)h, b.pageInfo, b.paletteInfo, b.xMin, b.yMin);
				tex.wrapMode = TextureWrapMode.Clamp;
                b.texture = tex;
                if (exportTextures) {
                    Util.ByteArrayToFile(gameDataBinFolder + "test_tex/" + lvlName + "/" + i++ + $"_{b.xMin}_{b.yMin}_{w}_{h}" + ".png", tex.EncodeToPNG());
                }
            }
        }

        public TextureBounds GetTextureBounds(ushort pageInfo, ushort paletteInfo, int x, int y) {
			return textureBounds.FirstOrDefault(
				t => t.pageInfo == pageInfo && t.paletteInfo == paletteInfo &&
				x >= t.xMin && x < t.xMax &&
				y >= t.yMin && y < t.yMax);
		}
		#endregion
	}
}
