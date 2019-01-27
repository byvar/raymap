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
            //MapLoader.Loader.print("DsgMem " + offset);
            Pointer dsgVarPointer = Pointer.Read(reader);
            Pointer.DoAt(ref reader, dsgVarPointer, () => {
                if (Settings.s.game == Settings.Game.R2Demo) {
                    Pointer.Read(reader);
                }
                dsgMem.off_dsgVar = Pointer.Read(reader);
            });
            
            dsgMem.memBufferInitial = Pointer.Read(reader);
            dsgMem.memBuffer = Pointer.Read(reader);

            Pointer.DoAt(ref reader, dsgMem.off_dsgVar, () => {
                dsgMem.dsgVar = DsgVar.Read(reader, dsgMem.off_dsgVar, dsgMem);
            });
            return dsgMem;
        }
    }
}
