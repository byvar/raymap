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
        public ushort scaleVector;
        public ushort positionVector;
        public double interpolationFactor;

        public static ushort flag_endKF = (1 << 7);

        public AnimKeyframe() {}

        public static AnimKeyframe Read(EndianBinaryReader reader) {
            AnimKeyframe kf = new AnimKeyframe();
            kf.frame = reader.ReadUInt16();
            kf.flags = reader.ReadUInt16();
            kf.quaternion = reader.ReadUInt16();
            kf.quaternion2 = reader.ReadUInt16();
            kf.scaleVector = reader.ReadUInt16();
            kf.positionVector = reader.ReadUInt16();
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
