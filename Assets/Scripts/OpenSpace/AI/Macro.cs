using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Macro {
        public Pointer offset;

        public string name = null;
        public Pointer off_script;
        public Pointer off_script2;
        public Script script;

        public Macro(Pointer offset) {
            this.offset = offset;
        }

        public static Macro Read(EndianBinaryReader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Macro m = new Macro(offset);
            if (l.mode == MapLoader.Mode.Rayman3GC) m.name = new string(reader.ReadChars(0x100)).TrimEnd('\0');
            m.off_script = Pointer.Read(reader);
            m.off_script2 = Pointer.Read(reader);
            //if (m.name != null) l.print(m.name);

            if (m.off_script != null) {
                Pointer off_current = Pointer.Goto(ref reader, m.off_script);
                m.script = Script.Read(reader, Pointer.Current(reader), null);
                Pointer.Goto(ref reader, off_current);
            }
                
            return m;
        }
    }
}
