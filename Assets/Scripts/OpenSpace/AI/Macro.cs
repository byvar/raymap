using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Macro : BehaviorOrMacro
    {
        public string name = null;
        public Pointer off_script;
        public Pointer off_script2;
		public Script script;


		public string ShortName {
			get {
				string shortName = "";
				if (name != null) {
					shortName = name;
					if (shortName.Contains("^CreateMacro:")) {
						shortName = shortName.Substring(shortName.LastIndexOf("^CreateMacro:") + 13);
					}
					shortName = "[\"" + shortName + "\"]";
				}
				shortName = aiModel.name + ".Macro[" + index + "]" + shortName;
				return shortName;
			}
		}
		public AIModel aiModel;
		public int index;

		public Macro(Pointer offset) {
            this.offset = offset;
        }

        public static Macro Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Macro m = new Macro(offset);

			if (Settings.s.hasNames) {
				m.name = reader.ReadString(0x100);
			}

            m.off_script = Pointer.Read(reader);
            m.off_script2 = Pointer.Read(reader);

			l.macros.Add(m);
            //if (m.name != null) l.print(m.name);

            if (m.off_script != null) {
                Pointer off_current = Pointer.Goto(ref reader, m.off_script);
                m.script = Script.Read(reader, Pointer.Current(reader), m);
                Pointer.Goto(ref reader, off_current);
            }
                
            return m;
        }

		public static Macro FromOffset(Pointer offset) {
			if (offset == null) return null;
			MapLoader l = MapLoader.Loader;
			return l.macros.FirstOrDefault(m => m.offset == offset);
		}

		public static Macro FromOffsetOrRead(Pointer offset, Reader reader) {
			if (offset == null) return null;
			Macro m = FromOffset(offset);
			if (m == null) {
				Pointer.DoAt(ref reader, offset, () => {
					m = Macro.Read(reader, offset);
				});
			}
			return m;
		}
	}
}
