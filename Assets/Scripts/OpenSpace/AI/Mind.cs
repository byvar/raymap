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
        public byte byte0;
        public byte byte1;
        public byte byte2;
        public byte byte3;
        
        public AIModel AI_model;
        public DsgMem dsgMem;
        public Intelligence intelligenceNormal;
        public Intelligence intelligenceReflex;
        public string name = "";

        public Mind(Pointer offset) {
            this.offset = offset;
        }

        public static Mind Read(Reader reader, Pointer offset) {
            Mind m = new Mind(offset);
            m.off_AI_model = Pointer.Read(reader);
            m.off_intelligence_normal = Pointer.Read(reader);
            //if (m.off_intelligence_normal != null) MapLoader.Loader.print(m.off_intelligence_normal);
            m.off_intelligence_reflex = Pointer.Read(reader);
            m.off_dsgMem = Pointer.Read(reader);
            if (Settings.s.hasNames) {
                m.off_name = Pointer.Read(reader);
            }
            m.byte0 = reader.ReadByte();
            m.byte1 = reader.ReadByte();
            m.byte2 = reader.ReadByte();
            m.byte3 = reader.ReadByte();

            m.AI_model = AIModel.FromOffsetOrRead(m.off_AI_model, reader);

            if (m.off_dsgMem != null) {
                Pointer.Goto(ref reader, m.off_dsgMem);
                m.dsgMem = DsgMem.Read(reader, m.off_dsgMem);
            }

            if (m.off_intelligence_normal != null) {
                Pointer.Goto(ref reader, m.off_intelligence_normal);
                m.intelligenceNormal = Intelligence.Read(reader, m.off_intelligence_normal);
            }

            if (m.off_intelligence_reflex != null) {
                Pointer.Goto(ref reader, m.off_intelligence_reflex);
                m.intelligenceReflex = Intelligence.Read(reader, m.off_intelligence_reflex);
            }
            return m;
        }
    }
}
