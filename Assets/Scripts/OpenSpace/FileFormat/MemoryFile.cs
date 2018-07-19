using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class MemoryFile : FileWithPointers {
        public MemoryFile(string name) {
            allowUnsafePointers = true;
            baseOffset = 0;
            headerOffset = 0;
            this.name = name;
            ProcessMemoryStream stream = new ProcessMemoryStream(name, ProcessMemoryStream.Mode.Read);
            MapLoader.Loader.print("Base addr: " + stream.BaseAddress);
            baseOffset = 0;
            stream.Seek(stream.BaseAddress, SeekOrigin.Begin);
            reader = new EndianBinaryReader(stream, Settings.s.IsLittleEndian);
        }
    }
}
