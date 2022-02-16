using BinarySerializer.Unity;
using System;
using System.IO;
using UnityEngine;

namespace OpenSpace.FileFormat.Texture {
    public class GF64 {
        public int width, height;
		public bool hasAlpha;
		public Texture2D texture;
		public Format format;
		public int palette_num_colors;

		public GF64(Reader reader, LegacyPointer off_texture, int width, int height, Format format, LegacyPointer off_palette, int palette_num_colors) {
			this.width = width;
			this.height = height;
			this.format = format;
			this.palette_num_colors = palette_num_colors;
			Parse(reader, null, off_texture, off_palette);
		}

        public GF64(string filePath, int width, int height, Format format, string palettePath, int palette_num_colors)
			: this(FileSystem.GetFileReadStream(filePath), width, height, format, FileSystem.GetFileReadStream(palettePath), palette_num_colors) { }

		public GF64(Stream stream, int width, int height, Format format, Stream palette, int palette_num_colors) {
			MapLoader l = MapLoader.Loader;

			this.width = width;
			this.height = height;
			this.format = format;
			this.palette_num_colors = palette_num_colors;
			using (Reader r = new Reader(stream, Legacy_Settings.s.IsLittleEndian)) {
				if (palette != null) {
					using (Reader p = new Reader(palette, Legacy_Settings.s.IsLittleEndian)) {
						Parse(r, p, null, null);
					}
				} else {
					Parse(r, null, null, null);
				}
			}
		}

		public void LoadAlphaTexture(GF64 gf) {
			hasAlpha = true;
			Color[] colors = texture.GetPixels();
			Color[] alphaColors = gf.texture.GetPixels();
			for (int i = 0; i < colors.Length; i++) {
				colors[i] = new Color(colors[i].r, colors[i].g, colors[i].b, alphaColors[i].a);
			}
			texture.SetPixels(colors);
			texture.Apply();
		}

		private void Parse(Reader reader, Reader paletteReader, LegacyPointer off_texture, LegacyPointer off_palette) {
			
			// If both texture_index and texture_index2: texture_index2 is the real one, texture_index is palette_index.

			Color[] palette = null;
			if (off_palette != null) {
				LegacyPointer.DoAt(ref reader, off_palette, () => {
					palette = ReadPalette(reader);
				});
			} else if (paletteReader != null) {
				palette = ReadPalette(paletteReader);
			}
			texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			Color[] pixels = new Color[width * height];
			if (format == Format.I4 || format == Format.I4Alpha) {
				LegacyPointer off_current = null;
				if (off_texture != null) off_current = LegacyPointer.Goto(ref reader, off_texture);
				byte[] texBytes = reader.ReadBytes((width * height) / 2);
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						int index = (x * height) + y;
						bool shift = index % 2 != 0;
						byte texByte = texBytes[((x * height) + y) / 2];
						if (Legacy_Settings.s.IsLittleEndian) {
							if (shift) {
								texByte = (byte)(texByte >> 4);
							} else {
								texByte = (byte)(texByte & 0x0F);
							}
						} else {
							if (shift) {
								texByte = (byte)(texByte & 0x0F);
							} else {
								texByte = (byte)(texByte >> 4);
							}
						}
						if (format == Format.I4Alpha) {
							if (palette != null) {
								pixels[index] = new Color(1f,1f,1f, palette[texByte % palette.Length].r);
							} else {
								pixels[index] = new Color(1f, 1f, 1f, texByte / 15f);
							}
						} else {
							if (palette != null) {
								pixels[index] = palette[texByte % palette.Length];
							} else {
								pixels[index] = new Color(texByte / 15f, texByte / 15f, texByte / 15f, 1f);
							}
						}
					}
				}
				if (off_current != null) LegacyPointer.Goto(ref reader, off_current);
			} else if (format == Format.I8) {
				LegacyPointer off_current = null;
				if (off_texture != null) off_current = LegacyPointer.Goto(ref reader, off_texture);
				byte[] texBytes = reader.ReadBytes(width * height);
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						int index = (x * height) + y;
						byte texByte = texBytes[(x * height) + y];
						if (palette != null) {
							Color paletteCol = palette[texByte % palette.Length];
							if (palette.Length > 64) {
								pixels[index] = palette[texByte % palette.Length];
							} else {
								int alpha = texByte / palette.Length;
								pixels[index] = new Color(paletteCol.r, paletteCol.g, paletteCol.b, alpha / 7f);
							}
							/*} else {
								pixels[index] = palette[texByte % palette.Length];
							}*/
						} else {
							pixels[index] = new Color(1f, 1f, 1f, texByte / 255f);
						}
					}
				}
				if (off_current != null) LegacyPointer.Goto(ref reader, off_current);
			} else if (format == Format.RGBA) {
				LegacyPointer off_current = null;
				if (off_texture != null) off_current = LegacyPointer.Goto(ref reader, off_texture);
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						int index = (x * height) + y;
						ushort shortCol = reader.ReadUInt16();
						pixels[index] = ParseColorRGBA5551(shortCol);
					}
				}
				if (off_current != null) LegacyPointer.Goto(ref reader, off_current);
			}
			texture.SetPixels(pixels);
			texture.Apply();
		}

		Color[] ReadPalette(Reader reader) {
			Color[] palette = new Color[palette_num_colors];
			for (int i = 0; i < palette_num_colors; i++) {
				ushort shortCol = reader.ReadUInt16();
				palette[i] = ParseColorRGBA5551(shortCol);
			}
			return palette;
		}

        static uint ExtractBits(int number, int count, int offset) {
            return (uint)(((1 << count) - 1) & (number >> (offset)));
		}

		static uint ReverseBits(uint number, int count) {
			uint result = 0;
			for (int i = 0; i < count; i++) {
				result |= (uint)(((number & (1 << i)) >> i) << (count-1-i));
			}
			return result;
		}

		static Color ParseColorRGBA5551(ushort shortCol) {
			uint alpha, blue, green, red;
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.DS) {
				alpha = ExtractBits(shortCol, 1, 15);
				blue = ExtractBits(shortCol, 5, 10);
				green = ExtractBits(shortCol, 5, 5);
				red = ExtractBits(shortCol, 5, 0);
			} else {
				alpha = ExtractBits(shortCol, 1, 0);
				blue = ExtractBits(shortCol, 5, 1);
				green = ExtractBits(shortCol, 5, 6);
				red = ExtractBits(shortCol, 5, 11);
			}
			return new Color(red / 31.0f, green / 31.0f, blue / 31.0f, alpha);
		}

		/*public enum Format {
			CI4, // Paletted, each index is 4 bits
			CI8, // Paletted, each index is 8 bits
			I4, // Not paletted, each B/W value is 4 bits
			I8, // Not paletted, each B/W value is 8 bits
		}*/

		public enum Format {
			I4,
			I4Alpha,
			I8,
			RGBA // RGBA5551
		}

    }
}