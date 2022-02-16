using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Script {
        public LegacyPointer offset; // offset of the pointer to the script
        public BehaviorOrMacro behaviorOrMacro;

        public LegacyPointer off_script; // offset where the script starts
        public List<ScriptNode> scriptNodes = new List<ScriptNode>();

        public Script(LegacyPointer offset) {
            this.offset = offset;
        }

        public static Script Read(Reader reader, LegacyPointer offset, BehaviorOrMacro behaviorOrMacro, bool single = false) {
            MapLoader l = MapLoader.Loader;
            Script s = new Script(offset);

            s.behaviorOrMacro = behaviorOrMacro;

			if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution && single) {
				s.off_script = LegacyPointer.Current(reader);
				bool endReached = false;
				while (!endReached) {
					ScriptNode sn = ScriptNode.Read(reader, LegacyPointer.Current(reader), s);
					s.scriptNodes.Add(sn);

					if (sn.indent == 0) endReached = true;
				}
			} else {
				s.off_script = LegacyPointer.Read(reader);

                //l.print(s.off_script);
				LegacyPointer.DoAt(ref reader, s.off_script, () => {
					bool endReached = false;
					while (!endReached) {
						ScriptNode sn = ScriptNode.Read(reader, LegacyPointer.Current(reader), s);
						s.scriptNodes.Add(sn);

						if (sn.indent == 0) endReached = true;
					}
				});
			}
            return s;
        }

		public bool ContentEquals(Script s) {
			if (scriptNodes.Count != s.scriptNodes.Count) return false;
			for (int i = 0; i < scriptNodes.Count; i++) {
				if (scriptNodes[i] != null) {
					if (s.scriptNodes[i] == null) return false;
					if (!scriptNodes[i].ContentEquals(s.scriptNodes[i])) return false;
				} else if (s.scriptNodes[i] != null) return false;
			}
			return true;
		}

        public void print(Perso perso) {

            TranslatedScript.TranslationSettings settings = new TranslatedScript.TranslationSettings();

            // TODO: Use perso to print states, etc.
            MapLoader l = MapLoader.Loader;
            StringBuilder builder = new StringBuilder();
            builder.Append("Script @ offset: " + offset + "\n");
            foreach (ScriptNode sn in scriptNodes) {
                if (sn.indent == 0) {
                    builder.Append("---- END OF SCRIPT ----");
                } else {
                    builder.Append(new String(' ', (sn.indent - 1) * 4));
                    builder.Append(sn.ToString(perso, settings, advanced: true));
                }
                builder.Append("\n");
            }
            l.print(builder.ToString());
        }

        /**
         * Get nodes as an array of bytes, with each node consisting of the following:
         * struct node {
         *  int param
         *  short padding
         *  byte indent
         *  byte type
         * }
         */
        public byte[] GetNodeBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach (var n in scriptNodes) {
                
                bytes.AddRange(n.param_ptr!=null?BitConverter.GetBytes(n.param_ptr.offset): BitConverter.GetBytes(n.param));
                bytes.AddRange(BitConverter.GetBytes((short)n.indent));
                bytes.Add(n.indent);
                bytes.Add(n.type);
            }

            return bytes.ToArray();
        }
    }
}
