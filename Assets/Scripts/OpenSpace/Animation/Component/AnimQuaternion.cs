using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimQuaternion {
        public Quaternion quaternion;

        public AnimQuaternion() {}

        public static AnimQuaternion Read(EndianBinaryReader reader) {
            AnimQuaternion qua = new AnimQuaternion();
            float x = (float)reader.ReadInt16() / (float)Int16.MaxValue;
            float z = (float)reader.ReadInt16() / (float)Int16.MaxValue;
            float y = (float)reader.ReadInt16() / (float)Int16.MaxValue;
            float w = (float)reader.ReadInt16() / (float)Int16.MaxValue;
            qua.quaternion = new Quaternion(x, y, z, w);
            return qua;
        }
    }
}
