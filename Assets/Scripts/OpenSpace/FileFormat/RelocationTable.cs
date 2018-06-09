using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class RelocationPointerList {
        public byte module;
        public byte id;
        public uint count;
        public RelocationPointerInfo[] pointers;
    }
    public class RelocationPointerInfo {
        public uint offsetInMemory;
        public byte module;
        public byte id;
        public byte byte6;
        public byte byte7;
    }

    public enum RelocationType {
        RTB = 0,
        RTP = 1,
        RTS = 2,
        RTT = 3,
        RTL = 4
    }

    public class RelocationTableReference {
        public byte levelId;
        public byte relocationType;
        public byte byte2;
        public byte byte3;
        public RelocationTableReference() { }
        public RelocationTableReference(byte levelId, byte relocationType) {
            this.levelId = levelId;
            this.relocationType = relocationType;
            byte2 = 0;
            byte3 = 0;
        }
    }

    public class RelocationTable {
        public RelocationPointerList[] pointerBlocks;

        bool isLittleEndian = true;

        public RelocationTable(string path) : this(File.OpenRead(path)) { }

        public RelocationTable(Stream stream, bool masking = false) {
            using (EndianBinaryReader reader = new EndianBinaryReader(stream, isLittleEndian)) {
                Read(reader);
            }
        }

        public RelocationTable(DAT dat, string name, RelocationType type) {
            EndianBinaryReader reader = dat.reader;
            int levelIndex = 0;
            string[] levelList = DAT.levelList;
            for (int i = 0; i < levelList.Length; i++) {
                if (levelList[i].ToLower().Equals(name.ToLower())) {
                    levelIndex = i;
                    break;
                }
            }
            RelocationTableReference rtref = new RelocationTableReference((byte)levelIndex, (byte)type);
            //R3Loader.Loader.print("RtRef Pre  (" + rtref.levelId + "," + rtref.relocationType + ")");
            uint mask = dat.GetMask(rtref);
            uint offset = dat.GetOffset(rtref);
            //R3Loader.Loader.print("RtRef Post (" + rtref.levelId + "," + rtref.relocationType + ")");
            /*dat.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            reader.SetMask(mask);
            byte[] dataNew = reader.ReadBytes(1000000);
            Util.ByteArrayToFile(name + "_" + type + ".data", dataNew);*/
            dat.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            reader.SetMask(mask);
            reader.ReadUInt32();
            Read(reader);
        }

        public RelocationPointerList GetListForPart(byte module, byte block) {
            for (int i = 0; i < pointerBlocks.Length; i++) {
                RelocationPointerList e = pointerBlocks[i];
                if (e.module == module && e.id == block) return e;
            }
            return null;
        }

        void Read(EndianBinaryReader reader) {
            MapLoader l = MapLoader.Loader;

            byte count = reader.ReadByte();
            reader.ReadUInt32();
            pointerBlocks = new RelocationPointerList[count];
            for (int i = 0; i < count; i++) {
                // A pointer list contains pointers located in SNA part with matching module & block
                pointerBlocks[i] = new RelocationPointerList();
                pointerBlocks[i].module = reader.ReadByte();
                pointerBlocks[i].id = reader.ReadByte();
                l.print("Parsing pointer block for (" + pointerBlocks[i].module + "," + pointerBlocks[i].id + ")");
                //l.print("Module: " + parts[i].module + " - Block: " + parts[i].block);
                pointerBlocks[i].count = reader.ReadUInt32();
                pointerBlocks[i].pointers = new RelocationPointerInfo[pointerBlocks[i].count];
                for (int j = 0; j < pointerBlocks[i].count; j++) {
                    // One pointer info struct contains the address where the pointer is located and the module & block of the part it points to.
                    // The address that it points to is to be read at address offsetInMemory.
                    // The part's baseInMemory should be subtracted from it to get the offset relative to the part.
                    pointerBlocks[i].pointers[j] = new RelocationPointerInfo();
                    pointerBlocks[i].pointers[j].offsetInMemory = reader.ReadUInt32();
                    if (pointerBlocks[i].pointers[j].offsetInMemory == 1824756 || pointerBlocks[i].pointers[j].offsetInMemory == 1824748) {
                        l.print("Found! It's (" + pointerBlocks[i].module + "," + pointerBlocks[i].id + ")");
                    }
                    pointerBlocks[i].pointers[j].module = reader.ReadByte();
                    pointerBlocks[i].pointers[j].id = reader.ReadByte();
                    pointerBlocks[i].pointers[j].byte6 = reader.ReadByte();
                    pointerBlocks[i].pointers[j].byte7 = reader.ReadByte();
                }
            }
        }
    }
}
