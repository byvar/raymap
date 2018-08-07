using OpenSpace.EngineObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Script {
        public Pointer offset; // offset of the pointer to the script
        public BehaviorOrMacro behaviorOrMacro;

        public Pointer off_script; // offset where the script starts
        public List<ScriptNode> scriptNodes = new List<ScriptNode>();

        public Script(Pointer offset) {
            this.offset = offset;
        }

        public static Script Read(Reader reader, Pointer offset, BehaviorOrMacro behaviorOrMacro) {
            MapLoader l = MapLoader.Loader;
            Script s = new Script(offset);

            s.behaviorOrMacro = behaviorOrMacro;

            s.off_script = Pointer.Read(reader);
            if (s.off_script != null) {
                Pointer off_current = Pointer.Goto(ref reader, s.off_script);
                bool endReached = false;
                while (!endReached) {
                    ScriptNode sn = ScriptNode.Read(reader, Pointer.Current(reader), s);
                    s.scriptNodes.Add(sn);

                    if (sn.indent == 0) endReached = true;
                }
                Pointer.Goto(ref reader, off_current);
            }
            return s;
        }

        public void print(Perso perso) {
            // TODO: Use perso to print states, etc.
            MapLoader l = MapLoader.Loader;
            StringBuilder builder = new StringBuilder();
            builder.Append("Script @ offset: " + offset + "\n");
            foreach (ScriptNode sn in scriptNodes) {
                if (sn.indent == 0) {
                    builder.Append("---- END OF SCRIPT ----");
                } else {
                    builder.Append(new String(' ', (sn.indent - 1) * 4));
                    if (Settings.s.engineMode == Settings.EngineMode.R2) {
                        builder.Append(R2AITypes.readableFunctionSubType(sn, perso));
                    } else {
                        builder.Append(R3AITypes.readableFunctionSubType(sn, perso));
                    }
                }
                builder.Append("\n");
            }
            l.print(builder.ToString());
        }
    }
}
