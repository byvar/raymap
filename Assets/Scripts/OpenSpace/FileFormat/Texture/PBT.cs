using DDSImageParser;
using LibGC.Texture;
using System;
using System.IO;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture {
	// Largo Winch DDS Textures file
    public class PBT {
        uint count = 0;
		uint baseOffset;
		uint unk;

        public uint Count {
            get { return count; }
        }

		public string path;

        public Texture2D[] textures = null;
        public PBT(Stream fs) {
            using (Reader reader = new Reader(fs, Settings.s.IsLittleEndian)) {
                baseOffset = reader.ReadUInt32();
				unk = reader.ReadUInt32();

				count = (baseOffset - 8) / 8;

				textures = new Texture2D[count];
				uint off_imtable = 8;
                for (uint i = 0; i < count; i++) {
                    fs.Seek(off_imtable + 8 * i, SeekOrigin.Begin);
                    uint off_image = reader.ReadUInt32();
                    uint image_size = reader.ReadUInt32();
                    fs.Seek(off_image, SeekOrigin.Begin);
					byte[] texData = reader.ReadBytes((int)image_size);
					using (DDSImage dds = new DDSImage(texData)) {
						textures[i] = dds.BitmapImage;
					}
				}
            }
        }
	}
}