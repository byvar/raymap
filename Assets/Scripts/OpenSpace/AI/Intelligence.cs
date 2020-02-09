using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Intelligence : OpenSpaceStruct {
        public Pointer offset;

        public Pointer off_aiModel;
        public Pointer off_actionTree;
        public Pointer off_comport;
        public Pointer off_lastComport;
        public Pointer off_actionTable;

        // Processed data
        public AIModel aiModel;
        public Behavior comport;
        public Behavior lastComport;

        protected override void ReadInternal(Reader reader) {
            off_aiModel = Pointer.Read(reader); // 0x0
            off_actionTree = Pointer.Read(reader); //0x4
            off_comport = Pointer.Read(reader); //0x8
            if (Settings.s.game != Settings.Game.R2Demo) {
                off_lastComport = Pointer.Read(reader);
                off_actionTable = Pointer.Read(reader);
            }

            aiModel = Load.FromOffset<AIModel>(off_aiModel);
            comport = Load.FromOffset<Behavior>(off_comport);
            lastComport = Load.FromOffset<Behavior>(off_lastComport);
        }
    }
}