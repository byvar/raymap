using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Mind : OpenSpaceStruct {
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

        public void UpdateCurrentBehaviors(Reader reader)
        {
            off_AI_model = Pointer.Read(reader);
            off_intelligence_normal = Pointer.Read(reader);
            off_intelligence_reflex = Pointer.Read(reader);
            off_dsgMem = Pointer.Read(reader);

            MapLoader l = MapLoader.Loader;
            if (dsgMem == null) {
                dsgMem = l.FromOffsetOrRead<DsgMem>(reader, off_dsgMem);
            } 
            dsgMem?.Read(reader);

            if (intelligenceNormal == null) {
                intelligenceNormal = l.FromOffsetOrRead<Intelligence>(reader, off_intelligence_normal);
            }
            intelligenceNormal?.Read(reader);

            if (intelligenceReflex == null) {
                intelligenceReflex = l.FromOffsetOrRead<Intelligence>(reader, off_intelligence_reflex);
            }
            intelligenceReflex?.Read(reader);
        }

        protected override void ReadInternal(Reader reader) {
            off_AI_model = Pointer.Read(reader);
            off_intelligence_normal = Pointer.Read(reader);
            if (CPA_Settings.s.game == CPA_Settings.Game.R2Demo) {
                off_dsgMem = Pointer.Read(reader);
                off_intelligence_reflex = Pointer.Read(reader);
            } else {
                off_intelligence_reflex = Pointer.Read(reader);
                off_dsgMem = Pointer.Read(reader);
            }
            if (CPA_Settings.s.hasNames) {
                off_name = Pointer.Read(reader);
            }
            byte0 = reader.ReadByte();
            byte1 = reader.ReadByte();
            byte2 = reader.ReadByte();
            byte3 = reader.ReadByte();

            if (CPA_Settings.s.game == CPA_Settings.Game.R2Demo) {
                // null
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            }

            MapLoader l = MapLoader.Loader;
            AI_model = l.FromOffsetOrRead<AIModel>(reader, off_AI_model);
            dsgMem = l.FromOffsetOrRead<DsgMem>(reader, off_dsgMem);
            intelligenceNormal = l.FromOffsetOrRead<Intelligence>(reader, off_intelligence_normal);
            intelligenceReflex = l.FromOffsetOrRead<Intelligence>(reader, off_intelligence_reflex);
        }
    }
}
