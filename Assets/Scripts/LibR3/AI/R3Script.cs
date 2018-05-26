using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3Script {
        public R3Pointer offset; // offset of the pointer to the script

        public R3Pointer off_script; // offset where the script starts
        public List<R3ScriptNode> scriptNodes = new List<R3ScriptNode>();

        public R3Script(R3Pointer offset) {
            this.offset = offset;
        }

        public static R3Script Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            R3Script s = new R3Script(offset);
            
            s.off_script = R3Pointer.Read(reader);
            if (s.off_script != null) {
                R3Pointer off_current = R3Pointer.Goto(ref reader, s.off_script);
                bool endReached = false;
                while (!endReached) {
                    R3ScriptNode sn = R3ScriptNode.Read(reader, R3Pointer.Current(reader));
                    s.scriptNodes.Add(sn);
                    if (sn.indent == 0) endReached = true;
                }
                R3Pointer.Goto(ref reader, off_current);
            }
            return s;
        }
    }
}
