using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class LinearFile : FileWithPointers {
        string path;
        long length;
        byte[] data = null;

        public LinearFile(string name, string path, int fileID, uint baseAddress = 0) : this(name, FileSystem.GetFileReadStream(path), fileID, baseAddress) {
            this.path = path;
        }

        public LinearFile(string name, Stream stream, int fileID, uint baseAddress = 0) {
            allowUnsafePointers = true;
            this.name = name;
            this.fileID = fileID;
            length = stream.Length;
            headerOffset = baseAddress;
            baseOffset = -headerOffset;
            reader = new Reader(stream, CPA_Settings.s.IsLittleEndian);
        }

        public override void CreateWriter() {
            return; // No writing support for PS1Data files yet
        }

        public override void WritePointer(LegacyPointer pointer) {
            if (writer != null) {
                if (pointer == null) {
                    writer.Write((uint)0);
                } else {
                    writer.Write(pointer.offset);
                }
            }
        }

        public override LegacyPointer GetUnsafePointer(uint value) {
            if (headerOffset != 0 && value >= headerOffset && value < headerOffset + length) {
                return new LegacyPointer(value, this);
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
