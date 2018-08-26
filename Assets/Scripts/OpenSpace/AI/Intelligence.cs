using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Intelligence {
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

        public Intelligence(Pointer offset) {
            this.offset = offset;
        }

        public static Intelligence Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Intelligence i = new Intelligence(offset);
            i.off_aiModel = Pointer.Read(reader); // 0x0
            i.off_actionTree = Pointer.Read(reader); //0x4
            i.off_comport = Pointer.Read(reader); //0x8
            if (Settings.s.game != Settings.Game.R2Demo) {
                i.off_lastComport = Pointer.Read(reader);
                i.off_actionTable = Pointer.Read(reader);
            }

            i.aiModel = AIModel.FromOffset(i.off_aiModel);
            i.comport = Behavior.FromOffset(i.off_comport);
            i.lastComport = Behavior.FromOffset(i.off_comport);

            return i;
        }
    }
}