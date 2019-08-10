using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

// Source: https://github.com/xdanieldzd/UntoldUnpack/tree/master/UntoldUnpack/Graphics

namespace UntoldUnpack.Graphics {
	internal delegate void TileDecoderDelegate(BinaryReader reader, byte[] targetData, int x, int y, int width, int height);

	internal class Codec {
		public PicaDataTypes DataType { get; private set; }
		public PicaPixelFormats PixelFormat { get; private set; }
		public TileDecoderDelegate Decoder { get; private set; }

		public Codec(PicaDataTypes dataType, PicaPixelFormats pixelFormat, TileDecoderDelegate decoder) {
			DataType = dataType;
			PixelFormat = pixelFormat;
			Decoder = decoder;
		}
	}

	internal static class TileCodecs {
		static List<Codec> codecs = new List<Codec>()
		{
            /* RGBA4444 */ new Codec(PicaDataTypes.UnsignedShort4444,   PicaPixelFormats.RGBANativeDMP,             DecodeRGBA4444),
            /* RGBA5551 */ new Codec(PicaDataTypes.UnsignedShort5551,   PicaPixelFormats.RGBANativeDMP,             DecodeRGBA5551),
            /* RGBA8888 */ new Codec(PicaDataTypes.UnsignedByte,        PicaPixelFormats.RGBANativeDMP,             DecodeRGBA8888),
            /* RGB565   */ new Codec(PicaDataTypes.UnsignedShort565,    PicaPixelFormats.RGBNativeDMP,              DecodeRGB565),
            /* RGB888   */ new Codec(PicaDataTypes.UnsignedByte,        PicaPixelFormats.RGBNativeDMP,              DecodeRGB888),
            /* ETC1     */ new Codec(PicaDataTypes.UnsignedByte,        PicaPixelFormats.ETC1RGB8NativeDMP,         DecodeETC1),
            /* ETC1_A4  */ new Codec(PicaDataTypes.UnsignedByte,        PicaPixelFormats.ETC1AlphaRGB8A4NativeDMP,  DecodeETC1_A4),
            /* A8       */ new Codec(PicaDataTypes.UnsignedByte,        PicaPixelFormats.AlphaNativeDMP,            DecodeA8),
            /* A4       */ new Codec(PicaDataTypes.Unsigned4BitsDMP,    PicaPixelFormats.AlphaNativeDMP,            DecodeA4),
            /* L8       */ new Codec(PicaDataTypes.UnsignedByte,        PicaPixelFormats.LuminanceNativeDMP,        DecodeL8),
            /* L4       */ new Codec(PicaDataTypes.Unsigned4BitsDMP,    PicaPixelFormats.LuminanceNativeDMP,        DecodeL4),
            /* LA88     */ new Codec(PicaDataTypes.UnsignedByte,        PicaPixelFormats.LuminanceAlphaNativeDMP,   DecodeLA88),
            /* LA44     */ new Codec(PicaDataTypes.UnsignedByte44DMP,   PicaPixelFormats.LuminanceAlphaNativeDMP,   DecodeLA44)
		};

		static readonly int[] tileOrder =
		{
			0, 1, 8, 9,
			2, 3, 10, 11,
			16, 17, 24, 25,
			18, 19, 26, 27,

			4, 5, 12, 13,
			6, 7, 14, 15,
			20, 21, 28, 29,
			22, 23, 30, 31,

			32, 33, 40, 41,
			34, 35, 42, 43,
			48, 49, 56, 57,
			50, 51, 58, 59,

			36, 37, 44, 45,
			38, 39, 46, 47,
			52, 53, 60, 61,
			54, 55, 62, 63
		};

		static readonly int[,] etc1ModifierTables =
		{
			{ 2, 8, -2, -8 },
			{ 5, 17, -5, -17 },
			{ 9, 29, -9, -29 },
			{ 13, 42, -13, -42 },
			{ 18, 60, -18, -60 },
			{ 24, 80, -24, -80 },
			{ 33, 106, -33, -106 },
			{ 47, 183, -47, -183 }
		};

