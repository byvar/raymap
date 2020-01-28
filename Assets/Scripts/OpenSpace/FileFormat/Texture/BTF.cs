using DDSImageParser;
using LibGC.Texture;
using System;
using System.IO;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture {
	// Largo Winch DDS Textures file
    public class BTF {
		uint baseOffset;
		uint unk;

		public uint Count { get; } = 0;

		public string path;

		public Texture2D[] textures = null;
		public BTFTexture[] headers = null;

        public BTF(string btfPath, string bhfPath) {
			path = btfPath;
			Stream btf = FileSystem.GetFileReadStream(btfPath);
			Stream bhf = FileSystem.GetFileReadStream(bhfPath);
			using (Reader reader = new Reader(bhf, Settings.s.IsLittleEndian)) {
				Count = reader.ReadUInt32();

				headers = new BTFTexture[Count];
				textures = new Texture2D[Count];
				for (uint i = 0; i < Count; i++) {
					headers[i] = new BTFTexture();
					headers[i].word_00 = reader.ReadUInt16(); // always 1
					headers[i].word_02 = reader.ReadUInt16(); // always 4
					headers[i].offset = reader.ReadUInt32();
					headers[i].dword_08 = reader.ReadUInt32(); // always 0
					headers[i].flags = reader.ReadUInt32();
					headers[i].dword_10 = reader.ReadUInt32();
				}
			}
			using (Reader reader = new Reader(btf, Settings.s.IsLittleEndian)) {
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
            }
        }

		public class BTFTexture {
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
	}
}