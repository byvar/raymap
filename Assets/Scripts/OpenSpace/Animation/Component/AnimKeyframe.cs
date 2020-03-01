using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimKeyframe : OpenSpaceStruct {
        public float x;
        public float y;
        public float z;
        public float positionMultiplier = 1f;
        public ushort frame;
        public ushort flags;
        public ushort quaternion;
        public ushort quaternion2;
        public ushort scaleVector;
        public ushort positionVector;
        public double interpolationFactor;

        public static ushort flag_endKF = (1 << 7);

		protected override void ReadInternal(Reader reader) {
			if (Settings.s.engineVersion < Settings.EngineVersion.R3
				|| Settings.s.game == Settings.Game.RM
				|| Settings.s.game == Settings.Game.Dinosaur
                || (Settings.s.game == Settings.Game.RA && Settings.s.platform == Settings.Platform.PS2)) {
				x = reader.ReadSingle();
				y = reader.ReadSingle();
				z = reader.ReadSingle();
				positionMultiplier = reader.ReadSingle();
			}
			frame = reader.ReadUInt16();
			flags = reader.ReadUInt16();
			quaternion = reader.ReadUInt16();
			quaternion2 = reader.ReadUInt16();
			scaleVector = reader.ReadUInt16();

			positionVector = reader.ReadUInt16();
			if (Settings.s.engineVersion < Settings.EngineVersion.R3
				|| Settings.s.game == Settings.Game.RM
				|| Settings.s.game == Settings.Game.Dinosaur
                || (Settings.s.game == Settings.Game.RA && Settings.s.platform == Settings.Platform.PS2)) {
				reader.ReadUInt16();
				reader.ReadUInt16();
				reader.ReadUInt16();
			}
			interpolationFactor = (double)reader.ReadUInt16() * 0.00012207031;
		}

		public bool IsEndKeyframe {
            get {
                if((flags & flag_endKF) != 0) return true;
                return false;
            }
        }

        /*public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.R3:
						if (Settings.s.game == Settings.Game.Dinosaur || Settings.s.game == Settings.Game.RM) {
							return 36;
						} else {
							return 14;
						}
                    default: return 36;
                }
            }
        }*/

        public static bool Aligned {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
