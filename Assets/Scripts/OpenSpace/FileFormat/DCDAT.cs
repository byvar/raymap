using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class DCDAT : FileWithPointers {
        string path;
        long length;
        byte[] data = null;

        public DCDAT(string name, string path, int fileID) : this(name, FileSystem.GetFileReadStream(path), fileID) {
            this.path = path;
        }

        public DCDAT(string name, Stream stream, int fileID) {
            allowUnsafePointers = true;
            this.name = name;
            this.fileID = fileID;
            length = stream.Length;
            using (Reader fileReader = new Reader(stream, Settings.s.IsLittleEndian)) {
                data = fileReader.ReadBytes((int)stream.Length);
            }
            reader = new Reader(new MemoryStream(data), Settings.s.IsLittleEndian);
            //reader = new Reader(stream, Settings.s.IsLittleEndian);
            switch (fileID) {
                case 0:
                    reader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                    headerOffset = reader.ReadUInt32();
                    break;
                case 1:
                    reader.BaseStream.Seek(0x94, SeekOrigin.Begin);
                    headerOffset = reader.ReadUInt32();
                    break;
            }
            baseOffset = -headerOffset;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        public void SetHeaderOffset(uint headerOffset) {
            this.headerOffset = headerOffset;
            baseOffset = -headerOffset;
        }

        public override void CreateWriter() {
            return; // No writing support for DC DAT files yet
        }

        public override void WritePointer(Pointer pointer) {
            if (writer != null) {
                if (pointer == null) {
                    writer.Write((uint)0);
                } else {
                    writer.Write(pointer.offset);
                }
            }
        }

        public override Pointer GetUnsafePointer(uint value) {
            if (value >= headerOffset && value < headerOffset + length) {
                return new Pointer(value, this);
            } else {
                MapLoader l = MapLoader.Loader;
                foreach (FileWithPointers f in l.files_array) {
                    DCDAT dcFile = f as DCDAT;
                    if (dcFile != null && value >= dcFile.headerOffset && value < dcFile.headerOffset + dcFile.length) {
                        return new Pointer(value, dcFile);
                    }
                }
                // Do a second loop over the files. If end and start overlap we want the start (returned by previous loop),
                // but without the second loop some valid pointers will be null
                foreach (FileWithPointers f in l.files_array) {
                    DCDAT dcFile = f as DCDAT;
                    if (dcFile != null && value == dcFile.headerOffset + dcFile.length) {
                        return new Pointer(value, dcFile);
                    }
                }
            }
            return null;
        }

        public void OverwriteData(uint position, byte[] data) {
            Array.Copy(data, 0, this.data, position, data.Length);
        }

        public void OverwriteData(uint position, uint data) {
            OverwriteData(position, BitConverter.GetBytes(data));
        }
    }
}
