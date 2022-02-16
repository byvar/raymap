using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgMem : OpenSpaceStruct {
        public LegacyPointer off_dsgVar;
        public LegacyPointer memBufferInitial; // initial state
        public LegacyPointer memBuffer; // current state

        public DsgVar dsgVar;
        public DsgVarValue[] values;
        public DsgVarValue[] valuesInitial;


        protected override void ReadInternal(Reader reader) {
            //MapLoader.Loader.print("DsgMem " + Offset);
            LegacyPointer dsgVarPointer = LegacyPointer.Read(reader);
            LegacyPointer.DoAt(ref reader, dsgVarPointer, () => {
                off_dsgVar = LegacyPointer.Read(reader);
            });

            memBufferInitial = LegacyPointer.Read(reader);
            memBuffer = LegacyPointer.Read(reader);

            dsgVar = MapLoader.Loader.FromOffsetOrRead<DsgVar>(reader, off_dsgVar);
            if (dsgVar != null && dsgVar.amountOfInfos > 0) {
                if (memBuffer != null && Legacy_Settings.s.platform != Legacy_Settings.Platform.DC) {
                    // Current MemBuffer is cleared in DC files
                    values = new DsgVarValue[dsgVar.amountOfInfos];
                    for (int i = 0; i < dsgVar.amountOfInfos; i++) {
                        values[i] = new DsgVarValue(dsgVar.dsgVarInfos[i].type, this);
                        values[i].ReadFromDsgMemBuffer(reader, dsgVar.dsgVarInfos[i], this);
                        values[i].RegisterReferences(this);
                    }
                }
                if (memBufferInitial != null) {
                    valuesInitial = new DsgVarValue[dsgVar.amountOfInfos];
                    for (int i = 0; i < dsgVar.amountOfInfos; i++) {
                        valuesInitial[i] = new DsgVarValue(dsgVar.dsgVarInfos[i].type, this);
                        valuesInitial[i].ReadFromDsgMemBufferInitial(reader, dsgVar.dsgVarInfos[i], this);
                        valuesInitial[i].RegisterReferences(this);
                    }
                }
            }


        }

        public Mind mind { get; internal set; }
    }
}
