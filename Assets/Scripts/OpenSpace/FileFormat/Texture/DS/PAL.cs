using BinarySerializer.Unity;
using LibGC.Texture;
using System;
using System.IO;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture.DS {
    public class PAL {
		public string path;
		public Color[] palette;

        public PAL(string path) {
			this.path = path;
            Stream fs = FileSystem.GetFileReadStream(path);
            using (Reader reader = new Reader(fs, Legacy_Settings.s.IsLittleEndian)) {
				palette = new Color[reader.BaseStream.Length/2];
				for (int i = 0; i < palette.Length; i++) {
					ushort shortCol = reader.ReadUInt16();
					palette[i] = ParseColorRGBA5551(shortCol);
					palette[i].a = 1;
				}
            }
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
		static uint ExtractBits(int number, int count, int offset) {
			return (uint)(((1 << count) - 1) & (number >> (offset)));
		}
	}
}