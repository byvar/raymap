using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.Texture {
    // R2 Dreamcast texture format
    public class TEX {
        uint count = 0;

        public uint Count {
            get { return count; }
        }

        public Texture2D[] textures = null;

		public TEX(string path, bool compressed = true) {
			Stream fs = FileSystem.GetFileReadStream(path);
			using (Reader reader = new Reader(fs, Legacy_Settings.s.IsLittleEndian)) {
				if (compressed) {
					reader.ReadUInt32(); // "LZSS"
					List<Texture2D> texturesList = new List<Texture2D>();
					while (reader.BaseStream.Position < reader.BaseStream.Length) {
						uint zsize = reader.ReadUInt32() - 4;
						byte[] compressedData = reader.ReadBytes((int)zsize);
						byte[] decompressedData = DecompressLZSS(compressedData);
						PvrTexture pvr = new PvrTexture(decompressedData);
						texturesList.Add(pvr.ToTexture2D());
					}
					textures = texturesList.ToArray();
					count = (uint)textures.Length;
				} else {
					textures = new Texture2D[1];
					PvrTexture pvr = new PvrTexture(reader.ReadBytes((int)fs.Length));
					textures[0] = pvr.ToTexture2D();
					count = 1;
				}
			}
		}

        private byte[] DecompressLZSS(byte[] compressed) {
            byte[] decompressed = new byte[compressed.Length * 128];
            uint decompressedPos = 0;
            using (Reader reader = new Reader(new MemoryStream(compressed), Legacy_Settings.s.IsLittleEndian)) {
                while (reader.BaseStream.Position < reader.BaseStream.Length) {
                    byte controlByte = reader.ReadByte();
                    if (controlByte == 0) {
                        // Read 4 uncompressed bytes
                        Array.Copy(reader.ReadBytes(4), 0, decompressed, decompressedPos, 4);
                        decompressedPos += 4;
                    } else {
                        if ((controlByte & 1) == 1) {
                            // Read data about LZSS dict reference
                            ushort offByte = reader.ReadUInt16();
                            byte length = reader.ReadByte();

                            //Calculate dictionary offset
                            uint dictOff = decompressedPos - ((((uint)offByte) << 7) + (((uint)controlByte) >> 1));
                            for (int i = 0; i < length; i++) {
                                decompressed[decompressedPos++] = decompressed[dictOff++];
                            }
                        } else {
                            while (controlByte > 2) {
                                // Read and save one uncompressed byte
                                decompressed[decompressedPos++] = reader.ReadByte();
                                controlByte >>= 1;
                            }
                        }
                    }
                }
            }
            Array.Resize(ref decompressed, (int)decompressedPos);
            return decompressed;
        }
    }
}