using LibGC.Texture;
using System;
using System.IO;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture {
    public class TPL {
        bool isLittleEndian = false;
        uint count = 0;

        public uint Count {
            get { return count; }
        }

		public string path;

        public Texture2D[] textures = null;
        public TPL(string path) {
			this.path = path;
            Stream fs = FileSystem.GetFileReadStream(path);
            using (Reader reader = new Reader(fs, isLittleEndian)) {
                reader.ReadUInt32();
                count = reader.ReadUInt32();
                textures = new Texture2D[count];
                uint off_imtable = reader.ReadUInt32();
                for (uint i = 0; i < count; i++) {
                    fs.Seek(off_imtable + 8 * i, SeekOrigin.Begin);
                    uint off_imheader = reader.ReadUInt32();
                    uint off_palheader = reader.ReadUInt32();
                    fs.Seek(off_imheader, SeekOrigin.Begin);
                    uint height = reader.ReadUInt16();
                    uint width = reader.ReadUInt16();
                    uint format = reader.ReadUInt32();
                    uint off_imdata = reader.ReadUInt32();
                    uint wrapS = reader.ReadUInt32();
                    uint wrapT = reader.ReadUInt32();
                    uint minFilter = reader.ReadUInt32();
                    uint maxFilter = reader.ReadUInt32();
                    float LODBias = reader.ReadSingle();
                    byte edgeLODenable = reader.ReadByte();
                    byte minLOD = reader.ReadByte();
                    byte maxLOD = reader.ReadByte();
                    byte unpacked = reader.ReadByte();
                    fs.Seek(off_imdata, SeekOrigin.Begin);
                    switch (format) {
                        case 0x01:
                            textures[i] = ReadTexture(fs, reader, (int)height, (int)width, GcTextureFormat.I8);
                            break;
                        case 0x02:
                            textures[i] = ReadTexture(fs, reader, (int)height, (int)width, GcTextureFormat.IA4);
                            break;
                        case 0x06:
                            // RGBA32 (RGBA8)
                            textures[i] = ReadTexture(fs, reader, (int)height, (int)width, GcTextureFormat.RGBA8);
                            break;
                        case 0x0E:
                            // CMPR
                            textures[i] = ReadTexture(fs, reader, (int)height, (int)width, GcTextureFormat.CMPR);
                            break;
                        default:
                            throw new FormatException("Format not implemented: " + format);
                    }
                }
            }
        }

        private Texture2D ReadTexture(Stream fs, Reader reader, int height, int width, LibGC.Texture.GcTextureFormat format) {
            GcTextureFormatCodec codec = GcTextureFormatCodec.GetCodec(format);
            int byteSize = codec.CalcTextureSize(width, height);
            //int stride = (width / block_width) * bpp / 8;
            int stride = width * 4;
            byte[] imageData = reader.ReadBytes(byteSize);
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            byte[] destData = new byte[width * height * 4];
            codec.DecodeTexture(destData, 0, width, height, stride, imageData, 0, null, 0);
            tex.LoadRawTextureData(destData);
            tex.Apply();
            return tex;
        }
    }
}