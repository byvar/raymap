using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace.FileFormat {
    struct DATHeader {
        public int field_0;
        public int field_4;
        public int field_8;
        public int field_C;
    }

    public class DAT : FileWithPointers {
        // Hardcoded, because every level GPT contains this, except fix. Fix only contains the index of the first level to load.
        // To load arbitrary levels, we need to have access to this table before loading the level so it had to be hardcoded.
        // Actually no, can be loaded from Game.dsb
        public static string[] levelList = {
            "Menu", "Jail_10", "Jail_20", "Mapmonde", "Learn_10", "Learn_30", "Bonux", "Learn_31",
            "Bast_20", "Bast_22", "Learn_60", "Ski_10", "Vulca_10", "Vulca_20", "Ski_60",
            "Batam_10", "Chase_10", "Ly_10", "Chase_22", "Rodeo_10", "Rodeo_40", "Rodeo_60",
            "nego_10", "Water_10", "Water_20", "GLob_30", "GLob_10", "GLob_20", "Whale_00",
            "Whale_05", "Whale_10", "Plum_00", "Plum_10", "Bast_09", "Bast_10", "Cask_10",
            "Cask_30", "Batam_20", "Nave_10", "Nave_15", "Nave_20", "Seat_10", "Seat_11",
            "Earth_10", "Earth_20", "Ly_20", "Earth_30", "Helic_10", "Helic_20", "Helic_30",
            "Plum_20", "Morb_00", "Morb_10", "Morb_20", "Learn_40", "Ball", "ile_10", "Mine_10",
            "Boat01", "Boat02", "Astro_00", "Astro_10", "Rhop_10", "end_10", "staff_10",
            "poloc_10", "poloc_20", "poloc_30", "poloc_40", "Raycap"
        };
        public DAT(string name, string path) : this(name, File.OpenRead(path)) { }

        public DAT(string name, Stream stream) {
            baseOffset = 0;
            headerOffset = 0;
            this.name = name;
            reader = new EndianBinaryReader(stream, Settings.s.IsLittleEndian);
        }

        public uint GetOffset(RelocationTableReference rtref) {
            reader.MaskingOff();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

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
            uint value1 = reader.ReadUInt32();

            uint dataOffset = (uint)(header.field_4 ^ (value1 - header.field_0));

            return dataOffset;
        }

        public uint GetMask(RelocationTableReference rtref) {
            rtref.byte3 = (byte)~rtref.byte2;
            byte[] rtRefBytes = new byte[] { rtref.levelId, rtref.relocationType, rtref.byte2, rtref.byte3 };
            if (Settings.s.IsLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(rtRefBytes);
            uint currentMask = BitConverter.ToUInt32(rtRefBytes, 0);
            if (MapLoader.Loader.mode == MapLoader.Mode.Rayman2IOS) {
                return (uint)(16807 * ((currentMask ^ 0x75BD924) % 0x1F31D) - 2836 * ((currentMask ^ 0x75BD924) / 0x1F31D));
            } else {
                return (uint)(16807 * (currentMask ^ 0x75BD924) - 0x7FFFFFFF * ((currentMask ^ 0x75BD924) / 0x1F31D));
            }
        }
    }
}
