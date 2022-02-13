using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Macro : BehaviorOrMacro {
        public string name = null;
        public LegacyPointer off_script;
        public LegacyPointer off_script2;
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
			if (CPA_Settings.s.hasNames
				&& CPA_Settings.s.platform != CPA_Settings.Platform.Xbox360
				&& CPA_Settings.s.platform != CPA_Settings.Platform.PS3
				&& CPA_Settings.s.platform != CPA_Settings.Platform.PS2) {
				name = reader.ReadString(0x100);

                // Some versions have extra information in the name, e.g.
                // rayman\YLT_RaymanModel\YLT_RaymanModel.rul^CreateIntelligence^CreateComport:YAM_C_Init
                int indexOf = name.IndexOf("CreateMacro:", StringComparison.Ordinal);
                if (indexOf >= 0) {
                    name = name.Substring(indexOf + "CreateMacro:".Length);
                }

			}

			off_script = LegacyPointer.Read(reader);
			off_script2 = LegacyPointer.Read(reader);

			LegacyPointer.DoAt(ref reader, off_script, () => {
				script = Script.Read(reader, LegacyPointer.Current(reader), this, single: true);
			});
		}
	}
}
