using BinarySerializer.Unity;
using DDSImageParser;
using LibGC.Texture;
using System;
using System.IO;
using System.Linq;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture {
	// Xbox / Xbox 360 BTF Textures file
    public class BTF {
		uint baseOffset;
		uint unk;

		public uint Count { get; } = 0;

		public string path;

		public Texture2D[] textures = null;
		public BTFHeader[] headers = null;
		public BTFHeader360[] headers360 = null;

		public BTF(string btfPath, string bhfPath) {
			path = btfPath;
			Stream btf = FileSystem.GetFileReadStream(btfPath);
			Stream bhf = FileSystem.GetFileReadStream(bhfPath);
			using (Reader reader = new Reader(bhf, Settings.s.IsLittleEndian)) {
				Count = reader.ReadUInt32();
				textures = new Texture2D[Count];
				if (Settings.s.platform == Settings.Platform.Xbox) {
					headers = new BTFHeader[Count];
					for (uint i = 0; i < Count; i++) {
						headers[i] = new BTFHeader();
						headers[i].word_00 = reader.ReadUInt16(); // always 1
						headers[i].word_02 = reader.ReadUInt16(); // always 4
						headers[i].offset = reader.ReadUInt32();
						headers[i].dword_08 = reader.ReadUInt32(); // always 0
						headers[i].flags = reader.ReadUInt32();
						headers[i].dword_10 = reader.ReadUInt32();
					}
				} else if (Settings.s.platform == Settings.Platform.Xbox360) {
					headers360 = new BTFHeader360[Count];
					for (uint i = 0; i < Count; i++) {
						headers360[i] = new BTFHeader360();
						headers360[i].size1 = reader.ReadUInt32();
						headers360[i].offset1 = reader.ReadUInt32();
						headers360[i].size2 = reader.ReadUInt32();
						headers360[i].offset2 = reader.ReadUInt32();
						headers360[i].name = reader.ReadString(0x104);
						headers360[i].index = (int)i;
						/*MapLoader.Loader.print(
							string.Format("{0:X8}",headers360[i].size1) + " - " +
							string.Format("{0:X8}", headers360[i].offset1) + " - " +
							string.Format("{0:X8}", headers360[i].size2) + " - " +
							string.Format("{0:X8}", headers360[i].offset2) + " - " +
							headers360[i].name);*/
					}
				}
			}
			using (Reader reader = new Reader(btf, Settings.s.IsLittleEndian)) {
				if (Settings.s.platform == Settings.Platform.Xbox) {
					for (uint i = 0; i < Count; i++) {
						btf.Seek(headers[i].offset, SeekOrigin.Begin);
						uint width = (uint)(1 << (int)headers[i].WidthExponent);
						uint height = (uint)(1 << (int)headers[i].HeightExponent);
						DDSImage.PixelFormat pixelFormat = headers[i].IsDXT1 ? DDSImage.PixelFormat.DXT1 : DDSImage.PixelFormat.DXT5;
						uint size = Math.Max((headers[i].IsDXT1 ? (width * height / 2) : (width * height)), 0x80);
						byte[] data = reader.ReadBytes((int)size);
						//Debug.LogWarning(width + " - " + height + " - " + data.Length);
						using (DDSImage dds = new DDSImage(data, pixelFormat, width, height)) {
							textures[i] = dds.BitmapImage;
						}
					}
				} else if (Settings.s.platform == Settings.Platform.Xbox360) {
					for (uint i = 0; i < Count; i++) {
						//Debug.Log("parsing " + i);
						btf.Seek(headers360[i].offset1, SeekOrigin.Begin);
						headers360[i].data1 = reader.ReadBytes((int)headers360[i].size1);

						btf.Seek(headers360[i].offset2, SeekOrigin.Begin);
						headers360[i].data2 = reader.ReadBytes((int)headers360[i].size2);
					}
				}
            }
        }

		public Texture2D GetTexture(int i, uint width, uint height) {
			if (Settings.s.platform == Settings.Platform.Xbox) {
				return textures[i];
			} else if (Settings.s.platform == Settings.Platform.Xbox360) {
				//i = headers360.FirstOrDefault(t => t.name.Substring(0, t.name.LastIndexOf('.')) == name).index;
				if (textures[i] == null) {
					/*while (width * height < headers360[i].data1.Length) {
						width *= 2;
						height *= 2;
					}*/
					uint actualWidth = Math.Max(128, width);
					uint actualHeight = Math.Max(128, height);

					DDSImage.PixelFormat pixelFormat = (actualWidth * actualHeight == headers360[i].data1.Length * 2) ? DDSImage.PixelFormat.DXT1 : DDSImage.PixelFormat.DXT5;
					if (headers360[i].data1swapped == null) {
						headers360[i].CreateData1Swapped();
					}
					/*if (width * height == headers360[i].data1.Length / 4) {
						width *= 2;
						height *= 2;
						//pixelFormat = DDSImage.PixelFormat.DXT5;
						//Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "textures/" + headers360[i].name, headers360[i].data1swapped);
						//Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "textures/" + headers360[i].name + "_2", headers360[i].data2);
					}*/
					using (DDSImage dds = new DDSImage(headers360[i].data1swapped, pixelFormat, actualWidth, actualHeight)) {
						textures[i] = dds.BitmapImage;
						if (actualWidth != width || actualHeight != height) {
							Texture2D tex = new Texture2D((int)width, (int)height);
							for (int x = 0; x < width; x++) {
								for (int y = 0; y < height; y++) {
									tex.SetPixel(x, y, textures[i].GetPixel(x, y));
								}
							}
							tex.Apply();
							textures[i] = tex;
						}
					}
				}
				return textures[i];
			}
			return null;
		}

		public class BTFHeader {
			// Xbox
			public ushort word_00;
			public ushort word_02;
			public uint offset;
			public uint dword_08;
			public uint flags;
			public uint dword_10;

			public uint HeightExponent {
				get {
					return (flags & 0x0F000000) >> 24;
				}
			}
			public uint WidthExponent {
				get {
					return (flags & 0x00F00000) >> 20;
				}
			}
			public bool IsDXT1 {
				get {
					return (((flags & 0x00000F00) >> 8) == 0xC);
				}
			}
		}

		public class BTFHeader360 {
			// Xbox 360
			public uint size1;
			public uint offset1;
			public uint size2;
			public uint offset2;
			public string name;

			public byte[] data1;
			public byte[] data2;
			public byte[] data1swapped;
			public int index;

			public void CreateData1Swapped() {
				data1swapped = new byte[data1.Length];
				for (uint i = 0; i < data1.Length / 2; i++) {
					data1swapped[i * 2 + 0] = data1[i * 2 + 1];
					data1swapped[i * 2 + 1] = data1[i * 2 + 0];
				}
			}

			/*public uint Size {
				get { return offset2 - offset1; }
			}
			public uint Width {
				get { return size1 >> 8; }
			}
			public uint Height {
				get { return size2 >> 8; }
			}*/
		}
	}
}