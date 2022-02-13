using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class ROMBIN : FileWithPointers {
        string path;
        long length;
        byte[] data = null;
		private FileWithPointers pointerFile;

        public ROMBIN(string name, string path, int fileID) : this(name, FileSystem.GetFileReadStream(path), fileID) {
            this.path = path;
        }

        public ROMBIN(string name, Stream stream, int fileID) {
            allowUnsafePointers = true;
            this.name = name;
            this.fileID = fileID;
            length = stream.Length;
            using (Reader fileReader = new Reader(stream, CPA_Settings.s.IsLittleEndian)) {
                data = fileReader.ReadBytes((int)stream.Length);
            }
            reader = new Reader(new MemoryStream(data), CPA_Settings.s.IsLittleEndian);
			headerOffset = 0;
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

        public override void WritePointer(LegacyPointer pointer) {
            if (writer != null) {
                if (pointer == null) {
                    writer.Write((uint)0);
                } else {
                    writer.Write(pointer.offset);
                }
            }
        }

		public void WithPointerFile(FileWithPointers f, Action action) {
			pointerFile = f;
			action();
			pointerFile = null;
		}

        public override LegacyPointer GetUnsafePointer(uint value) {
            if (value >= headerOffset && value < headerOffset + length) {
                return new LegacyPointer(value, pointerFile != null ? pointerFile : this);
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
