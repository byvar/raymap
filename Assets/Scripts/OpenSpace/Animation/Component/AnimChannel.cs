using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimChannel : OpenSpaceStruct {
        public ushort unk0;
        public short id;
        public ushort vector;
        public ushort numOfNTTO;
        public uint framesKF;
        public uint keyframe;

		protected override void ReadInternal(Reader reader) {
			unk0 = reader.ReadUInt16();
			id = reader.ReadInt16();
			vector = reader.ReadUInt16();
			numOfNTTO = reader.ReadUInt16();
			if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.TT
				&& CPA_Settings.s.game != CPA_Settings.Game.R2Revolution) {
				framesKF = reader.ReadUInt32();
				keyframe = reader.ReadUInt32();
			} else {
				framesKF = reader.ReadUInt16();
				keyframe = reader.ReadUInt16();
			}
		}

		/*public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.TT: return 12;
                    default:
						if (Settings.s.game == Settings.Game.R2Revolution) {
							return 12;
						} else {
							return 16;
						}
                }
            }
        }*/

        public static bool Aligned {
            get {
                if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.TT) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
