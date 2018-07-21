using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class LVL : FileWithPointers {
        string path;

        public LVL(string name, string path, int fileID) : this(name, File.OpenRead(path), fileID) {
            this.path = path;
        }

        public LVL(string name, Stream stream, int fileID) {
            baseOffset = 4;
            headerOffset = 0;
            this.name = name;
            this.fileID = fileID;
            reader = new EndianBinaryReader(stream, Settings.s.IsLittleEndian);
        }

        public void ReadPTR(string path) {
            if (!File.Exists(path)) return;
            Stream ptrStream = File.OpenRead(path);
            long totalSize = ptrStream.Length;
            using (EndianBinaryReader ptrReader = new EndianBinaryReader(ptrStream, Settings.s.IsLittleEndian)) {
                uint num_ptrs = ptrReader.ReadUInt32();
                for (uint j = 0; j < num_ptrs; j++) {
                    int file = ptrReader.ReadInt32();
                    uint ptr_ptr = ptrReader.ReadUInt32();
                    reader.BaseStream.Seek(ptr_ptr + baseOffset, SeekOrigin.Begin);
                    uint ptr = reader.ReadUInt32();
                    pointers[ptr_ptr] = new Pointer(ptr, GetFileWithID(file));
                }
                long num_fillInPtrs = (totalSize - ptrStream.Position) / 16;
                for (uint j = 0; j < num_fillInPtrs; j++) {
                    uint ptr_ptr = ptrReader.ReadUInt32(); // the address the pointer should be located at
                    int src_file = ptrReader.ReadInt32(); // the file the pointer should be located in
                    uint ptr = ptrReader.ReadUInt32();
                    int target_file = ptrReader.ReadInt32();
                    GetFileWithID(src_file).pointers[ptr_ptr] = new Pointer(ptr, GetFileWithID(target_file)); // can overwrite if necessary
                }
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            ptrStream.Close();
        }

        public FileWithPointers GetFileWithID(int id) {
            return MapLoader.Loader.files_array.Where(f => f != null && f.fileID == id).FirstOrDefault();
        }
        
        public override void CreateWriter() {
            if (path != null) {
                FileStream stream = new FileStream(path, FileMode.Open);
                writer = new EndianBinaryWriter(stream, Settings.s.IsLittleEndian);
            }
        }
    }
}
