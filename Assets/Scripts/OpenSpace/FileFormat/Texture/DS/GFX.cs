using BinarySerializer.Unity;
using LibGC.Texture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture.DS {
    public class GFX {
		public string gfxPath;
		public string mapPath = null;
		public string palPath = null;
		private MapEntry[] map = null;
		private Color[][] palettes = null;
		public Texture2D texture = null;

		public GFX(string gfxPath, string mapPath, string palPath, int width, int height) {
			this.gfxPath = gfxPath;
			this.mapPath = mapPath;
			this.palPath = palPath;
			Stream palFS = FileSystem.GetFileReadStream(palPath);
			using (Reader reader = new Reader(palFS, Settings.s.IsLittleEndian)) {
				palettes = new Color[reader.BaseStream.Length / 32][];
				for (int i = 0; i < palettes.Length; i++) {
					palettes[i] = new Color[16];
					for (int j = 0; j < palettes[i].Length; j++) {
						palettes[i][j] = ParseColorRGBA5551(reader.ReadUInt16());
						palettes[i][j].a = 1;
					}
				}
			}
			Stream mapFS = FileSystem.GetFileReadStream(mapPath);
			using (Reader reader = new Reader(mapFS, Settings.s.IsLittleEndian)) {
				map = new MapEntry[reader.BaseStream.Length / 2];
				for (int i = 0; i < map.Length; i++) {
					map[i] = new MapEntry(reader.ReadUInt16());
				}
			}
			int tile_size = 8;
			Stream fs = FileSystem.GetFileReadStream(gfxPath);
            using (Reader reader = new Reader(fs, Settings.s.IsLittleEndian)) {
				texture = new Texture2D(width * tile_size, height * tile_size);
				Color[] pixels = new Color[texture.width * texture.height];
				byte[][] tiles = new byte[reader.BaseStream.Length / 0x20][];
				for (int i = 0; i < tiles.Length; i++) {
					tiles[i] = reader.ReadBytes(0x20);
				}
				/*byte[] tile_pal;
				byte[] applied_tiles = Apply_Map(tiles, out tile_pal, 16, 4);
				*/
				Color[][] applied_tiles = Apply_Map(tiles, 4, tile_size, palettes);
				//Color[] flattened_tiles = applied_tiles.SelectMany(a => a).ToArray();
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						int tile_index = x * height + y;
						if (tile_index >= applied_tiles.Length) continue;
						Color[] tile = applied_tiles[tile_index];
						/*for (int i = 0; i < tile_size * tile_size; i++) {
							pixels[(tile_index*tile_size*tile_size)+i] = tile[i];
						}*/
						for (int tx = 0; tx < tile_size; tx++) {
							for (int ty = 0; ty < tile_size; ty++) {
								int index = ((x * tile_size + tx) * texture.height) + (y * tile_size + ty);
								pixels[index] = tile[tx * tile_size + ty];
							}
						}
						/*for (int tx = 0; tx < tile_size; tx++) {
							for (int ty = 0; ty < tile_size; ty++) {
								int index = ((x * tile_size + tx) * texture.height) + (y * tile_size + ty);
								pixels[index] = flattened_tiles[index];
							}
						}*/
					}
				}
				texture.SetPixels(pixels);
				texture.Apply();
            }
        }

		public Color[][] Apply_Map(byte[][] tiles, int bpp, int tile_size, Color[][] palettes) {
			int tile_width = tile_size * bpp / 8;
			int tile_length = tile_size * tile_width;
			int num_tiles = tiles.Length;

			Color[][] newTiles = new Color[map.Length][];
			for (int i = 0; i < map.Length; i++) {
				if (map[i].nTile >= num_tiles) map[i].nTile = 0;

				byte[] curTile = new byte[tile_length];
				Array.Copy(tiles[map[i].nTile], 0, curTile, 0, tile_length);

				//if (map[i].xFlip == 1) curTile = XFlip(curTile, tile_size, bpp);
				//if (map[i].yFlip == 1) curTile = YFlip(curTile, tile_size, bpp);

				newTiles[i] = new Color[tile_size * tile_size];
				for (int x = 0; x < tile_size; x++) {
					for (int y = 0; y < tile_size; y++) {
						byte palIndex = curTile[((x * tile_size) + y) / 2];
						if (((x * tile_size) + y) % 2 == 0) {
							palIndex = (byte)(palIndex & 0xF);
						} else {
							palIndex = (byte)((palIndex >> 4) & 0xF);
						}
						newTiles[i][(x * tile_size) + y] = palettes[map[i].nPalette][palIndex];
					}
				}

				/*for (int t = 0; t < tile_size * tile_size; t++)
					tile_pal[i * tile_size * tile_size + t] = map[i].nPalette;*/
			}

			return newTiles;
		}

		public static Byte[] XFlip(Byte[] tile, int tile_size, int bpp) {
			byte[] newTile = new byte[tile.Length];
			int tile_width = tile_size * bpp / 8;

			for (int h = 0; h < tile_size; h++) {
				for (int w = 0; w < tile_width / 2; w++) {
					byte b = tile[((tile_width - 1) - w) + h * tile_width];
					newTile[w + h * tile_width] = Reverse_Bits(b, bpp);

					b = tile[w + h * tile_width];
					newTile[((tile_width - 1) - w) + h * tile_width] = Reverse_Bits(b, bpp);
				}
			}
			return newTile;
		}
		public static Byte Reverse_Bits(byte b, int length) {
			byte rb = 0;

			if (length == 4)
				rb = (byte)((b << 4) + (b >> 4));
			else if (length == 8)
				return b;

			return rb;
		}
		public static Byte[] YFlip(Byte[] tile, int tile_size, int bpp) {
			byte[] newTile = new byte[tile.Length];
			int tile_width = tile_size * bpp / 8;

			for (int h = 0; h < tile_size / 2; h++) {
				for (int w = 0; w < tile_width; w++) {
					newTile[w + h * tile_width] = tile[w + (tile_size - 1 - h) * tile_width];
					newTile[w + (tile_size - 1 - h) * tile_width] = tile[w + h * tile_width];
				}
			}
			return newTile;
		}

		private byte[] ReadTile(Reader reader) {
			byte[] pixels = reader.ReadBytes(0x20);
			for (int x = 0; x < 8; x++) {
				for (int y = 0; y < 8; y++) {
					byte texByte = reader.ReadByte();
					/*if (map != null) {
						Array.Copy(
					} else {
						pixels[x * 8 + y] = palette[texByte];
					}*/
				}
			}
			return pixels;
		}

		static Color ParseColorRGBA5551(ushort shortCol) {
			uint alpha, blue, green, red;
			if (Settings.s.platform == Settings.Platform.DS) {
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
		static uint ExtractBits(int number, int count, int offset) {
			return (uint)(((1 << count) - 1) & (number >> (offset)));
		}

		private class MapEntry {
			public ushort nTile;
			public byte xFlip;
			public byte yFlip;
			public byte nPalette;

			public MapEntry(ushort value) {
				nTile = (ushort)(value & 0x3FF);
				xFlip = (byte)((value >> 10) & 1);
				yFlip = (byte)((value >> 11) & 1);
				nPalette = (byte)((value >> 12) & 0xF);
			}
		}
	}
}