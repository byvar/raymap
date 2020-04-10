using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class PS1Data : FileWithPointers {
        string path;
        long length;
        byte[] data = null;

        public PS1Data(string name, string path, int fileID, uint baseAddress) : this(name, FileSystem.GetFileReadStream(path), fileID, baseAddress) {
            this.path = path;
        }

        public PS1Data(string name, Stream stream, int fileID, uint baseAddress) {
            allowUnsafePointers = true;
            this.name = name;
            this.fileID = fileID;
            length = stream.Length;
            headerOffset = baseAddress;
            baseOffset = -headerOffset;
            reader = new Reader(stream, Settings.s.IsLittleEndian);
        }

        public override void CreateWriter() {
            return; // No writing support for PS1Data files yet
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
            if (headerOffset != 0 && value >= headerOffset && value < headerOffset + length) {
                return new Pointer(value, this);
            } else {
                MapLoader l = MapLoader.Loader;
                foreach (FileWithPointers f in l.files_array) {
                    PS1Data ps1File = f as PS1Data;
                    if (ps1File != null && ps1File.headerOffset != 0 && value >= ps1File.headerOffset && value < ps1File.headerOffset + ps1File.length) {
                        return new Pointer(value, ps1File);
                    }
                }
                // Do a second loop over the files. If end and start overlap we want the start (returned by previous loop),
                // but without the second loop some valid pointers will be null
                foreach (FileWithPointers f in l.files_array) {
                    PS1Data ps1File = f as PS1Data;
                    if (ps1File != null && ps1File.headerOffset != 0 && value == ps1File.headerOffset + ps1File.length) {
                        return new Pointer(value, ps1File);
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
