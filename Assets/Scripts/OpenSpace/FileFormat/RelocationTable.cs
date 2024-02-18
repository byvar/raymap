﻿using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using lzo.net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        RTB = 0, // SNA
        RTP = 1, // Pointer file (GPT)
        RTS = 2, // Sound
        RTT = 3, // Texture file (PTX)
        // ^ in Rayman 2 | v Not in Rayman 2
        RTL = 4,
        RTD = 5, // Language pointer file (DLG)
        RTG = 6, // Language-specific SNA blocks (lng)
        RTV = 7  // Video?
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
		RelocationType type;
		private DAT dat;
		string name;

        // Use this to automatically decide whether to load it from RT* file or from DAT
        public RelocationTable(string path, DAT dat, string name, RelocationType type) {
			this.name = name;
			this.type = type;
			this.dat = dat;
			/*string newPath = path;
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
                case RelocationType.RTD:
                    newPath = Path.ChangeExtension(path, "rtd"); break;
                case RelocationType.RTG:
                    newPath = Path.ChangeExtension(path, "rtg"); break;
                case RelocationType.RTV:
                    newPath = Path.ChangeExtension(path, "rtv"); break;
            }
            this.path = newPath;*/
			this.path = path;
        }

		public async UniTask Init() {
			if (FileSystem.FileExists(path)) {
				await Load(FileSystem.GetFileReadStream(path), false);
			} else if (dat != null) {
				await LoadFromDAT();
			}
		}

        private async UniTask Load(Stream stream, bool masking) {
            using (Reader reader = new Reader(stream, isLittleEndian)) {
                if (Legacy_Settings.s.encryptPointerFiles) {
                    reader.InitMask();
					//reader.InitWindowMask();
					/*byte [] data = reader.ReadBytes((int)stream.Length);
                    MapLoader.Loader.print(path);
                    Util.ByteArrayToFile(path + ".dmp", data);
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    reader.InitWindowMask();*/
				}
				await Read(reader);
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

        private async UniTask LoadFromDAT() {
            Reader reader = dat.reader;
			PartialHttpStream httpStream = reader.BaseStream as PartialHttpStream;
			
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
			await dat.GetOffset(rtref);
			uint offset = dat.lastOffset;
            //R3Loader.Loader.print("RtRef Post (" + rtref.levelId + "," + rtref.relocationType + ")");
            /*dat.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            reader.SetMask(mask);
            byte[] dataNew = reader.ReadBytes(1000000);
            Util.ByteArrayToFile(name + "_" + type + ".data", dataNew);*/
            dat.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			if (httpStream != null) await httpStream.FillCacheForRead(4);
			reader.SetMask(mask);
            reader.ReadUInt32();
            await Read(reader);
        }

        public RelocationPointerList GetListForPart(byte module, byte block) {
            for (int i = 0; i < pointerBlocks.Length; i++) {
                RelocationPointerList e = pointerBlocks[i];
                if (e.module == module && e.id == block) return e;
            }
            return null;
        }

        async UniTask Read(Reader reader) {
            MapLoader l = MapLoader.Loader;
			PartialHttpStream httpStream = reader.BaseStream as PartialHttpStream;

			if (httpStream != null) await httpStream.FillCacheForRead(5);
			byte count = reader.ReadByte();
            if (Legacy_Settings.s.game != Legacy_Settings.Game.R2Demo && Legacy_Settings.s.game != Legacy_Settings.Game.RedPlanet
                && Legacy_Settings.s.engineVersion > Legacy_Settings.EngineVersion.Montreal) {
                reader.ReadUInt32();
            }
            pointerBlocks = new RelocationPointerList[count];
            for (int i = 0; i < count; i++) {
                if (reader.BaseStream.Position >= reader.BaseStream.Length) {
                    Array.Resize(ref pointerBlocks, i);
                    break;
				}
				if (httpStream != null) await httpStream.FillCacheForRead(6);
				// A pointer list contains pointers located in SNA part with matching module & block
				pointerBlocks[i] = new RelocationPointerList();
                pointerBlocks[i].module = reader.ReadByte();
                pointerBlocks[i].id = reader.ReadByte();
                l.print("Parsing pointer block for (" + pointerBlocks[i].module + "," + pointerBlocks[i].id + ")");
                //l.print("Module: " + parts[i].module + " - Block: " + parts[i].block);
                pointerBlocks[i].count = reader.ReadUInt32();
				pointerBlocks[i].pointers = new RelocationPointerInfo[pointerBlocks[i].count];
                if (pointerBlocks[i].count > 0) {
                    if (Legacy_Settings.s.snaCompression) {
						if (httpStream != null) await httpStream.FillCacheForRead(5*4);
						uint isCompressed = reader.ReadUInt32();
                        uint compressedSize = reader.ReadUInt32();
                        uint compressedChecksum = reader.ReadUInt32();
                        uint decompressedSize = reader.ReadUInt32();
                        uint decompressedChecksum = reader.ReadUInt32();
						if (httpStream != null) await httpStream.FillCacheForRead((int)compressedSize);
						byte[] compressedData = reader.ReadBytes((int)compressedSize);
                        if (isCompressed != 0) {
                            using (var compressedStream = new MemoryStream(compressedData))
                            using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
                            using (Reader lzoReader = new Reader(lzo, Legacy_Settings.s.IsLittleEndian)) {
                                ReadPointerBlock(lzoReader, pointerBlocks[i]);
                            }
                        } else {
                            using (var uncompressedStream = new MemoryStream(compressedData))
                            using (Reader unCompressedReader = new Reader(uncompressedStream, Legacy_Settings.s.IsLittleEndian)) {
                                ReadPointerBlock(unCompressedReader, pointerBlocks[i]);
                            }
                        }
                    } else {
						if (httpStream != null) await httpStream.FillCacheForRead((int)pointerBlocks[i].count * 8);
						ReadPointerBlock(reader, pointerBlocks[i]);
                    }
                }
            }
        }

        private void ReadPointerBlock(Reader reader, RelocationPointerList pointerBlock) {
            for (int j = 0; j < pointerBlock.count; j++) {
                // One pointer info struct contains the address where the pointer is located and the module & block of the part it points to.
                // The address that it points to is to be read at address offsetInMemory.
                // The part's baseInMemory should be subtracted from it to get the offset relative to the part.
                pointerBlock.pointers[j] = new RelocationPointerInfo();
                pointerBlock.pointers[j].offsetInMemory = reader.ReadUInt32();
				pointerBlock.pointers[j].module = reader.ReadByte();
                pointerBlock.pointers[j].id = reader.ReadByte();
                if (Legacy_Settings.s.engineVersion > Legacy_Settings.EngineVersion.TT
					&& Legacy_Settings.s.mode != Legacy_Settings.Mode.PlaymobilLauraPC) {
                    pointerBlock.pointers[j].byte6 = reader.ReadByte();
                    pointerBlock.pointers[j].byte7 = reader.ReadByte();
                }
            }
        }
    }
}
