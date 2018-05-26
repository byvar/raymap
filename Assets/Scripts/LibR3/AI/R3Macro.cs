using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3Macro {
        public R3Pointer offset;

        public string name = null;
        public R3Pointer off_script;
        public R3Pointer off_script2;
        public R3Script script;

        public R3Macro(R3Pointer offset) {
            this.offset = offset;
        }

        public static R3Macro Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            R3Macro m = new R3Macro(offset);
            if (l.mode == R3Loader.Mode.Rayman3GC) m.name = new string(reader.ReadChars(0x100));
            m.off_script = R3Pointer.Read(reader);
            m.off_script2 = R3Pointer.Read(reader);
            //if (m.name != null) l.print(m.name);

            if (m.off_script != null) {
                R3Pointer off_current = R3Pointer.Goto(ref reader, m.off_script);
                m.script = R3Script.Read(reader, R3Pointer.Current(reader));
                R3Pointer.Goto(ref reader, off_current);
            }
                
            return m;
        }
    }
}
