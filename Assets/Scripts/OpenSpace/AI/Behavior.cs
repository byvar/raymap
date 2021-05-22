using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace OpenSpace.AI {
    public class Behavior : BehaviorOrMacro {
		public enum BehaviorType {
			Intelligence,
			Reflex
		}

        public string name = null;
        public Pointer off_scripts;
        private Pointer off_firstScript;
        public byte num_scripts;
        public Script[] scripts;
        public Script firstScript;

        // Custom
        public List<Pointer> copies;
        public BehaviorType type;
        public int index;

        public List<Script> referencedBy = new List<Script>();

        public string ShortName {
			get {
                return GetShortName(aiModel, type, index);
			}
		}
        public string GetShortName(AIModel model, BehaviorType type, int index) {
            string shortName = "";
            if (name != null) {
                shortName = name;
                //string comportNamePattern = @"^(?<family>[^\\]+?)\\(?<model>[^\\]+?)\\(?<model2>[^\\]+?)\.(?<type>...?)\^CreateIntelligence\^CreateComport:(?<name>.*?)$";
                if (shortName.Contains("^CreateComport:")) {
                    shortName = shortName.Substring(shortName.LastIndexOf("^CreateComport:") + 15);
                }
                shortName = "[\"" + shortName + "\"]";
            }
            shortName = (model!=null?model.name:"") + "." + type + "[" + index + "]" + shortName;
            return shortName;
        }

        public string VeryShortName
        {
            get
            {
                string shortName = "";
                if (name != null) {
                    shortName = name;
                    //string comportNamePattern = @"^(?<family>[^\\]+?)\\(?<model>[^\\]+?)\\(?<model2>[^\\]+?)\.(?<type>...?)\^CreateIntelligence\^CreateComport:(?<name>.*?)$";
                    if (shortName.Contains("^CreateComport:")) {
                        shortName = shortName.Substring(shortName.LastIndexOf("^CreateComport:") + 15);
                    }
                    shortName = "[\"" + shortName + "\"]";
                }
                return shortName;
            }
        }

        public string NameSubstring {
            get {
                string shortName = "";
                if (name != null) {
                    shortName = name;
                    //string comportNamePattern = @"^(?<family>[^\\]+?)\\(?<model>[^\\]+?)\\(?<model2>[^\\]+?)\.(?<type>...?)\^CreateIntelligence\^CreateComport:(?<name>.*?)$";
                    if (shortName.Contains("^CreateComport:")) {
                        shortName = shortName.Substring(shortName.LastIndexOf("^CreateComport:") + 15);
                    }
                }
                return shortName;
            }
        }

        public Behavior() : base() {
			copies = new List<Pointer>();
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

        protected override void ReadInternal(Reader reader) {
            MapLoader l = MapLoader.Loader;
            l.behaviors.Add(this);
            //l.print("Behavior " + Offset);
            if (Settings.s.hasNames && Settings.s.platform != Settings.Platform.PS2) {
                name = new string(reader.ReadChars(0x100)).TrimEnd('\0');

                // Some versions have extra information in the name, e.g.
                // rayman\YLT_RaymanModel\YLT_RaymanModel.rul^CreateIntelligence^CreateComport:YAM_C_Init
                int indexOf = name.IndexOf("CreateComport:", StringComparison.Ordinal);
                if (indexOf >= 0) {
                    name = name.Substring(indexOf + "CreateComport:".Length);
                }
            }
            off_scripts = Pointer.Read(reader);
            off_firstScript = Pointer.Read(reader);
            if (Settings.s.platform == Settings.Platform.DC || 
                Settings.s.game == Settings.Game.RedPlanet
                || Settings.s.game == Settings.Game.R2Demo) {
                reader.ReadUInt32();
            }
            num_scripts = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            //if (entry.name != null) l.print(entry.name);
            scripts = new Script[num_scripts];
            Pointer.DoAt(ref reader, off_scripts, () => {
                for (int i = 0; i < num_scripts; i++) {
                    scripts[i] = Script.Read(reader, Pointer.Current(reader), this);
                }
            });
            Pointer.DoAt(ref reader, off_firstScript, () => {
                firstScript = Script.Read(reader, Pointer.Current(reader), this, single: true);
            });
        }

        public override string ToString() {
			return ShortName;
        }
    }
}
