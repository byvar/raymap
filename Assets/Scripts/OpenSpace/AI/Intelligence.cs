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
        

        public Intelligence(Pointer offset) {
            this.offset = offset;
        }

        public static Intelligence Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Intelligence i = new Intelligence(offset);
            i.off_aiModel = Pointer.Read(reader);
            i.off_actionTree = Pointer.Read(reader);
            i.off_comport = Pointer.Read(reader);
            if (!Settings.s.isR2Demo) {
                i.off_lastComport = Pointer.Read(reader);
                i.off_actionTable = Pointer.Read(reader);
            }
            return i;
        }
    }
}