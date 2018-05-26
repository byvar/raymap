using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3Mind {
        public R3Pointer offset;

        public R3Pointer off_AI_model;
        public R3Pointer off_intelligence_normal;
        public R3Pointer off_intelligence_reflex;
        public R3Pointer off_dsgMem;
        public R3Pointer off_name;

        public R3AIModel AI_model;

        public R3Mind(R3Pointer offset) {
            this.offset = offset;
        }

        public static R3Mind Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Mind m = new R3Mind(offset);
            m.off_AI_model = R3Pointer.Read(reader);
            m.off_intelligence_normal = R3Pointer.Read(reader);
            m.off_intelligence_reflex = R3Pointer.Read(reader);
            m.off_dsgMem = R3Pointer.Read(reader);
            m.off_name = R3Pointer.Read(reader);
            
            if (m.off_AI_model != null) {
                m.AI_model = R3AIModel.FromOffset(m.offset);
                if (m.AI_model == null) {
                    R3Pointer.Goto(ref reader, m.off_AI_model);
                    m.AI_model = R3AIModel.Read(reader, m.off_AI_model);
                }
            }
            return m;
        }
    }
}
