using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimFramesKFIndex : OpenSpaceStruct {
        public uint kf;

		protected override void ReadInternal(Reader reader) {
			if (Legacy_Settings.s.engineVersion == Legacy_Settings.EngineVersion.TT ||
				Legacy_Settings.s.game == Legacy_Settings.Game.R2Demo ||
				Legacy_Settings.s.game == Legacy_Settings.Game.RedPlanet ||
				Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
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

				if (Legacy_Settings.s.engineVersion == Legacy_Settings.EngineVersion.TT ||
					Legacy_Settings.s.game == Legacy_Settings.Game.R2Demo ||
					Legacy_Settings.s.game == Legacy_Settings.Game.RedPlanet ||
					Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
					return false;
				} else {
					return true;
				}
            }
        }
    }
}
