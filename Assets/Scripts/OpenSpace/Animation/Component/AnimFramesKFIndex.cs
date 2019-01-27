using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimFramesKFIndex {
        public uint kf;

        public AnimFramesKFIndex() {}

        public static AnimFramesKFIndex Read(Reader reader) {
            AnimFramesKFIndex kfi = new AnimFramesKFIndex();
            if (Settings.s.engineVersion == Settings.EngineVersion.TT || 
				Settings.s.game == Settings.Game.R2Demo || 
				Settings.s.game == Settings.Game.R2Revolution) {
                kfi.kf = reader.ReadUInt16();
            } else {
                kfi.kf = reader.ReadUInt32();
            }
            return kfi;
        }

        public static int Size {
            get {
                if (Settings.s.engineVersion == Settings.EngineVersion.TT || 
					Settings.s.game == Settings.Game.R2Demo ||
					Settings.s.game == Settings.Game.R2Revolution) {
                    return 2;
                } else return 4;
            }
        }

        public static bool Aligned {
            get {

				if (Settings.s.engineVersion == Settings.EngineVersion.TT ||
					Settings.s.game == Settings.Game.R2Demo ||
					Settings.s.game == Settings.Game.R2Revolution) {
					return false;
				} else {
					return true;
				}
            }
        }
    }
}
