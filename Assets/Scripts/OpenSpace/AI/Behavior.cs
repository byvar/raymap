using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Behavior : BehaviorOrMacro {
		public enum BehaviorType {
			Rule,
			Reflex
		}

		public List<Pointer> copies;

        public string name = null;
        public Pointer off_scripts;
        private Pointer off_firstScript;
        public byte num_scripts;
        public Script[] scripts;
        public Script firstScript;


		public string ShortName {
			get {
				string shortName = "";
				if (name != null) {
					shortName = name;
					//string comportNamePattern = @"^(?<family>[^\\]+?)\\(?<model>[^\\]+?)\\(?<model2>[^\\]+?)\.(?<type>...?)\^CreateIntelligence\^CreateComport:(?<name>.*?)$";
					if (shortName.Contains("^CreateComport:")) {
						shortName = shortName.Substring(shortName.LastIndexOf("^CreateComport:") + 15);
					}
					shortName = "[\"" + shortName + "\"]";
				}
				shortName = aiModel.name + "." + type.ToString() + "[" + index +  "]" + shortName;
				return shortName;
			}
		}
		public AIModel aiModel;
		public BehaviorType type;
		public int index;

		public Behavior(Pointer offset) {
            this.offset = offset;
			copies = new List<Pointer>();
        }

        public static Behavior FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.behaviors.FirstOrDefault(b => (b.offset == offset) || (Settings.s.platform == Settings.Platform.DC && b.copies.Contains(offset)));
        }

		public static Behavior FromOffsetOrRead(Pointer offset, Reader reader) {
			if (offset == null) return null;
			Behavior b = FromOffset(offset);
			if (b == null) {
				Pointer.DoAt(ref reader, offset, () => {
					b = Behavior.Read(reader, offset);
				});
			}
			return b;
		}

		public bool ContentEquals(Behavior b) {
			if (firstScript != null) {
				if (b.firstScript == null || !firstScript.ContentEquals(b.firstScript)) return false;
			} else if (b.firstScript != null) return false;
			if (scripts.Length != b.scripts.Length) return false;
			for (int i = 0; i < scripts.Length; i++) {
				if (scripts[i] != null) {
					if (b.scripts[i] == null || !scripts[i].ContentEquals(b.scripts[i])) return false;
				} else if (b.scripts[i] != null) return false;
			}
			return true;
		}

		/*public static Behavior FromOffsetOrRead(Pointer offset, Reader reader, AIModel aiModel, BehaviorType type, int number) {
            if (offset == null) return null;
            Behavior b = FromOffset(offset);
            if (b == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    b = Behavior.Read(reader, offset, aiModel, type, number);
                });
            }
            return b;
        }*/

		public static Behavior Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Behavior behavior = new Behavior(offset);

            if (Settings.s.hasNames) {
                behavior.name = new string(reader.ReadChars(0x100)).TrimEnd('\0');
            }
            behavior.off_scripts = Pointer.Read(reader);
            behavior.off_firstScript = Pointer.Read(reader);
            if (Settings.s.game == Settings.Game.R2Demo || Settings.s.platform == Settings.Platform.DC) {
                reader.ReadUInt32();
            }
            behavior.num_scripts = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            //if (entry.name != null) l.print(entry.name);
            behavior.scripts = new Script[behavior.num_scripts];
            Pointer.DoAt(ref reader, behavior.off_scripts, () => {
                for (int i = 0; i < behavior.num_scripts; i++) {
                    behavior.scripts[i] = Script.Read(reader, Pointer.Current(reader), behavior);
                }
            });
            Pointer.DoAt(ref reader, behavior.off_firstScript, () => {
                behavior.firstScript = Script.Read(reader, Pointer.Current(reader), behavior);
            });

            l.behaviors.Add(behavior);

            return behavior;
        }

        public override string ToString() {
			return ShortName;
        }
    }
}
