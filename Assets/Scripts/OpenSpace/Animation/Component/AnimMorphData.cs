using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimMorphData {

        public AnimMorphData() {}

        public static AnimMorphData Read(Reader reader) {
            AnimMorphData m = new AnimMorphData();
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                reader.ReadBytes(0x8); // Haven't deciphered this yet
            } else {
                reader.ReadBytes(0x26); // Haven't deciphered this yet
            }
            return m;
        }

        public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.R3: return 0x26;
                    default: return 0x8;
                }
            }
        }

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
