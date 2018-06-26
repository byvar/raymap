using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Mind {
        public Pointer offset;

        public Pointer off_AI_model;
        public Pointer off_intelligence_normal;
        public Pointer off_intelligence_reflex;
        public Pointer off_dsgMem;
        public Pointer off_name;

        public AIModel AI_model;
        public DsgMem dsgMem;

        public Mind(Pointer offset) {
            this.offset = offset;
        }

        public static Mind Read(EndianBinaryReader reader, Pointer offset) {
            Mind m = new Mind(offset);
            m.off_AI_model = Pointer.Read(reader);
            m.off_intelligence_normal = Pointer.Read(reader);
            m.off_intelligence_reflex = Pointer.Read(reader);
            m.off_dsgMem = Pointer.Read(reader);
            m.off_name = Pointer.Read(reader);
            
            if (m.off_AI_model != null) {
                m.AI_model = AIModel.FromOffset(m.offset);
                if (m.AI_model == null) {
                    Pointer.Goto(ref reader, m.off_AI_model);
                    m.AI_model = AIModel.Read(reader, m.off_AI_model);
                }
            }

            if (MapLoader.Loader.mode == MapLoader.Mode.Rayman2PC && m.off_dsgMem != null) {
                Pointer.Goto(ref reader, m.off_dsgMem);
                m.dsgMem = DsgMem.Read(reader, m.off_dsgMem);
            }
            return m;
        }
    }
}
