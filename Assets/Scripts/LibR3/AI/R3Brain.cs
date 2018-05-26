using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3Brain {
        public R3Pointer offset;

        public R3Pointer off_mind;
        public uint unknown1;
        public uint unknown2;

        public R3Mind mind;

        public R3Brain(R3Pointer offset) {
            this.offset = offset;
        }

        public static R3Brain Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Brain b = new R3Brain(offset);
            b.off_mind = R3Pointer.Read(reader);
            b.unknown1 = reader.ReadUInt32(); // init at 0xCDCDCDCD
            b.unknown2 = reader.ReadUInt32(); // 0

            if (b.off_mind != null) {
                R3Pointer.Goto(ref reader, b.off_mind);
                b.mind = R3Mind.Read(reader, b.off_mind);
            }
            return b;
        }
    }
}
