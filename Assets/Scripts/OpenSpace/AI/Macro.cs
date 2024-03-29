﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Macro : BehaviorOrMacro {
        public string name = null;
        public Pointer off_script;
        public Pointer off_script2;
		public Script script;

		// Custom
		public int index;

		public string ShortName {
			get {
				return GetShortName(aiModel, index);
			}
		}
		public string GetShortName(AIModel model, int index) {
			string shortName = "";
			if (name != null) {
				shortName = name;
				if (shortName.Contains("^CreateMacro:")) {
					shortName = shortName.Substring(shortName.LastIndexOf("^CreateMacro:") + 13);
				}
				shortName = "[\"" + shortName + "\"]";
			}
			shortName = model.name + ".Macro[" + index + "]" + shortName;
			return shortName;
		}
		public string NameSubstring {
			get {
				string shortName = "";
				if (name != null) {
					shortName = name;
					if (shortName.Contains("^CreateMacro:")) {
						shortName = shortName.Substring(shortName.LastIndexOf("^CreateMacro:") + 13);
					}
				} else {
                    return "Macro_" + index;
                }
				return shortName;
			}
		}


		protected override void ReadInternal(Reader reader) {
			MapLoader.Loader.macros.Add(this);
			if (Settings.s.hasNames
				&& Settings.s.platform != Settings.Platform.Xbox360
				&& Settings.s.platform != Settings.Platform.PS3
				&& Settings.s.platform != Settings.Platform.PS2) {
				name = reader.ReadString(0x100);

                // Some versions have extra information in the name, e.g.
                // rayman\YLT_RaymanModel\YLT_RaymanModel.rul^CreateIntelligence^CreateComport:YAM_C_Init
                int indexOf = name.IndexOf("CreateMacro:", StringComparison.Ordinal);
                if (indexOf >= 0) {
                    name = name.Substring(indexOf + "CreateMacro:".Length);
                }

			}

			off_script = Pointer.Read(reader);
			off_script2 = Pointer.Read(reader);

			Pointer.DoAt(ref reader, off_script, () => {
				script = Script.Read(reader, Pointer.Current(reader), this, single: true);
			});
		}
	}
}
