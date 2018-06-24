using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgMem {
        public Pointer offset;

        public Pointer off_dsgVar;
        public uint memBufferInitial; // initial state
        public uint memBuffer; // current state

        public DsgVar dsgVar;

        public DsgMem(Pointer offset) {
            this.offset = offset;
        }

        public static DsgMem Read(EndianBinaryReader reader, Pointer offset) {
            DsgMem d = new DsgMem(offset);

            d.off_dsgVar = Pointer.Read(reader);

            d.memBufferInitial = reader.ReadUInt32();
            d.memBuffer = reader.ReadUInt32();

            if (d.off_dsgVar != null) {
                Pointer.Goto(ref reader, d.off_dsgVar);
                d.dsgVar = DsgVar.Read(reader, d.off_dsgVar);
            }
            return d;
        }
    }
}
