using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimFramesKFIndex : OpenSpaceStruct {
        public uint kf;

		protected override void ReadInternal(Reader reader) {
			if (Settings.s.engineVersion == Settings.EngineVersion.TT ||
				Settings.s.game == Settings.Game.R2Demo ||
				Settings.s.game == Settings.Game.Donald_BinRP ||
				Settings.s.game == Settings.Game.R2Revolution) {
				kf = reader.ReadUInt16();
			} else {
				kf = reader.ReadUInt32();
			}
		}

		/*public static int Size {
            get {
                if (Settings.s.engineVersion == Settings.EngineVersion.TT || 
					Settings.s.game == Settings.Game.R2Demo ||
					Settings.s.game == Settings.Game.R2Revolution) {
                    return 2;
                } else return 4;
            }
        }*/

        public static bool Aligned {
            get {

				if (Settings.s.engineVersion == Settings.EngineVersion.TT ||
					Settings.s.game == Settings.Game.R2Demo ||
					Settings.s.game == Settings.Game.Donald_BinRP ||
					Settings.s.game == Settings.Game.R2Revolution) {
					return false;
				} else {
					return true;
				}
            }
        }
    }
}
