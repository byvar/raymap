using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class DSB : FileWithPointers {
        byte[] data = null;
        public DSB(string name, string path) : this(name, File.OpenRead(path)) { }

        public DSB(string name, Stream stream) {
            baseOffset = 0;
            headerOffset = 0;
            this.name = name;
            using (Reader encodedReader = new Reader(stream, Settings.s.IsLittleEndian)) {
                encodedReader.ReadMask();
                data = encodedReader.ReadBytes((int)stream.Length - 4);
            }
            reader = new Reader(new MemoryStream(data), Settings.s.IsLittleEndian);
        }

        public override void CreateWriter() {
            return; // Don't need to write to this file
        }

        public void Save(string path) {
            Util.ByteArrayToFile(path, data);
        }
    }
}
