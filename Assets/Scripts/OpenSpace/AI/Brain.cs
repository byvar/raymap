using OpenSpace.EngineObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Brain {
        public Pointer offset;

        public Pointer off_mind;
        public uint unknown1;
        public uint unknown2;
        
        public Mind mind;

        public Brain(Pointer offset) {
            this.offset = offset;
        }

        public static Brain Read(Reader reader, Pointer offset) {
            Brain b = new Brain(offset);
            b.off_mind = Pointer.Read(reader);
            b.unknown1 = reader.ReadUInt32(); // init at 0xCDCDCDCD
            b.unknown2 = reader.ReadUInt32(); // 0

            if (b.off_mind != null) {
                Pointer.Goto(ref reader, b.off_mind);
                b.mind = Mind.Read(reader, b.off_mind);
            }
            return b;
        }
    }
}
