using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimKeyframe {
        public ushort frame;
        public ushort flags;
        public ushort quaternion;
        public ushort quaternion2;
        public ushort vector;
        public ushort vector2;
        public double interpolationFactor;

        public static ushort flag_endKF = (1 << 7);

        public AnimKeyframe() {}

        public static AnimKeyframe Read(EndianBinaryReader reader) {
            AnimKeyframe kf = new AnimKeyframe();
            kf.frame = reader.ReadUInt16();
            kf.flags = reader.ReadUInt16();
            kf.quaternion = reader.ReadUInt16();
            kf.quaternion2 = reader.ReadUInt16();
            kf.vector = reader.ReadUInt16();
            kf.vector2 = reader.ReadUInt16();
            kf.interpolationFactor = (double)reader.ReadUInt16() * 0.00012207031;
            return kf;
        }

        public bool IsEndKeyframe {
            get {
                if((flags & flag_endKF) != 0) return true;
                return false;
            }
        }
    }
}