		private static Codec GetCodec(PicaDataTypes dataType, PicaPixelFormats picaPixelFormat) {
			Codec codec = codecs.FirstOrDefault(x => x.DataType == dataType && x.PixelFormat == picaPixelFormat);
			if (codec == null) throw new Exception(string.Format("Codec not found for {0} {1}", dataType, picaPixelFormat));
			return codec;
		}

		public static TileDecoderDelegate GetDecoder(PicaDataTypes dataType, PicaPixelFormats picaPixelFormat) {
			Codec codec = GetCodec(dataType, picaPixelFormat);
			if (codec.Decoder == null) throw new Exception(string.Format("Decoder not found for {0} {1}", dataType, picaPixelFormat));
			return codec.Decoder;
		}

		private static int GetTilePixelIndex(int t, int x, int y, int width) {
			return (int)((((tileOrder[t] / 8) + y) * width) + ((tileOrder[t] % 8) + x));
		}

		private static int GetTilePixelOffset(int t, int x, int y, int width, int pixelFormatSize) {
			return (GetTilePixelIndex(t, x, y, width) * (pixelFormatSize / 8));
		}

		private static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
			if (val.CompareTo(min) < 0) return min;
			else if (val.CompareTo(max) > 0) return max;
			else return val;
		}

		#region Decoders

