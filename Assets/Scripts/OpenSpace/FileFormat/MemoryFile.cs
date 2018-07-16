using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class MemoryFile : FileWithPointers {
        public MemoryFile(string name) {
            baseOffset = 0;
            headerOffset = 0;
            this.name = name;
            ProcessMemoryStream stream = new ProcessMemoryStream(name, ProcessMemoryStream.Mode.Read);
            baseOffset = stream.BaseAddress;
            reader = new EndianBinaryReader(stream, Settings.s.IsLittleEndian);
        }
    }
}
