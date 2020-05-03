using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1AnimationQuaternion : OpenSpaceStruct {
		public short x;
		public short y;
		public short z;
		public short w;
        public Quaternion quaternion;

        protected override void ReadInternal(Reader reader) {
			x = reader.ReadInt16();
			y = reader.ReadInt16();
			z = reader.ReadInt16();
			w = reader.ReadInt16();
            quaternion = new Quaternion(
                x / (float)Int16.MaxValue,
                y / (float)Int16.MaxValue,
                z / (float)Int16.MaxValue,
                w / (float)Int16.MaxValue);
            ConvertRotation();
        }

        public Matrix ToMatrix() {
            Matrix4x4 m = new Matrix4x4();
            float x = quaternion.x;
            float y = quaternion.y;
            float z = quaternion.z;
            float w = quaternion.w;
            float qMagnitude = 1.0f / Mathf.Sqrt(x * x + y * y + z * z + w * w);
            x *= qMagnitude;
            y *= qMagnitude;
            z *= qMagnitude;
            w *= qMagnitude;

            float twoX = 2 * x;
            float twoY = 2 * y;
            float twoZ = 2 * z;
            float xw = twoX * w;
            float yw = twoY * w;
            float zw = twoZ * w;
            float xx = twoX * x;
            float yx = twoY * x;
            float zx = twoZ * x;
            float yy = twoY * y;
            float zy = twoZ * y;
            float zz = twoZ * z;
            m.m00 = 1.0f - (zz + yy);
            m.m01 = yx + zw;
            m.m02 = zx - yw;
            m.m10 = yx - zw;
            m.m11 = 1.0f - (zz + xx);
            m.m12 = zy + xw;
            m.m20 = zx + yw;
            m.m21 = zy - xw;
            m.m22 = 1.0f - (yy + xx);
            m.SetColumn(3, new Vector4(0, 0, 0, 1f));
            m.SetRow(3, new Vector4(0, 0, 0, 1f));
            Matrix mat = new Matrix(null, 8, m, Vector4.one);
            return mat;
        }

        public void ConvertRotation() {
            quaternion = ToMatrix().GetRotation(convertAxes: true);
        }
    }
}
