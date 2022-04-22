using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA {
	public static class MTH4D_VectorExtensions {
		public static Vector4 GetUnityVector(this MTH4D_Vector v) {
			return new Vector4(v.X, v.Y, v.Z, v.W);
		}
		public static Quaternion GetUnityQuaternion(this MTH4D_Vector v, bool convertAxes = false) {
			if (convertAxes) {
				return new Quaternion(v.X, v.Z, v.Y, -v.W);
			} else {
				return new Quaternion(v.X, v.Y, v.Z, v.W);
			}
		}
	}
}
