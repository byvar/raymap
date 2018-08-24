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
        string path;
        bool isLittleEndian = true;

        // Use this to automatically decide whether to load it from RT* file or from DAT
        public RelocationTable(string path, DAT dat, string name, RelocationType type) {
            string newPath = path;
            switch (type) {
                case RelocationType.RTB:
                    newPath = Path.ChangeExtension(path, "rtb"); break;
                case RelocationType.RTP:
                    newPath = Path.ChangeExtension(path, "rtp"); break;
                case RelocationType.RTS:
                    newPath = Path.ChangeExtension(path, "rts"); break;
                case RelocationType.RTT:
                    newPath = Path.ChangeExtension(path, "rtt"); break;
                case RelocationType.RTL:
                    newPath = Path.ChangeExtension(path, "rtl"); break;
            }
            this.path = newPath;
            if (File.Exists(newPath)) {
                Load(File.OpenRead(newPath), false);
            } else if(dat != null) {
                Load(dat, name, type);
            }
        }

        public RelocationTable(string path) : this(File.OpenRead(path)) { }

        public RelocationTable(Stream stream, bool masking = false) {
            Load(stream, masking);
        }

        public RelocationTable(DAT dat, string name, RelocationType type) {
            Load(dat, name, type);
        }

        private void Load(Stream stream, bool masking) {
            using (Reader reader = new Reader(stream, isLittleEndian)) {
                if (Settings.s.useWindowMasking) {
                    reader.InitWindowMask();
                    /*byte [] data = reader.ReadBytes((int)stream.Length);
                    MapLoader.Loader.print(path);
                    Util.ByteArrayToFile(path + ".dmp", data);
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    reader.InitWindowMask();*/
                }
                Read(reader);
            }
        }

        // Used for Tonic Trouble's Fixlvl.rtb, which contains a list of fix->lvl pointers that should be loaded along with the fix.rtb file
        public void Add(RelocationTable rt) {
            if (rt == null) return;
            for (int i = 0; i < rt.pointerBlocks.Length; i++) {
                RelocationPointerList ptrList = GetListForPart(rt.pointerBlocks[i].module, rt.pointerBlocks[i].id);
                if (ptrList == null) {
                    Array.Resize(ref pointerBlocks, pointerBlocks.Length + 1);
                    pointerBlocks[pointerBlocks.Length - 1] = rt.pointerBlocks[i];
                } else {
                    ptrList.count += rt.pointerBlocks[i].count;
                    Array.Resize(ref ptrList.pointers, (int)ptrList.count);
                    Array.Copy(rt.pointerBlocks[i].pointers, 0, ptrList.pointers, ptrList.count - rt.pointerBlocks[i].count, rt.pointerBlocks[i].count);
                }
            }
        }

        private void Load(DAT dat, string name, RelocationType type) {
            Reader reader = dat.reader;
            int levelIndex = 0;

            for (int i = 0; i < dat.gameDsb.levels.Count; i++) {
                if (dat.gameDsb.levels[i].ToLower().Equals(name.ToLower())) {
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

        void Read(Reader reader) {
            MapLoader l = MapLoader.Loader;

            byte count = reader.ReadByte();
            if (Settings.s.game != Settings.Game.R2Demo && Settings.s.engineVersion > Settings.EngineVersion.TT) {
                reader.ReadUInt32();
            }
            pointerBlocks = new RelocationPointerList[count];
            for (int i = 0; i < count; i++) {
                if (reader.BaseStream.Position >= reader.BaseStream.Length) {
                    Array.Resize(ref pointerBlocks, i);
                    break;
                }
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
                    pointerBlocks[i].pointers[j].module = reader.ReadByte();
                    pointerBlocks[i].pointers[j].id = reader.ReadByte();
                    if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                        pointerBlocks[i].pointers[j].byte6 = reader.ReadByte();
                        pointerBlocks[i].pointers[j].byte7 = reader.ReadByte();
                    }
                }
            }
        }
    }
}
