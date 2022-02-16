using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.FileFormat {
    struct DATHeader {
        public int field_0;
        public int field_4;
        public int field_8;
        public int field_C;
    }

    public class DAT : FileWithPointers {
        public DSB gameDsb;
		public uint lastOffset = 0;

        public DAT(string name, DSB gameDsb, string path) : this(name, gameDsb, FileSystem.GetFileReadStream(path)) { }

        public DAT(string name, DSB gameDsb, Stream stream) {
            this.gameDsb = gameDsb;
            baseOffset = 0;
            headerOffset = 0;
            this.name = name;
            reader = new Reader(stream, Legacy_Settings.s.IsLittleEndian);
        }

        public async UniTask GetOffset(RelocationTableReference rtref) {
			PartialHttpStream httpStream = reader.BaseStream as PartialHttpStream;
			reader.MaskingOff();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

			if (httpStream != null) await httpStream.FillCacheForRead(12*4);
			DATHeader header = new DATHeader();
            header.field_0 = reader.ReadInt32();
            header.field_4 = reader.ReadInt32();
            header.field_8 = reader.ReadInt32();
            header.field_C = reader.ReadInt32();

            int number;
            number = reader.ReadInt32();
            header.field_0 += header.field_8;
            header.field_4 += header.field_C;
            number = reader.ReadInt32();
            uint levels0DatValue_0 = (uint)(header.field_4 ^ (number - header.field_0));
            header.field_0 += header.field_8;
            header.field_4 += header.field_C;
            number = reader.ReadInt32();
            uint levels0DatValue_1 = (uint)((header.field_4 ^ (uint)(number - header.field_0)) >> 2);
            header.field_0 += header.field_8;
            header.field_4 += header.field_C;
            number = reader.ReadInt32();
            header.field_0 += header.field_8;
            header.field_4 += header.field_C;
            number = reader.ReadInt32();
            int v4 = header.field_4 ^ (number - header.field_0);
            header.field_0 += header.field_8;
            header.field_4 += header.field_C;
            uint levels0DatValue_2 = (uint)v4;
            number = reader.ReadInt32();
            int v5 = header.field_4 ^ (number - header.field_0);
            header.field_0 += header.field_8;
            uint levels0DatValue_3 = (uint)v5;
            header.field_4 += header.field_C;
            number = reader.ReadInt32();
            int v6 = header.field_4 ^ (number - header.field_0);
            header.field_0 += header.field_8;
            header.field_4 += header.field_C;
            uint levels0DatValue_4 = (uint)v6;
            number = reader.ReadInt32();
            int levels0DatValue_5 = header.field_4 ^ (number - header.field_0);
            header.field_0 += header.field_8;
            header.field_4 += header.field_C;

            // Get offset with sinus header - SNA_fn_hGetOffSetInBigFileWithSinusHeader
            
            rtref.levelId = (byte)(rtref.levelId % levels0DatValue_1);
            rtref.relocationType &= 3; // can only be 0, 1, 2 or 3. No RTL? or another relocation type?

            v6 = 4 * rtref.levelId;
            int v28 = 4 * rtref.levelId;

            int v7 = v28;

            switch (rtref.relocationType) {
                case 1:
                    v7 = v6 + 1;
                    v28 = v7;
                    break;
                case 2:
                    v7 = v6 + 2;
                    v28 = v7;
                    break;
                case 3:
                    v7 = v6 + 3;
                    v28 = v7;
                    break;
                default:
                    break;
            }

            uint v8 = rtref.byte2 % levels0DatValue_2;
            float v9 = 1.06913f;
            float v30 = 1.06913f;
            rtref.byte2 = (byte)v8;

            if (v8 != 0) {
                uint v10 = 0;
                double v11 = 0;

                do {
                    v30 = v10;
                    v11 = v10;
                    v10++;
                    v9 = v9 - Math.Abs((float)Math.Sin(v11 * v11 * 1.69314f)) * -0.69314f - -0.52658f;
                }
                while (v10 < v8);

                v30 = v9;
            }

            //float v23 = (float)Math.Truncate(v30);
            //double v12 = v30 - v23;
            double v12 = v30 % 1f;
            double v13 = Math.Floor(v12 * 1000000.0) / 1000000.0;
            //R3Loader.Loader.print("Double v13: " + v13);
            ulong v24 = levels0DatValue_0;
            long v14 = (long)Math.Floor(levels0DatValue_0 * v13);

            reader.BaseStream.Seek(levels0DatValue_4 + levels0DatValue_5 * v14, SeekOrigin.Begin);
			if (httpStream != null) await httpStream.FillCacheForRead(4 * 4);

			header.field_0 = reader.ReadInt32();
            header.field_4 = reader.ReadInt32();
            header.field_8 = reader.ReadInt32();
            header.field_C = reader.ReadInt32();

            if (v28 != 0) {
                int v15, v16;

                header.field_0 += v28 * header.field_8;
                v15 = header.field_4;
                v16 = v28;
                do {
                    v15 += header.field_C;
                    --v16;
                }
                while (v16 != 0);
                header.field_4 = v15;
            }

            reader.BaseStream.Seek(4 * v28, SeekOrigin.Current);
			if (httpStream != null) await httpStream.FillCacheForRead(4);
			uint value1 = reader.ReadUInt32();

            uint dataOffset = (uint)(header.field_4 ^ (value1 - header.field_0));

			lastOffset = dataOffset;
        }

        public uint GetMask(RelocationTableReference rtref) {
            rtref.byte3 = (byte)~rtref.byte2;
            byte[] rtRefBytes = new byte[] { rtref.levelId, rtref.relocationType, rtref.byte2, rtref.byte3 };
            if (Legacy_Settings.s.IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(rtRefBytes);
            uint currentMask = BitConverter.ToUInt32(rtRefBytes, 0);
            if (Legacy_Settings.s.platform == Legacy_Settings.Platform.iOS) {
                return (uint)(16807 * ((currentMask ^ 0x75BD924) % 0x1F31D) - 2836 * ((currentMask ^ 0x75BD924) / 0x1F31D));
            } else {
                return (uint)(16807 * (currentMask ^ 0x75BD924) - 0x7FFFFFFF * ((currentMask ^ 0x75BD924) / 0x1F31D));
            }
        }

        public override void CreateWriter() {
            return; // Don't need to write to this file
        }

        public override void WritePointer(LegacyPointer pointer) {
            return;
        }
    }
}
