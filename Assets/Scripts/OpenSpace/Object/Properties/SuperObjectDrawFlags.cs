using OpenSpace;
using System;
using System.Collections;
using UnityEngine;

namespace OpenSpace.Object.Properties {
    public struct SuperObjectDrawFlags {

        public Flags flags;
        [Flags]
        public enum Flags {
			None = 0,
			Flag0 = 1 << 0,
            Flag1 = 1 << 1,
            Flag2 = 1 << 2,
            Flag3 = 1 << 3,
            Flag4 = 1 << 4,
            Flag5 = 1 << 5,
            Flag6 = 1 << 6,
            Flag7 = 1 << 7,
            Flag8 = 1 << 8,
            Flag9 = 1 << 9,
            Flag10 = 1 << 10,
            Flag11 = 1 << 11,
            Flag12 = 1 << 12,
            Flag13 = 1 << 13,
            Flag14 = 1 << 14,
            Flag15 = 1 << 15,
            Flag16 = 1 << 16,
            Flag17 = 1 << 17,
            Flag18 = 1 << 18,
            Flag19 = 1 << 19,
            Flag20 = 1 << 20,
            Flag21 = 1 << 21,
            Flag22 = 1 << 22,
            DontDrawInMirror = 1 << 23,
            DrawOnlyInMirror = 1 << 24,
            Flag25 = 1 << 25,
            Flag26 = 1 << 26,
            Flag27 = 1 << 27,
            Mirror = 1 << 28,
            Flag29 = 1 << 29,
            Flag30 = 1 << 30,
            SinusEffect = 1 << 31,
        }

        public static SuperObjectDrawFlags Read(Reader reader) {
            SuperObjectDrawFlags soFlags = new SuperObjectDrawFlags();
            soFlags.flags = (Flags)(reader.ReadUInt32() ^ 0xFFFFFFFF);
            return soFlags;
        }

        public void Write(Writer writer) {
            writer.Write(((uint)flags ^ 0xFFFFFFFF));
        }

        public bool HasFlag(Flags flag) {
            return (flags & flag) == flag;
        }
    }
}