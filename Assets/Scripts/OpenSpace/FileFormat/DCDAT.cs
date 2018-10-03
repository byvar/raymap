using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class DCDAT : FileWithPointers {
        string path;
        long length;

        public DCDAT(string name, string path, int fileID) : this(name, File.OpenRead(path), fileID) {
            this.path = path;
        }

        public DCDAT(string name, Stream stream, int fileID) {
            allowUnsafePointers = true;
            baseOffset = 0;
            headerOffset = 0;
            this.name = name;
            this.fileID = fileID;
            length = stream.Length;
            reader = new Reader(stream, Settings.s.IsLittleEndian);
            switch (fileID) {
                case 0:
                    stream.Seek(0x10, SeekOrigin.Begin);
                    headerOffset = reader.ReadUInt32();
                    break;
                case 1:
                    stream.Seek(0x94, SeekOrigin.Begin);
                    headerOffset = reader.ReadUInt32();
                    break;
            }
            baseOffset = -headerOffset;
            stream.Seek(0, SeekOrigin.Begin);
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
            }
            return null;
        }
    }
}
