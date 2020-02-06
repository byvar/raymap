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

namespace OpenSpace.Loader {
    public class R2PS1Loader : MapLoader {
		protected override async Task Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				string bigFile = "COMBIN";
				await FileSystem.CheckDirectory(gameDataBinFolder);
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
                loadingState = "Initializing files";
				byte[] data = new byte[0];
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + bigFile + ".DAT"))) {
					List<MemoryBlock> memoryBlocks = new List<MemoryBlock>();
					foreach (string line in File.ReadLines(gameDataBinFolder + bigFile + ".txt")) {
						string[] blockStr = line.Split('\t');
						MemoryBlock b = new MemoryBlock(Convert.ToUInt32(blockStr[0], 16), int.Parse(blockStr[1]) == 1,
							new LBA(Convert.ToUInt32(blockStr[2], 16), Convert.ToUInt32(blockStr[3], 16)),
							new LBA(Convert.ToUInt32(blockStr[4], 16), Convert.ToUInt32(blockStr[5], 16)),
							new LBA(Convert.ToUInt32(blockStr[6], 16), Convert.ToUInt32(blockStr[7], 16)));
						for (int i = 8; i < blockStr.Length; i+=2) {
							b.cutscenes.Add(new LBA(Convert.ToUInt32(blockStr[i], 16), Convert.ToUInt32(blockStr[i + 1], 16)));
						}
						memoryBlocks.Add(b);
					}
					await WaitIfNecessary();
					for(int i = 0; i < memoryBlocks.Count; i++) {
						MemoryBlock b = memoryBlocks[i];
						List<byte[]> mainBlock = ExtractPackedBlocks(reader, b.compressed, 0x1f4);
						for (int j = 0; j < mainBlock.Count; j++) {
							if (j == 0) {
								Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_main_loading.tim", mainBlock[j]);
							} else if(mainBlock[j].Length == 0xB0000) {
								Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_main_textures.bin", mainBlock[j]);
							} else {
								Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_main_" + j + ".blk", mainBlock[j]);
							}
						}
						Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_fat_and_anims.blk", ExtractBlock(reader, b.filetable, 0x1f4));
						Util.ByteArrayToFile(gameDataBinFolder + "ext/" + bigFile + "_" + i + "_uncompr.blk", ExtractBlock(reader, b.uncompressed, 0x1f4));
						for (int j = 0; j < b.cutscenes.Count; j++) {
							string cutsceneAudioName = gameDataBinFolder + "ext/" + bigFile + "_" + i + "_cutsceneAudio_" + j + ".blk";
							byte[] cutsceneAudioBlk = ExtractBlock(reader, b.cutscenes[j], 0x1f4);
							if (cutsceneAudioBlk != null) {
								Util.ByteArrayToFile(cutsceneAudioName, DecompressCutsceneAudio(cutsceneAudioBlk));
							}
						}
						//ParseMainBlock(mainBlock, b, i, gameDataBinFolder + "ext/" + bigFile + "_" + i + "_main");
						await WaitIfNecessary();
					}
				}
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

		public void ParseMainBlock(byte[] data, MemoryBlock block, int index, string basename) {
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
						if (block.isSomething) {
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

		public byte[] ExtractBlock(Reader reader, LBA lba, uint baseOffset) {
			byte[] data;
			if (lba.lba < baseOffset || lba.size <= 0) return null;
			reader.BaseStream.Seek((lba.lba - baseOffset) * 0x800, SeekOrigin.Begin);
			
			data = reader.ReadBytes((int)lba.size);
			return data;
		}
		public List<byte[]> ExtractPackedBlocks(Reader reader, LBA lba, uint baseOffset) {
			List<byte[]> datas = new List<byte[]>();
			byte[] data;
			if (lba.lba < baseOffset || lba.size <= 0) return null;
			reader.BaseStream.Seek((lba.lba - baseOffset) * 0x800, SeekOrigin.Begin);

			data = new byte[0];
			uint end = (lba.lba + lba.size - baseOffset) * 0x800;
			bool previousWasZero = false;
			bool previousWasFF = false;
			while (reader.BaseStream.Position < end) {
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
						print(decompressedSize + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
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
					print(decompressedSize + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
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
	}

	public class LBA {
		public uint lba;
		public uint size;

		public LBA(uint lba, uint size) {
			this.lba = lba;
			this.size = size;
		}
	}

	public class MemoryBlock {
		public uint address;
		public bool isSomething;
		public LBA compressed;
		public LBA filetable;
		public LBA uncompressed;
		public List<LBA> cutscenes;

		public MemoryBlock(uint address, bool isSomething, LBA compressed, LBA filetable, LBA uncompressed) {
			this.address = address;
			this.isSomething = isSomething;
			this.compressed = compressed;
			this.filetable = filetable;
			this.uncompressed = uncompressed;
			cutscenes = new List<LBA>();
		}
	}
}
