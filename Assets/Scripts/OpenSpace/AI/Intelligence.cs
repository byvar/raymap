using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Intelligence : OpenSpaceStruct {
        public LegacyPointer off_aiModel;
        public LegacyPointer off_actionTree;
        public LegacyPointer off_comport;
        public LegacyPointer off_lastComport;
        public LegacyPointer off_actionTable;
        public LegacyPointer off_defaultComport;

        // Processed data
        public AIModel aiModel;
        public Behavior comport;
        public Behavior lastComport;
        public Behavior defaultComport;

        protected override void ReadInternal(Reader reader) {
            off_aiModel = LegacyPointer.Read(reader); // 0x0
            off_actionTree = LegacyPointer.Read(reader); //0x4
            off_comport = LegacyPointer.Read(reader); //0x8
            if (CPA_Settings.s.game != CPA_Settings.Game.R2Demo) {
                off_lastComport = LegacyPointer.Read(reader);
                off_actionTable = LegacyPointer.Read(reader);
                off_defaultComport = LegacyPointer.Read(reader);
            }

            aiModel = Load.FromOffset<AIModel>(off_aiModel);
            comport = Load.FromOffset<Behavior>(off_comport);
            lastComport = Load.FromOffset<Behavior>(off_lastComport);
            if (off_defaultComport != null) {
                defaultComport = Load.FromOffset<Behavior>(off_defaultComport);
            }
        }

        public void Write(Writer writer) {
            LegacyPointer.Goto(ref writer, Offset + 8);
            LegacyPointer.Write(writer, off_comport);

            LegacyPointer.Goto(ref writer, Offset + 20);
            LegacyPointer.Write(writer, off_defaultComport);
        }
    }
}