using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimVector {
        public Vector3 vector;

        public AnimVector() {}

        public static AnimVector Read(EndianBinaryReader reader) {
            AnimVector vec = new AnimVector();
            float x = reader.ReadSingle();
            float z = reader.ReadSingle();
            float y = reader.ReadSingle();
            vec.vector = new Vector3(x, y, z);
            return vec;
        }
    }
}
