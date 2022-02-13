using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class LVL : FileWithPointers {
        string path;
		private byte[] overrideData;

        public LVL(string name, string path, int fileID) : this(name, FileSystem.GetFileReadStream(path), fileID) {
            this.path = path;
        }

        public LVL(string name, Stream stream, int fileID) {
            baseOffset = 4;
            headerOffset = 0;
            this.name = name;
            this.fileID = fileID;
            reader = new Reader(stream, CPA_Settings.s.IsLittleEndian);
        }

        public void ReadPTR(string path) {
            if (!FileSystem.FileExists(path)) return;
            Stream ptrStream = FileSystem.GetFileReadStream(path);
            long totalSize = ptrStream.Length;
            using (Reader ptrReader = new Reader(ptrStream, CPA_Settings.s.IsLittleEndian)) {
				uint num_ptrs;
				if (CPA_Settings.s.game == CPA_Settings.Game.LargoWinch) {
					num_ptrs = (uint)totalSize / 8;
				} else {
					num_ptrs = ptrReader.ReadUInt32();
				}
                for (uint j = 0; j < num_ptrs; j++) {
                    int file = ptrReader.ReadInt32();
                    uint ptr_ptr = ptrReader.ReadUInt32();
                    reader.BaseStream.Seek(ptr_ptr + baseOffset, SeekOrigin.Begin);
                    uint ptr = reader.ReadUInt32();
                    pointers[ptr_ptr] = new LegacyPointer(ptr, GetFileWithID(file));
				}
				if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
					// Revolution
					ushort num_specialBlocks = ptrReader.ReadUInt16();
					for (uint j = 0; j < num_specialBlocks; j++) {
						ptrReader.ReadUInt16();
					}
					extraData["numLevels"] = ptrReader.ReadByte();
					if (fileID == MapLoader.Mem.Fix) {
						// Level list
						string[] levels = new string[(byte)extraData["numLevels"]];
						for (int i = 0; i < levels.Length; i++) {
							levels[i] = ptrReader.ReadString(0x1E);
						}
						extraData["levels"] = levels;
					} else if(fileID == MapLoader.Mem.Lvl) {
						// Level data
						ptrReader.ReadByte();
						ptrReader.ReadUInt32();
						ptrReader.ReadUInt32();
						ptrReader.ReadUInt32();
						extraData["numMeshes"] = ptrReader.ReadUInt32();
						extraData["numMaterials"] = ptrReader.ReadUInt32();
						extraData["numTextures"] = ptrReader.ReadUInt32();
						ptrReader.ReadUInt32();
						ptrReader.ReadUInt32();
						extraData["levelIndex"] = ptrReader.ReadUInt32();
						uint num_specialPtrs = ptrReader.ReadUInt32();
						for (uint j = 0; j < num_specialPtrs; j++) {
							ptrReader.ReadUInt32(); // points to a struct of size 0x4 that looks like { 0600 3E00 }
						}
						ptrReader.ReadUInt32();
						extraData["numLightmappedObjects"] = ptrReader.ReadUInt32();
					}
					ptrReader.ReadByte();
				} else {
					// Rayman 3
					long num_fillInPtrs = (totalSize - ptrStream.Position) / 16;
					for (uint j = 0; j < num_fillInPtrs; j++) {
						uint ptr_ptr = ptrReader.ReadUInt32(); // the address the pointer should be located at
						int src_file = ptrReader.ReadInt32(); // the file the pointer should be located in
						uint ptr = ptrReader.ReadUInt32();
						int target_file = ptrReader.ReadInt32();
						GetFileWithID(src_file).pointers[ptr_ptr] = new LegacyPointer(ptr, GetFileWithID(target_file)); // can overwrite if necessary
					}
				}
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            ptrStream.Close();
        }

        public FileWithPointers GetFileWithID(int id) {
            return MapLoader.Loader.files_array.FirstOrDefault(f => f != null && f.fileID == id);
        }
        
        public override void CreateWriter() {
            if (path != null) {
                FileStream stream = new FileStream(path, FileMode.Open);
                writer = new Writer(stream, CPA_Settings.s.IsLittleEndian);
            }
        }

        public override void WritePointer(LegacyPointer pointer) {
			UnityEngine.Debug.LogWarning("Pointer writing not supported!");
			writer.BaseStream.Position += 4;
		}
		public void OverrideData(byte[] data) {
			baseOffset = 0;
			overrideData = data;
			reader.Close();
			reader = new Reader(new MemoryStream(overrideData), CPA_Settings.s.IsLittleEndian);
		}
	}
}
