using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimKeyframe {
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

        public AnimKeyframe() {}

        public static AnimKeyframe Read(Reader reader) {
            MapLoader l = MapLoader.Loader;
            AnimKeyframe kf = new AnimKeyframe();
            if (Settings.s.engineVersion < Settings.EngineVersion.R3 || Settings.s.game == Settings.Game.RM) {
                kf.x = reader.ReadSingle();
                kf.y = reader.ReadSingle();
                kf.z = reader.ReadSingle();
                kf.positionMultiplier = reader.ReadSingle();
            }
            kf.frame = reader.ReadUInt16();
            kf.flags = reader.ReadUInt16();
            kf.quaternion = reader.ReadUInt16();
            kf.quaternion2 = reader.ReadUInt16();
            kf.scaleVector = reader.ReadUInt16();
            kf.positionVector = reader.ReadUInt16();
            if (Settings.s.engineVersion < Settings.EngineVersion.R3 || Settings.s.game == Settings.Game.RM) {
                reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt16();
            }
            kf.interpolationFactor = (double)reader.ReadUInt16() * 0.00012207031;
            return kf;
        }

        public bool IsEndKeyframe {
            get {
                if((flags & flag_endKF) != 0) return true;
                return false;
            }
        }

        public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.R3: return 14;
                    default: return 36;
                }
            }
        }

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
