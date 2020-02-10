using lzo.net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        RelocationTable rtd;
        SNAMemoryBlock gpt;
        SNAMemoryBlock ptx;
        SNAMemoryBlock sda;
        SNAMemoryBlock dlg;
        int tmpModule = 10;

        public SNA(string name, string path, RelocationTable rtb) : this(name, FileSystem.GetFileReadStream(path), rtb) {
            this.path = path;
        }

        public SNA(string name, Stream stream, RelocationTable rtb) {
            baseOffset = 0; // we're skipping the first 4 bytes for this one.
            headerOffset = 0;
            this.name = name;
            this.rtb = rtb;
            using (Reader encodedReader = new Reader(stream, Settings.s.IsLittleEndian)) {
                int maskBytes = encodedReader.InitMask();
                data = encodedReader.ReadBytes((int)stream.Length - maskBytes);
            }
            reader = new Reader(new MemoryStream(data), Settings.s.IsLittleEndian);
            ReadSNA();
        }

        public void WriteSNA()
        {
            if (Settings.s.game == Settings.Game.TT)
            {
                throw new NotImplementedException();
                //byte headerLength = reader.ReadByte();
                //reader.ReadBytes(headerLength);
            }
            foreach (SNAMemoryBlock block in blocks)
            {
                writer.Write(block.module);
                writer.Write(block.id);
                if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) writer.Write(block.unk1);
                writer.Write(block.baseInMemory);

                if (block.baseInMemory != -1)
                {
                    writer.Write(block.unk2);
                    writer.Write(block.unk3);
                    writer.Write(block.maxPosMinus9);
                    writer.Write(block.size);
                    if (Settings.s.game == Settings.Game.TT) writer.Write(block.unk1);

                    WriteSNABlock(block);
                }
            }
        }

        void ReadSNA() {
            MapLoader l = MapLoader.Loader;
            uint szCounter = 0;
            if (Settings.s.game == Settings.Game.TT) {
                byte headerLength = reader.ReadByte();
                reader.ReadBytes(headerLength);
            }
            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                SNAMemoryBlock block = new SNAMemoryBlock();
                block.sna = this;
                block.position = (uint)reader.BaseStream.Position;
                block.module = reader.ReadByte();
                block.id = reader.ReadByte();
                if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) block.unk1 = reader.ReadByte();
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
                    if (Settings.s.game == Settings.Game.TT) block.unk1 = reader.ReadByte();
                    block.dataPosition = (uint)reader.BaseStream.Position;
                    //l.print("(" + block.module + "," + block.id + ") Base: " + block.baseInMemory + " - Size: " + block.size + " - 2:" + block.unk2 + " - 3:" + block.unk3 + " - 4:" + block.maxPosMinus9);

                    ReadSNABlock(block);
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

        public void AddSNA(SNA sna) {
            sna.Dispose();
            uint snaOffset = (uint)data.Length;
            AppendData(sna.data);
            rtb.Add(sna.rtb);
            foreach (SNAMemoryBlock block in sna.blocks) {
                block.sna = this;
                block.pointerList = rtb.GetListForPart(block.module, block.id);
                block.dataPosition += snaOffset;
                block.position += snaOffset;
                blocks.Add(block);
                ushort ptrRelocationKey = GetRelocationKey(block);
                if (relocation_local.ContainsKey(ptrRelocationKey)) {
                    if (block.size != 0) relocation_local[ptrRelocationKey] = block;
                } else relocation_local[ptrRelocationKey] = block;

            }
        }

        private void AppendData(byte[] other) {
            bool reading = false;
            uint readerOffset = 0;
            if (reader != null) {
                reading = true;
                readerOffset = (uint)reader.BaseStream.Position;
            }
            uint oldLength = (uint)data.Length;
            /*byte[] newData = new byte[data.Length + other.Length];
            Array.Copy(data, newData, data.Length);
            Array.Copy(other, 0, newData, data.Length, other.Length);
            data = newData;*/
            Array.Resize(ref data, (int)(oldLength + other.Length));
            Array.Copy(other, 0, data, oldLength, other.Length);
            if (reading) {
                reader.Close();
                reader = new Reader(new MemoryStream(data), Settings.s.IsLittleEndian);
                reader.BaseStream.Seek(readerOffset, SeekOrigin.Begin);
            }
        }

        void ReadSNABlock(SNAMemoryBlock block) {
            MapLoader l = MapLoader.Loader;
            if (block.size > 0) {
                if (Settings.s.snaCompression) {
                    uint isCompressed = reader.ReadUInt32();
                    uint compressedSize = reader.ReadUInt32();
                    uint compressedChecksum = reader.ReadUInt32();
                    uint decompressedSize = reader.ReadUInt32();
                    uint decompressedChecksum = reader.ReadUInt32();
                    //l.print(isCompressed + " - " + compressedSize + " - " + decompressedSize);
                    byte[] compressedData = reader.ReadBytes((int)compressedSize);
                    byte[] uncompressedData = null;
                    int diff = 0;

                    if (isCompressed != 0) {
                        diff = (int)decompressedSize - (int)compressedSize;

                        /*LZOCompressor lzo = new LZOCompressor();
                        uncompressedData = lzo.Decompress(compressedData);
                        using (var uncompressedStream = new MemoryStream(uncompressedData))
                        using (Reader unCompressedReader = new Reader(uncompressedStream, Settings.s.IsLittleEndian)) {
                            uncompressedData = unCompressedReader.ReadBytes((int)block.size);
                        }*/
                        using (var compressedStream = new MemoryStream(compressedData))
                        using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
                        using (Reader lzoReader = new Reader(lzo, Settings.s.IsLittleEndian)) {
                            lzo.SetLength(decompressedSize);
                            uncompressedData = lzoReader.ReadBytes((int)block.size);
                        }
                    } else {
                        diff = 0;
                        using (var uncompressedStream = new MemoryStream(compressedData))
                        using (Reader unCompressedReader = new Reader(uncompressedStream, Settings.s.IsLittleEndian)) {
                            uncompressedData = unCompressedReader.ReadBytes((int)block.size);
                        }
                    }
                    if (uncompressedData != null) {
                        byte[] newData = new byte[data.Length + diff - 20];
                        Array.Copy(data, newData, block.dataPosition);
                        //l.print(uncompressedData.Length);
                        Array.Copy(uncompressedData, 0, newData, block.dataPosition, block.size);
                        Array.Copy(data, block.dataPosition + 20 + compressedSize,
                            newData, block.dataPosition + compressedSize + diff,
                            data.Length - block.dataPosition - 20 - compressedSize);
                        data = newData;
                        reader.Close();
                        reader = new Reader(new MemoryStream(data), Settings.s.IsLittleEndian);
                        reader.BaseStream.Seek(block.dataPosition + compressedSize + diff, SeekOrigin.Begin);
                    }
                } else {
                    reader.ReadBytes((int)block.size);
                }
            }
        }
        long CalculateChecksum(SNAMemoryBlock block)
        {
            long v4 = 1;
            long v5 = 0;
            long v39 = 0;
            
            if (data == null) {
                return 1;
            }

            uint offset = block.dataPosition;
            for (uint i = block.size; i != 0; v5 %= 0xFFF1u)
            {
                uint v8 = i;
                if (i >= 5552)
                    v8 = 5552;
                for (i -= v8; v8 >= 16; v5 = v4 + v39)
                {
                    v8 -= 16;
                    v39 = data[offset + 14] + 2 * data[offset + 13] + 3 * data[offset + 12] + 4 * data[offset + 11] + 5 * data[offset + 10] + 6 * data[offset + 9] + 7 * data[offset + 8] + 8 * data[offset + 7] + 9 * data[offset + 6] + 10 * data[offset + 5] + 11 * data[offset + 4] + 12 * data[offset + 3] + 13 * data[offset + 2] + 14 * data[offset + 1] + 15 * data[offset + 0] + 15 * v4 + v5;
                    v4 = data[offset + 15] + data[offset + 14] + data[offset + 13] + data[offset + 12] + data[offset + 11] + data[offset + 10] + data[offset + 9] + data[offset + 8] + data[offset + 7] + data[offset + 6] + data[offset + 5] + data[offset + 4] + data[offset + 3] + data[offset + 2] + data[offset + 1] + data[offset + 0] + v4;

                    offset += 16;
                }
                if (v8 != 0)
                {
                  do
                  {
                    v4 += data[offset++];
                    v5 += v4;
                    --v8;
                  }
                  while (v8 > 0);
                }
                v4 %= 0xFFF1u;
            }
            return v4 | (v5 << 16);
        }

        void WriteSNABlock(SNAMemoryBlock block)
        {
            if (block.size > 0)
            {
                uint checksum = (uint)CalculateChecksum(block);
                writer.Write(0); // no compression
                // compressed size & checksum
                writer.Write(block.size);
                writer.Write(checksum);
                // decompressed size & checksum
                writer.Write(block.size);
                writer.Write(checksum);

                writer.BaseStream.Write(data, (int)block.dataPosition, (int)block.size);
            }
        }

        public void ReadGPT(string path, RelocationTable rtp) {
            this.rtp = rtp;
            Stream gptStream = FileSystem.GetFileReadStream(path);
            uint gptOffset = (uint)data.Length;
            byte[] gptData = null;
            using (Reader gptReader = new Reader(gptStream, Settings.s.IsLittleEndian)) {
                int maskBytes = Settings.s.encryptPointerFiles ? gptReader.InitMask() : 0;
                gptData = gptReader.ReadBytes((int)gptStream.Length - maskBytes);
            }
            //Util.ByteArrayToFile(path + ".dmp", gptData);
            AppendData(gptData);
            SNAMemoryBlock block;
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                ushort ptrRelocationKey = GetRelocationKey(rtp.pointerBlocks[0]);
                block = relocation_local[ptrRelocationKey];
            } else {
                // Tonic Trouble LVL
                block = new SNAMemoryBlock();
            }
            block.pointerList = null;
            block.dataPosition = gptOffset;
            block.size = (uint)gptData.Length;
            block.isGpt = true;
            gpt = block;
            headerOffset = gpt.dataPosition;
            //R3Loader.Loader.print("Base " + block.baseInMemory + " - Size: " + block.size);
        }

        public void ReadDLG(string path, RelocationTable rtd) {
            this.rtd = rtd;
            Stream dlgStream = FileSystem.GetFileReadStream(path);
            uint dlgOffset = (uint)data.Length;
            byte[] dlgData = null;
            using (Reader dlgReader = new Reader(dlgStream, Settings.s.IsLittleEndian)) {
                int maskBytes = Settings.s.encryptPointerFiles ? dlgReader.InitMask() : 0;
                dlgData = dlgReader.ReadBytes((int)dlgStream.Length - maskBytes);
            }
            //Util.ByteArrayToFile(path + ".dmp", gptData);
            AppendData(dlgData);
            SNAMemoryBlock block;
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                ushort ptrRelocationKey = GetRelocationKey(rtd.pointerBlocks[0]);
                block = relocation_local[ptrRelocationKey];
            } else {
                block = new SNAMemoryBlock();
            }
            block.pointerList = null;
            block.dataPosition = dlgOffset;
            block.size = (uint)dlgData.Length;
            block.isGpt = true;
            dlg = block;
            //R3Loader.Loader.print("Base " + block.baseInMemory + " - Size: " + block.size);
        }

        public void ReadSDA(string path) {
            Stream sdaStream = FileSystem.GetFileReadStream(path);
            uint sdaOffset = (uint)data.Length;
            byte[] sdaData = null;
            using (Reader sdaReader = new Reader(sdaStream, Settings.s.IsLittleEndian)) {
                sdaData = sdaReader.ReadBytes((int)sdaStream.Length);
            }
            AppendData(sdaData);
            sda = new SNAMemoryBlock();
            sda.dataPosition = sdaOffset;
            sda.position = sdaOffset;
            sda.size = (uint)sdaData.Length;
            sda.sna = this;
        }

        public void ReadPTX(string path, RelocationTable rtt) {
            this.rtt = rtt;
            Stream ptxStream = FileSystem.GetFileReadStream(path);
            uint ptxOffset = (uint)data.Length;
            byte[] ptxData = null;
            using (Reader ptxReader = new Reader(ptxStream, Settings.s.IsLittleEndian)) {
                int maskBytes = Settings.s.encryptPointerFiles ? ptxReader.InitMask() : 0;
                ptxData = ptxReader.ReadBytes((int)ptxStream.Length - maskBytes);
            }
            //Util.ByteArrayToFile(path + ".dmp", ptxData);
            AppendData(ptxData);
            ptx = new SNAMemoryBlock();
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
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
                            //l.print(info.module + "," + info.id + "| " + info.offsetInMemory + " - " + block_local.baseInMemory + " - " + block_global.dataPosition + "|" + block_global.sna.name);
                            uint relativeAddress = info.offsetInMemory - (uint)block_local.baseInMemory;
                            cur_reader.BaseStream.Seek(block_global.dataPosition + relativeAddress, SeekOrigin.Begin);
                            uint ptrValue = cur_reader.ReadUInt32();
                            
                            ushort ptrRelocationKey = GetRelocationKey(info);
                            if (!l.relocation_global.ContainsKey(ptrRelocationKey)) {
                                if (Settings.s.engineVersion > Settings.EngineVersion.Montreal || !(info.module == 0xFF && info.id == 0xFF)) {
                                    l.print("Could not find SNA block (" + info.module + "," + info.id + ")");
                                }
                            } else {
                                //l.print("SNA block (" + ptrList.module + "," + ptrList.id + ")" + " - " + "Info block (" + info.module + "," + info.id + ")");
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
            if (gpt != null) RelocatePointerFile(gpt, rtp); // Now for the Global Pointer Table
            if (ptx != null) RelocatePointerFile(ptx, rtt); // Now for the PTX
            if (dlg != null) RelocatePointerFile(dlg, rtd); // Now for the DLG
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
                        RelocationPointerInfo info = null;
                        if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                            foreach (RelocationPointerInfo info_new in ptrList.pointers) {
                                if (info_new.offsetInMemory == ptrValue) {
                                    info = info_new;
                                    break;
                                }
                            }
                        } else {
                            if (ptrList.pointers[listIndex].offsetInMemory == ptrValue) {
                                info = ptrList.pointers[listIndex];
                            }
                        }
                        if (info != null) {
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
                            if (Settings.s.engineVersion != Settings.EngineVersion.Montreal) {
                                listIndex++;
                                if (listIndex >= ptrList.pointers.Length) break;
                            }
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

        public Pointer PTX {
            get {
                if (ptx != null) {
                    return new Pointer(ptx.dataPosition, this);
                } else return null;
            }
        }

        public Pointer SDA {
            get {
                if (sda != null) {
                    return new Pointer(sda.dataPosition, this);
                } else return null;
            }
        }

        public Pointer DLG {
            get {
                if (dlg != null) {
                    return new Pointer(dlg.dataPosition, this);
                } else return null;
            }
        }

        public override void CreateWriter() {
            if (path != null)
            {
                FileStream stream = new FileStream(path, FileMode.Open);
                writer = new Writer(stream, Settings.s.IsLittleEndian);
            }
        }

        public override void WritePointer(Pointer pointer) {
            throw new NotImplementedException();
        }
    }
}
