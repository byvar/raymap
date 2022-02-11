using DDSImageParser;
using LibGC.Texture;
using System;
using System.IO;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture {
	// Largo Winch JPEG Lightmaps file
    public class LMS {
        uint count = 0;
		uint baseOffset;
		uint unk;

        public uint Count {
            get { return count; }
        }

		public string path;

        public Texture2D[] textures = null;
        public LMS(Stream fs) {
            using (Reader reader = new Reader(fs, CPA_Settings.s.IsLittleEndian)) {
				count = reader.ReadUInt32();
				int[] sizes = new int[count]; 
				for (int i = 0; i < count; i++) {
					sizes[i] = reader.ReadInt32();
				}
				textures = new Texture2D[count];
				byte[] header = new byte[0xA0];
                for (uint i = 0; i < count; i++) {
					byte[] texBytes = reader.ReadBytes(sizes[i]);
					byte[] fullTexBytes;
					if (i == 0) {
						Array.Copy(texBytes, 0, header, 0, 0xA0);
						fullTexBytes = texBytes;
					} else {
						fullTexBytes = new byte[sizes[i] + header.Length];
						Array.Copy(header, 0, fullTexBytes, 0, header.Length);
						Array.Copy(texBytes, 0, fullTexBytes, header.Length, texBytes.Length);
					}
					textures[i] = new Texture2D(2, 2);
					textures[i].LoadImage(fullTexBytes);
				}
            }
        }
	}
}