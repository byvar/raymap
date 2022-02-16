using BinarySerializer.Unity;
using LibGC.Texture;
using System;
using System.IO;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture.DS {
    public class NBFC {
		public string path;
		private bool i4;
		public Texture2D texture = null;

        public NBFC(string path, int width, int height, Color[] palette, bool i4) {
			this.path = path;
			this.i4 = i4;
            Stream fs = FileSystem.GetFileReadStream(path);
            using (Reader reader = new Reader(fs, Legacy_Settings.s.IsLittleEndian)) {
				texture = new Texture2D(width * 8, height * 8);
				Color[] pixels = new Color[texture.width * texture.height];
				int tile_size = 8;
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						Color[] tile = ReadTile(reader, palette);
						for (int tx = 0; tx < tile_size; tx++) {
							for (int ty = 0; ty < tile_size; ty++) {
								pixels[((x * tile_size + tx) * texture.height) + (y * tile_size + ty)] = tile[tx * tile_size + ty];
							}
						}
					}
				}
				texture.SetPixels(pixels);
				texture.Apply();
            }
        }

		private Color[] ReadTile(Reader reader, Color[] palette) {
			int tile_size = 8;
			Color[] pixels = new Color[tile_size * tile_size];
			byte[] texBytes = reader.ReadBytes(tile_size * (i4 ? tile_size / 2 : tile_size));
			for (int x = 0; x < tile_size; x++) {
				for (int y = 0; y < tile_size; y++) {
					int index = x * tile_size + y;
					if (i4) {
						bool shift = index % 2 != 0;
						byte texByte = texBytes[index / 2];
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
						pixels[index] = palette[texByte % palette.Length];
					} else {
						pixels[index] = palette[texBytes[index] % palette.Length];
					}
				}
			}
			return pixels;
		}
	}
}