		private static void DecodeRGBA4444(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				ushort rgba = reader.ReadUInt16();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = Common.ResampleChannel((rgba >> 0), 4, 8);
				targetData[pixelOffset + 0] = Common.ResampleChannel((rgba >> 4), 4, 8);
				targetData[pixelOffset + 1] = Common.ResampleChannel((rgba >> 8), 4, 8);
				targetData[pixelOffset + 2] = Common.ResampleChannel((rgba >> 12), 4, 8);
			}
		}

		private static void DecodeRGBA5551(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				ushort rgba = reader.ReadUInt16();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = Common.ResampleChannel((rgba >> 0), 1, 8);
				targetData[pixelOffset + 0] = Common.ResampleChannel((rgba >> 1), 5, 8);
				targetData[pixelOffset + 1] = Common.ResampleChannel((rgba >> 6), 5, 8);
				targetData[pixelOffset + 2] = Common.ResampleChannel((rgba >> 11), 5, 8);
			}
		}

		private static void DecodeRGBA8888(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				byte a = reader.ReadByte(), b = reader.ReadByte(), g = reader.ReadByte(), r = reader.ReadByte();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = a;
				targetData[pixelOffset + 0] = b;
				targetData[pixelOffset + 1] = g;
				targetData[pixelOffset + 2] = r;
			}
		}

		private static void DecodeRGB565(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				ushort rgba = reader.ReadUInt16();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = 0xFF;
				targetData[pixelOffset + 0] = Common.ResampleChannel((rgba >> 0), 5, 8);
				targetData[pixelOffset + 1] = Common.ResampleChannel((rgba >> 5), 6, 8);
				targetData[pixelOffset + 2] = Common.ResampleChannel((rgba >> 11), 5, 8);
			}
		}

		private static void DecodeRGB888(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				byte b = reader.ReadByte(), g = reader.ReadByte(), r = reader.ReadByte();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = 0xFF;
				targetData[pixelOffset + 0] = b;
				targetData[pixelOffset + 1] = g;
				targetData[pixelOffset + 2] = r;
			}
		}

		private static void DecodeETC1(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			DecodeETC1Tile(reader, targetData, x, y, width, height, false);
		}

		private static void DecodeETC1_A4(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			DecodeETC1Tile(reader, targetData, x, y, width, height, true);
		}

		private static void DecodeA8(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				byte a = reader.ReadByte();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = a;
				targetData[pixelOffset + 0] = targetData[pixelOffset + 1] = targetData[pixelOffset + 2] = 0xFF;
			}
		}

		private static void DecodeA4(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t += 2) {
				byte a4 = reader.ReadByte();
				for (int i = 0; i < 2; i++) {
					int pixelOffset = GetTilePixelOffset(t + i, x, y, width, 32);
					if (pixelOffset >= targetData.Length) continue;

					targetData[pixelOffset + 3] = (byte)(((a4 >> (i * 4)) & 0x0F) << 4 | ((a4 >> (i * 4)) & 0x0F));
					targetData[pixelOffset + 0] = targetData[pixelOffset + 1] = targetData[pixelOffset + 2] = 0xFF;
				}
			}
		}

		private static void DecodeL8(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				byte l = reader.ReadByte();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = 0xFF;
				targetData[pixelOffset + 0] = targetData[pixelOffset + 1] = targetData[pixelOffset + 2] = l;
			}
		}

		private static void DecodeL4(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t += 2) {
				byte l4 = reader.ReadByte();
				for (int i = 0; i < 2; i++) {
					int pixelOffset = GetTilePixelOffset(t + i, x, y, width, 32);
					if (pixelOffset >= targetData.Length) continue;

					targetData[pixelOffset + 3] = 0xFF;
					targetData[pixelOffset + 0] = targetData[pixelOffset + 1] = targetData[pixelOffset + 2] = (byte)(((l4 >> (i * 4)) & 0x0F) << 4 | ((l4 >> (i * 4)) & 0x0F));
				}
			}
		}

		private static void DecodeLA88(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				byte a = reader.ReadByte(), l = reader.ReadByte();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = a;
				targetData[pixelOffset + 0] = targetData[pixelOffset + 1] = targetData[pixelOffset + 2] = l;
			}
		}

		private static void DecodeLA44(BinaryReader reader, byte[] targetData, int x, int y, int width, int height) {
			for (int t = 0; t < tileOrder.Length; t++) {
				byte la = reader.ReadByte();
				int pixelOffset = GetTilePixelOffset(t, x, y, width, 32);
				if (pixelOffset >= targetData.Length) continue;

				targetData[pixelOffset + 3] = (byte)(((la >> 0) & 0x0F) << 4 | ((la >> 0) & 0x0F));
				targetData[pixelOffset + 0] = targetData[pixelOffset + 1] = targetData[pixelOffset + 2] = (byte)(((la >> 4) & 0x0F) << 4 | ((la >> 4) & 0x0F));
			}
		}

		private static void DecodeETC1Tile(BinaryReader reader, byte[] targetData, int x, int y, int width, int height, bool hasAlpha) {
			/* Specs: https://www.khronos.org/registry/gles/extensions/OES/OES_compressed_ETC1_RGB8_texture.txt */

			/* Other implementations:
             * https://github.com/richgel999/rg-etc1/blob/master/rg_etc1.cpp
             * https://github.com/Gericom/EveryFileExplorer/blob/master/3DS/GPU/Textures.cs
             * https://github.com/gdkchan/Ohana3DS-Rebirth/blob/master/Ohana3DS%20Rebirth/Ohana/TextureCodec.cs */

			for (int by = 0; by < 8; by += 4) {
				for (int bx = 0; bx < 8; bx += 4) {
					ulong alpha = (hasAlpha ? reader.ReadUInt64() : 0xFFFFFFFFFFFFFFFF);
					ulong block = reader.ReadUInt64();

					using (BinaryReader decodedReader = new BinaryReader(new MemoryStream(DecodeETC1Block(block)))) {
						for (int py = 0; py < 4; py++) {
							for (int px = 0; px < 4; px++) {
								if (x + bx + px >= width) continue;
								if (y + by + py >= height) continue;

								int pixelOffset = (int)((((y + by + py) * width) + (x + bx + px)) * 4);
								Buffer.BlockCopy(decodedReader.ReadBytes(3), 0, targetData, pixelOffset, 3);
								byte pixelAlpha = (byte)((alpha >> (((px * 4) + py) * 4)) & 0xF);
								targetData[pixelOffset + 3] = (byte)((pixelAlpha << 4) | pixelAlpha);
							}
						}
					}
				}
			}
		}

		private static byte[] DecodeETC1Block(ulong block) {
			byte r1, g1, b1, r2, g2, b2;

			byte tableIndex1 = (byte)((block >> 37) & 0x07);
			byte tableIndex2 = (byte)((block >> 34) & 0x07);
			byte diffBit = (byte)((block >> 33) & 0x01);
			byte flipBit = (byte)((block >> 32) & 0x01);

			if (diffBit == 0x00) {
				/* Individual mode */
				r1 = (byte)(((block >> 60) & 0x0F) << 4 | (block >> 60) & 0x0F);
				g1 = (byte)(((block >> 52) & 0x0F) << 4 | (block >> 52) & 0x0F);
				b1 = (byte)(((block >> 44) & 0x0F) << 4 | (block >> 44) & 0x0F);

				r2 = (byte)(((block >> 56) & 0x0F) << 4 | (block >> 56) & 0x0F);
				g2 = (byte)(((block >> 48) & 0x0F) << 4 | (block >> 48) & 0x0F);
				b2 = (byte)(((block >> 40) & 0x0F) << 4 | (block >> 40) & 0x0F);
			} else {
				/* Differential mode */

				/* 5bit base values */
				byte r1a = (byte)(((block >> 59) & 0x1F));
				byte g1a = (byte)(((block >> 51) & 0x1F));
				byte b1a = (byte)(((block >> 43) & 0x1F));

				/* Subblock 1, 8bit extended */
				r1 = (byte)((r1a << 3) | (r1a >> 2));
				g1 = (byte)((g1a << 3) | (g1a >> 2));
				b1 = (byte)((b1a << 3) | (b1a >> 2));

				/* 3bit modifiers */
				sbyte dr2 = (sbyte)((block >> 56) & 0x07);
				sbyte dg2 = (sbyte)((block >> 48) & 0x07);
				sbyte db2 = (sbyte)((block >> 40) & 0x07);
				if (dr2 >= 4) dr2 -= 8;
				if (dg2 >= 4) dg2 -= 8;
				if (db2 >= 4) db2 -= 8;

				/* Subblock 2, 8bit extended */
				r2 = (byte)((r1a + dr2) << 3 | (r1a + dr2) >> 2);
				g2 = (byte)((g1a + dg2) << 3 | (g1a + dg2) >> 2);
				b2 = (byte)((b1a + db2) << 3 | (b1a + db2) >> 2);
			}

			byte[] decodedData = new byte[(4 * 4) * 3];

			using (BinaryWriter writer = new BinaryWriter(new MemoryStream(decodedData))) {
				for (int py = 0; py < 4; py++) {
					for (int px = 0; px < 4; px++) {
						int index = (int)(((block >> ((px * 4) + py)) & 0x1) | ((block >> (((px * 4) + py) + 16)) & 0x1) << 1);

						if ((flipBit == 0x01 && py < 2) || (flipBit == 0x00 && px < 2)) {
							int modifier = etc1ModifierTables[tableIndex1, index];
							writer.Write((byte)((b1 + modifier)).Clamp<int>(byte.MinValue, byte.MaxValue));
							writer.Write((byte)((g1 + modifier)).Clamp<int>(byte.MinValue, byte.MaxValue));
							writer.Write((byte)((r1 + modifier)).Clamp<int>(byte.MinValue, byte.MaxValue));
						} else {
							int modifier = etc1ModifierTables[tableIndex2, index];
							writer.Write((byte)((b2 + modifier)).Clamp<int>(byte.MinValue, byte.MaxValue));
							writer.Write((byte)((g2 + modifier)).Clamp<int>(byte.MinValue, byte.MaxValue));
							writer.Write((byte)((r2 + modifier)).Clamp<int>(byte.MinValue, byte.MaxValue));
						}
					}
				}
			}

			return decodedData;
		}

		#endregion
	}
}
