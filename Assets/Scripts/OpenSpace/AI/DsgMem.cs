using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgMem {
        public Pointer offset;

        public Pointer off_dsgVar;
        public Pointer memBufferInitial; // initial state
        public Pointer memBuffer; // current state

        public DsgVar dsgVar;

        public DsgMem(Pointer offset) {
            this.offset = offset;
        }

        public static DsgMem Read(Reader reader, Pointer offset) {
            DsgMem dsgMem = new DsgMem(offset);

            Pointer dsgVarPointer = Pointer.Read(reader);
            if (dsgVarPointer != null) {
                Pointer off_current = Pointer.Goto(ref reader, dsgVarPointer);
                if (Settings.s.subMode == Settings.SubMode.R2Demo) {
                    Pointer.Read(reader);
                }
                dsgMem.off_dsgVar = Pointer.Read(reader);
                Pointer.Goto(ref reader, off_current);
            }

            dsgMem.memBufferInitial = Pointer.Read(reader);
            dsgMem.memBuffer = Pointer.Read(reader);

            if (dsgMem.off_dsgVar != null) {
                Pointer.Goto(ref reader, dsgMem.off_dsgVar);
                dsgMem.dsgVar = DsgVar.Read(reader, dsgMem.off_dsgVar, dsgMem);
            }
            return dsgMem;
        }
    }
}
