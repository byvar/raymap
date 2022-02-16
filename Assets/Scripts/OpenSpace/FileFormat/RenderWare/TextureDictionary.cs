using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.RenderWare {
	// R2 PS2 texture format
	// https://gtamods.com/wiki/Texture_Native_Struct
	public class TextureDictionary {
		public Section root;

		public uint Count { get; } = 0;

		public Texture2D[] textures = null;
		public TextureDictionary(string path) {
			Stream fs = FileSystem.GetFileReadStream(path);
			using (Reader reader = new Reader(fs, Legacy_Settings.s.IsLittleEndian)) {
				root = Section.Read(reader);
			}
			if (root != null && root.type == Section.Type.TextureDictionary) {
				ushort numTextures = (ushort)root.children[0].variables["numTextures"];
				Count = numTextures;
				textures = new Texture2D[numTextures];
				for (int i = 0; i < numTextures; i++) {
					Section texNative = root.children[i + 1];
					Section texNativeHeader = texNative.children[0];
					Section texNativeInfo = texNative.children[3].children[0];
					byte[] texNativeData = texNative.children[3].children[1].data;
					string name = (string)texNative.children[1]["string"];
					uint width = (uint)texNativeInfo.variables["width"];
					uint height = (uint)texNativeInfo.variables["height"];
					uint bpp = (uint)texNativeInfo.variables["bpp"];
					uint rasterFormat = (uint)texNativeInfo.variables["rasterFormat"];
					uint textureDataSize = (uint)texNativeInfo.variables["textureDataSize"];
					uint paletteDataSize = (uint)texNativeInfo.variables["paletteDataSize"];
					byte filterMode = (byte)texNativeHeader.variables["filterMode"];
					byte addressingMode = (byte)texNativeHeader.variables["addressingMode"];
					textures[i] = ParseTexture(texNativeData, width, height, bpp, textureDataSize, paletteDataSize, (FilterMode)filterMode, (AddressingMode)addressingMode, (RasterFormat)rasterFormat);
					if (MapLoader.Loader.exportTextures) {
						//MapLoader.Loader.print(MapLoader.Loader.gameDataBinFolder + "textures/" + Path.GetFileNameWithoutExtension(path) + "/" + name + ".png");
						Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "textures/" + Path.GetFileNameWithoutExtension(path) + "/" + name + ".png", textures[i].EncodeToPNG());
					}
				}
			}
		}

		/*public Texture2D Lookup(byte[] name, Type type) {
			if (root != null && root.type == Section.Type.TextureDictionary) {
				for (int i = 0; i < Count; i++) {
					Section texNative = root.children[i + 1];
					byte[] texName = type == Type.Texture ? texNative.children[1].data : texNative.children[2].data;
					if (name.SequenceEqual(texName)) {
						return textures[i];
					}
				}
			}
			return null;
		}

		public Texture2D Lookup(string name) {
			int addLength = name.Length - (name.Length/4)*4;
			byte[] bytes = new byte[name.Length + addLength];
			for (int i = 0; i < name.Length; i++) {
				bytes[i] = Convert.ToByte(name[i]);
			}
			for (int i = name.Length; i < name.Length + addLength; i++) {
				bytes[i] = 0x0;
			}
			return Lookup(bytes, Type.Texture);
		}*/

		public Texture2D Lookup(string name, Type type = Type.Texture) {
			if (root != null && root.type == Section.Type.TextureDictionary) {
				for (int i = 0; i < Count; i++) {
					Section texNative = root.children[i + 1];
					string texName = type == Type.Texture ? (string)texNative.children[1]["string"] : (string)texNative.children[2]["string"];
					if (name == texName) {
						return textures[i];
					}
				}
			}
			return null;
		}

		private Texture2D ParseTexture(byte[] data, uint width, uint height, uint bpp, uint textureDataSize, uint paletteDataSize, FilterMode filterMode, AddressingMode addressingMode, RasterFormat rasterFormat) {
			Texture2D tex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
			Color[] pixels = new Color[width * height];
			if (!rasterFormat.HasFlag(RasterFormat.FORMAT_EXT_PAL8)) {
				// No palette
				if (rasterFormat.HasFlag(RasterFormat.FORMAT_8888)) {
					// Just RGBA 32bit
					for (int i = 0; i < data.Length / 4; i++) {
						pixels[i] = new Color(data[i * 4] / 255f, data[i * 4 + 1] / 255f, data[i * 4 + 2] / 255f, data[i * 4 + 3] / 255f);
					}
				} else if(rasterFormat.HasFlag(RasterFormat.FORMAT_1555)) {
					// Just RGBA 5551
					for (int i = 0; i < data.Length / 2; i++) {
						ushort pixel = Util.ToUInt16(new byte[] { data[i * 2], data[i * 2 + 1] }, 0, Legacy_Settings.s.IsLittleEndian);
						uint alpha = extractBits(pixel, 1, 15);
						uint blue = extractBits(pixel, 5, 10);
						uint green = extractBits(pixel, 5, 5);
						uint red = extractBits(pixel, 5, 0);
						
						pixels[i] = new Color(red / 31.0f, green / 31.0f, blue / 31.0f, alpha);
					}
				} else {
					MapLoader.Loader.print("(RenderWare) Unknown Texture Format: " + rasterFormat);
				}
			} else {
				// Paletted PAL8 mode (2^8 = 256 palette colors)
				Color[] palette = new Color[256];
				if (rasterFormat.HasFlag(RasterFormat.FORMAT_8888)) {
					// Palette's colors are RGBA 32bit
					for (int i = 0; i < 256; i++) {
						palette[i] = new Color(
							data[textureDataSize + i * 4 + 0] / 255f,
							data[textureDataSize + i * 4 + 1] / 255f,
							data[textureDataSize + i * 4 + 2] / 255f,
							data[textureDataSize + i * 4 + 3] / 128f);
					}

					for (int i = 0; i < textureDataSize; i++) {
						uint paletteIndex = data[i];
						if (paletteIndex >= 8 && paletteIndex < 256 - 8 && ((paletteIndex - 8) % 32 < 16)) {
							uint paletteMod = (paletteIndex - 8) % 16;
							if (paletteMod < 8) {
								paletteIndex += 8;
							} else {
								paletteIndex -= 8;
							}
						}
						pixels[i] = palette[paletteIndex];
					}
				} else {
					MapLoader.Loader.print("(RenderWare) Unknown Texture Format: " + rasterFormat);
				}
			}
			tex.SetPixels(pixels);
			switch (filterMode) {
				case FilterMode.FILTER_LINEAR:
				case FilterMode.FILTER_LINEAR_MIP_LINEAR:
				case FilterMode.FILTER_LINEAR_MIP_NEAREST:
				case FilterMode.FILTER_MIP_LINEAR:
					tex.filterMode = UnityEngine.FilterMode.Bilinear; break;
				case FilterMode.FILTER_NONE:
				case FilterMode.FILTER_NEAREST:
				case FilterMode.FILTER_MIP_NEAREST:
					tex.filterMode = UnityEngine.FilterMode.Point; break;
			}
			switch (addressingMode) {
				case AddressingMode.WRAP_NONE:
				case AddressingMode.WRAP_WRAP:
					tex.wrapMode = TextureWrapMode.Repeat; break;
				case AddressingMode.WRAP_MIRROR:
					tex.wrapMode = TextureWrapMode.Mirror; break;
				case AddressingMode.WRAP_CLAMP:
					tex.wrapMode = TextureWrapMode.Clamp; break;
			}
			tex.Apply();
			return tex;
		}

		[Flags]
		public enum RasterFormat {
			FORMAT_DEFAULT = 0x0000,
			FORMAT_1555 = 0x0100, //1 bit alpha, RGB 5 bits each; also used for DXT1 with alpha
			FORMAT_565 = 0x0200, //5 bits red, 6 bits green, 5 bits blue; also used for DXT1 without alpha
			FORMAT_4444 = 0x0300, //RGBA 4 bits each; also used for DXT3
			FORMAT_LUM8 = 0x0400, //gray scale, D3DFMT_L8
			FORMAT_8888 = 0x0500, //RGBA 8 bits each
			FORMAT_888 = 0x0600, //RGB 8 bits each, D3DFMT_X8R8G8B8
			FORMAT_555 = 0x0A00, //RGB 5 bits each - rare, use 565 instead, D3DFMT_X1R5G5B5
			FORMAT_EXT_AUTO_MIPMAP = 0x1000, //RW generates mipmaps, see special section below
			FORMAT_EXT_PAL8 = 0x2000, //2^8 = 256 palette colors
			FORMAT_EXT_PAL4 = 0x4000, //2^4 = 16 palette colors
			FORMAT_EXT_MIPMAP = 0x8000 //mipmaps included
		}

		public enum FilterMode {
			FILTER_NONE = 0x00,
			FILTER_NEAREST = 0x01,
			FILTER_LINEAR = 0x02,
			FILTER_MIP_NEAREST = 0x03,
			FILTER_MIP_LINEAR = 0x04,
			FILTER_LINEAR_MIP_NEAREST = 0x05,
			FILTER_LINEAR_MIP_LINEAR = 0x06
		}

		public enum AddressingMode {
			WRAP_NONE = 0x00,
			WRAP_WRAP = 0x01,
			WRAP_MIRROR = 0x02,
			WRAP_CLAMP = 0x03
		}

		public enum Type {
			Texture,
			Alpha
		}

		static uint extractBits(int number, int count, int offset) {
			return (uint)(((1 << count) - 1) & (number >> (offset)));
		}
	}
}