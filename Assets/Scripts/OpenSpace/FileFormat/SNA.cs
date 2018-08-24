using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    public class SNAMemoryBlock {
        public SNA sna;
        public byte module;
        public byte id;
        public byte unk1;
        public int baseInMemory;

        public uint unk2;
        public uint unk3;
        public uint maxPosMinus9;
        public uint size;

        public uint position;
        public uint dataPosition;
        public RelocationPointerList pointerList;
        public bool isGpt = false;
        public bool relocateLater = false;
    }

    public class SNA : FileWithPointers {
        string path;
        List<SNAMemoryBlock> blocks = new List<SNAMemoryBlock>();
        public Dictionary<ushort, SNAMemoryBlock> relocation_local = new Dictionary<ushort, SNAMemoryBlock>();
        byte[] data = null;
        RelocationTable rtb;
        RelocationTable rtp;
        RelocationTable rtt;
        SNAMemoryBlock gpt;
        SNAMemoryBlock ptx;
        int tmpModule = 10;

        public SNA(string name, string path, RelocationTable rtb) : this(name, File.OpenRead(path), rtb) {
            this.path = path;
        }

        public SNA(string name, Stream stream, RelocationTable rtb) {
            baseOffset = 0; // we're skipping the first 4 bytes for this one.
            headerOffset = 0;
            this.name = name;
            this.rtb = rtb;
            using (Reader encodedReader = new Reader(stream, Settings.s.IsLittleEndian)) {
                if (Settings.s.useWindowMasking) {
                    encodedReader.InitWindowMask();
                    data = encodedReader.ReadBytes((int)stream.Length);
                } else {
                    encodedReader.ReadMask();
                    data = encodedReader.ReadBytes((int)stream.Length - 4);
                }
            }
            reader = new Reader(new MemoryStream(data), Settings.s.IsLittleEndian);
            ReadSNA();
        }

        void ReadSNA() {
            MapLoader l = MapLoader.Loader;
            uint szCounter = 0;
            if (Settings.s.engineVersion == Settings.EngineVersion.TT) {
                byte headerLength = reader.ReadByte();
                reader.ReadBytes(headerLength);
            }
            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                SNAMemoryBlock block = new SNAMemoryBlock();
                block.sna = this;
                block.position = (uint)reader.BaseStream.Position;
                block.module = reader.ReadByte();
                block.id = reader.ReadByte();
                if (Settings.s.engineVersion > Settings.EngineVersion.TT) block.unk1 = reader.ReadByte();
                block.baseInMemory = reader.ReadInt32();
                if (blocks.Count == 0) {
                    l.print("Initial block: " + block.module + "|" + block.id + " - base: " + block.baseInMemory);
                }

                // Initialize part
                block.size = 0;
                block.pointerList = null;

                if (block.baseInMemory != -1) {
                    block.unk2 = reader.ReadUInt32();
                    block.unk3 = reader.ReadUInt32();
                    block.maxPosMinus9 = reader.ReadUInt32();
                    block.size = reader.ReadUInt32();
                    szCounter += block.size;
                    if (Settings.s.engineVersion <= Settings.EngineVersion.TT) block.unk1 = reader.ReadByte();
                    block.dataPosition = (uint)reader.BaseStream.Position;
                    reader.ReadBytes((int)block.size);
                    block.pointerList = rtb.GetListForPart(block.module, block.id);
                    l.print("(" + block.module + "," + block.id + ") Base: " + block.baseInMemory + " - Size: " + block.size + " - 2:" + block.unk2 + " - 3:" + block.unk3 + " - 4:" + block.maxPosMinus9);
                    ushort ptrRelocationKey = GetRelocationKey(block);
                    if (l.relocation_global.ContainsKey(ptrRelocationKey)) {
                        if (block.size != 0) l.relocation_global[ptrRelocationKey] = block;
                    } else l.relocation_global[ptrRelocationKey] = block;
                    if (relocation_local.ContainsKey(ptrRelocationKey)) {
                        if (block.size != 0) relocation_local[ptrRelocationKey] = block;
                    } else relocation_local[ptrRelocationKey] = block;
                }
                blocks.Add(block);
            }
            /*for (int i = 0; i < rtb.pointerBlocks.Length; i++) {
                RelocationPointerList list = rtb.pointerBlocks[i];
                ushort ptrRelocationKey = GetRelocationKey(list);
                if (l.ptr_relocation.ContainsKey(ptrRelocationKey)) {
                    if (block.size != 0) {
                        l.ptr_relocation[ptrRelocationKey] = block;
                    } else {
                        l.ptr_relocation[ptrRelocationKey].baseInMemory = block.baseInMemory;
                        l.ptr_relocation[ptrRelocationKey].maxPosMinus9 = block.maxPosMinus9;
                    }
                    //l.print("Overwriting relocation with key " + block.module + "|" + block.id + " - new base: " + block.baseInMemory);
                }
            }*/
            //l.print("Size total: " + szCounter);
        }

        public void ReadGPT(string path, RelocationTable rtp) {
            this.rtp = rtp;
            Stream gptStream = File.OpenRead(path);
            uint gptOffset = (uint)data.Length;
            byte[] gptData = null;
            using (Reader gptReader = new Reader(gptStream, Settings.s.IsLittleEndian)) {
                if (Settings.s.useWindowMasking) gptReader.InitWindowMask();
                gptData = gptReader.ReadBytes((int)gptStream.Length);
            }
            //Util.ByteArrayToFile(path + ".dmp", gptData);
            data = data.Concat(gptData).ToArray(); //Array.Resize(ref data, (int)(data.Length + gptData.Length));
            reader.Close();
            reader = new Reader(new MemoryStream(data), Settings.s.IsLittleEndian);
            if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                ushort ptrRelocationKey = GetRelocationKey(rtp.pointerBlocks[0]);
                SNAMemoryBlock block = relocation_local[ptrRelocationKey];
                block.pointerList = null;
                block.dataPosition = gptOffset;
                block.size = (uint)gptData.Length;
                block.isGpt = true;
                gpt = block;
            } else {
                // Tonic Trouble LVL
                SNAMemoryBlock block = new SNAMemoryBlock();
                block.pointerList = null;
                block.dataPosition = gptOffset;
                block.size = (uint)gptData.Length;
                block.isGpt = true;
                gpt = block;
            }
            headerOffset = gpt.dataPosition;
            //R3Loader.Loader.print("Base " + block.baseInMemory + " - Size: " + block.size);
        }

        public void ReadPTX(string path, RelocationTable rtt) {
            this.rtt = rtt;
            Stream ptxStream = File.OpenRead(path);
            uint ptxOffset = (uint)data.Length;
            byte[] ptxData = null;
            using (Reader ptxReader = new Reader(ptxStream, Settings.s.IsLittleEndian)) {
                if (Settings.s.useWindowMasking) ptxReader.InitWindowMask();
                ptxData = ptxReader.ReadBytes((int)ptxStream.Length);
            }
            //Util.ByteArrayToFile(path + ".dmp", ptxData);
            data = data.Concat(ptxData).ToArray(); //Array.Resize(ref data, (int)(data.Length + gptData.Length));
            reader.Close();
            reader = new Reader(new MemoryStream(data), Settings.s.IsLittleEndian);
            ptx = new SNAMemoryBlock();
            if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                // It doesn't exist in Tonic Trouble
                ushort ptrRelocationKey = GetRelocationKey(rtt.pointerBlocks[0]);
                SNAMemoryBlock block = relocation_local[ptrRelocationKey];
                ptx.baseInMemory = block.baseInMemory;
                ptx.id = block.id;
                ptx.module = block.module;
                ptx.maxPosMinus9 = block.maxPosMinus9;
                ptx.pointerList = rtt.pointerBlocks[0];
            }
            ptx.dataPosition = ptxOffset;
            ptx.position = ptxOffset;
            ptx.size = (uint)ptxData.Length;
            ptx.sna = this;
            //R3Loader.Loader.print("Base " + block.baseInMemory + " - Size: " + block.size);
        }

        public void CreatePointers() {
            MapLoader l = MapLoader.Loader;
            foreach (RelocationPointerList ptrList in rtb.pointerBlocks) {
                ushort ptrListRelocationKey = GetRelocationKey(ptrList);
                if (l.relocation_global.ContainsKey(ptrListRelocationKey)) {
                    SNAMemoryBlock block_local = relocation_local[ptrListRelocationKey];
                    SNAMemoryBlock block_global = (block_local.size != 0) ? block_local : l.relocation_global[ptrListRelocationKey];
                    Reader cur_reader = block_global.sna.reader;
                    if (block_global.size > 0) {
                        foreach (RelocationPointerInfo info in ptrList.pointers) {
                            uint relativeAddress = info.offsetInMemory - (uint)block_local.baseInMemory;
                            cur_reader.BaseStream.Seek(block_global.dataPosition + relativeAddress, SeekOrigin.Begin);
                            uint ptrValue = cur_reader.ReadUInt32();
                            
                            ushort ptrRelocationKey = GetRelocationKey(info);
                            if (!l.relocation_global.ContainsKey(ptrRelocationKey)) {
                                if (Settings.s.engineVersion > Settings.EngineVersion.TT || !(info.module == 0xFF && info.id == 0xFF)) {
                                    l.print("Could not find SNA block (" + info.module + "," + info.id + ")");
                                }
                            } else {
                                SNAMemoryBlock ptr_block_local = relocation_local[ptrRelocationKey];
                                SNAMemoryBlock ptr_block_global = (ptr_block_local.size != 0) ? ptr_block_local : l.relocation_global[ptrRelocationKey];
                                if (ptr_block_global != null && ptr_block_local != null && ptr_block_local.baseInMemory != -1) {

                                    /*if (info.module == 0xA) {
                                        l.print("Special pointer in (" + ptrList.module + "," + ptrList.id + ") with offset " + info.offsetInMemory +
                                            "(Relative:" + relativeAddress + " - Datafile:" + (block_global.dataPosition + relativeAddress) + ")"
                                            );
                                        l.print("Value: " + ptrValue + " - Post relocation: " + (ptrValue - ptr_block_local.baseInMemory + ptr_block_global.dataPosition));
                                    }*/

                                    /*if (ptrValue < ptr_block_local.baseInMemory) {
                                        l.print("Info mod: " + info.module + " - block " + info.id);
                                        l.print("PtrList mod: " + ptrList.module + " - block " + ptrList.id);
                                        l.print("Post relocation: " + (ptrValue - ptr_block_local.baseInMemory + ptr_block_global.dataPosition));
                                        l.print("Too low: " + ptrValue + " < " + ptr_block_local.baseInMemory + " + " + ptr_block_global.size);
                                        l.print("Part 2:" + ptr_block_local.unk2 + " - 3:" + ptr_block_local.unk3 + " - 4:" + ptr_block_local.maxPosMinus9);
                                        l.print("Info off:" + info.offsetInMemory + " - 6:" + info.byte6 + " - 7:" + info.byte7);
                                        //return;
                                    }
                                    if (ptrValue >= ptr_block_local.maxPosMinus9 + 9) {
                                    //if (ptrValue >= ptr_block_local.baseInMemory + ptr_block_global.size) {
                                        //l.print("Part mod: " + block.module + " - block " + block.id);
                                        l.print("Info mod: " + info.module + " - block " + info.id);
                                        l.print("PtrList mod: " + ptrList.module + " - block " + ptrList.id);
                                        l.print("Post relocation: " + (ptrValue - ptr_block_local.baseInMemory + ptr_block_global.dataPosition));
                                        l.print("Post relocation 2: " + (ptrValue - ptr_block_local.baseInMemory));
                                        l.print("Too high: " + ptrValue + " < " + ptr_block_local.baseInMemory + " + " + ptr_block_global.size);
                                        l.print("Part 2:" + ptr_block_local.unk2 + " - 3:" + ptr_block_local.unk3 + " - 4:" + ptr_block_local.maxPosMinus9);
                                        l.print("Info off:" + info.offsetInMemory + " - 6:" + info.byte6 + " - 7:" + info.byte7);
                                        //return;
                                    }*/
                                    uint ptrValuePrior = ptrValue;
                                    ptrValue -= (uint)ptr_block_local.baseInMemory;
                                    if (info.module != tmpModule) ptrValue += ptr_block_global.dataPosition;
                                    // Test. There's a fillin pointer there
                                    /*if (ptrValue == 0x1b6a4) {
                                        l.print("Fillin pointer is located in block (" + block_global.module + "," + block_global.id + ") with base " + block_local.baseInMemory);
                                        l.print("Its offset prior to relocation is " + (info.offsetInMemory));
                                        l.print(info.byte6 + " - " + info.byte7);
                                        l.print("Local base: " + ptr_block_local.baseInMemory + " - " + ptr_block_local.unk1 + " - " + ptr_block_local.unk2 + " - " + ptr_block_local.unk3);
                                        l.print("Its offset in the data file is " + (block_global.dataPosition + relativeAddress));
                                        l.print("Fillin pointer points to block (" + ptr_block_global.module + "," + ptr_block_global.id + ") with base " + ptr_block_global.baseInMemory);
                                        l.print("Its value prior to relocation is " + ptrValuePrior + " - minus base: " + (ptrValuePrior - ptr_block_local.baseInMemory));
                                    }*/
                                    Pointer pointer = new Pointer(ptrValue, ptr_block_global.sna);
                                    block_global.sna.pointers[block_global.dataPosition + relativeAddress] = pointer;
                                } else {
                                    l.print("Pointer error: SNA part (" + info.module + "," + info.id + ") not found.");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            RelocatePointerFile(gpt, rtp); // Now for the Global Pointer Table
            RelocatePointerFile(ptx, rtt); // Now for the PTX
            GotoHeader();

        }

        private void RelocatePointerFile(SNAMemoryBlock pf, RelocationTable rt) {
            MapLoader l = MapLoader.Loader;
            reader.BaseStream.Seek(pf.dataPosition, SeekOrigin.Begin);
            if (rt != null) {
                foreach (RelocationPointerList ptrList in rt.pointerBlocks) {
                    int listIndex = 0;
                    for (uint i = 0; i < pf.size / 4; i++) {
                        uint ptrValue = reader.ReadUInt32();
                        if (ptrList.pointers[listIndex].offsetInMemory == ptrValue) {
                            RelocationPointerInfo info = ptrList.pointers[listIndex];
                            ushort ptrRelocationKey = GetRelocationKey(info);
                            SNAMemoryBlock ptr_block_local = relocation_local[ptrRelocationKey];
                            SNAMemoryBlock ptr_block_global = (ptr_block_local.size != 0) ? ptr_block_local : l.relocation_global[ptrRelocationKey];
                            if (ptr_block_global != null && ptr_block_local != null && ptr_block_local.baseInMemory != -1) {
                                ptrValue -= (uint)ptr_block_local.baseInMemory;
                                if (info.module != tmpModule) ptrValue += ptr_block_global.dataPosition;
                                Pointer pointer = new Pointer(ptrValue, ptr_block_global.sna);
                                pointers[pf.dataPosition + (i * 4)] = pointer;
                            } else {
                                l.print("Pointer error: SNA part (" + info.module + "," + info.id + ") not found.");
                            }
                            listIndex++;
                            if (listIndex >= ptrList.pointers.Length) break;
                        }
                    }
                }
            } else {
                // Tonic Trouble has rt=null for the levels, so let's go with a little hack
                for (uint i = 0; i < pf.size / 4; i++) {
                    uint ptrValue = reader.ReadUInt32();
                    foreach (SNAMemoryBlock block in MapLoader.Loader.relocation_global.Values) {
                        if (ptrValue > block.baseInMemory && ptrValue < block.baseInMemory + block.size) {
                            ptrValue -= (uint)block.baseInMemory;
                            ptrValue += block.dataPosition;
                            Pointer pointer = new Pointer(ptrValue, block.sna);
                            pointers[pf.dataPosition + (i * 4)] = pointer;
                        }
                    }
                }
            }
        }

        ushort GetRelocationKey(byte module, byte block) {
            return (ushort)(module * 0x100 + block); // Originally this is 10 (to save space), but 0x100 makes more sense since they're two bytes
        }
        ushort GetRelocationKey(RelocationPointerList list) { // The list of pointers for a certain SNA block
            return GetRelocationKey(list.module, list.id);
        }
        ushort GetRelocationKey(RelocationPointerInfo info) { // Details about a pointer to a certain block
            return GetRelocationKey(info.module, info.id);
        }
        ushort GetRelocationKey(SNAMemoryBlock block) { // A SNA block
            return GetRelocationKey(block.module, block.id);
        }
        
        public void CreateMemoryDump(string path, bool replacePointers) {
            if (replacePointers) {
                byte[] replacedData = new byte[data.Length];
                Array.Copy(data, replacedData, data.Length);
                foreach (KeyValuePair<uint, Pointer> ptrKeyVal in pointers) {
                    if (ptrKeyVal.Value == null) continue;
                    uint offset = ptrKeyVal.Key;
                    if (offset < replacedData.Length) {
                        Array.Copy(BitConverter.GetBytes(ptrKeyVal.Value.offset), 0, replacedData, offset, 4);
                    }
                }
                Util.ByteArrayToFile(path, replacedData);
            } else {
                Util.ByteArrayToFile(path, data);
            }
        }

        public void OverwriteData(uint position, byte[] data) {
            Array.Copy(data, 0, this.data, position, data.Length);
        }

        public void OverwriteData(uint position, uint data) {
            OverwriteData(position, BitConverter.GetBytes(data));
        }

        public void GotoPTX() {
            if (reader != null) {
                reader.BaseStream.Seek(ptx.dataPosition, SeekOrigin.Begin);
            }
        }

        public override void CreateWriter() {
            return; // No writing support for SNA files yet
        }

        public override void WritePointer(Pointer pointer) {
            throw new NotImplementedException();
        }
    }
}
