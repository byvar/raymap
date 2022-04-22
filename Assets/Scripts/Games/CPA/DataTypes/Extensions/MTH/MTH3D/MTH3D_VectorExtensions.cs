using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA {
	public static class MTH3D_VectorExtensions {
		public static Vector3 GetUnityVector(this MTH3D_Vector v, bool convertAxes = false) {
			if (convertAxes) {
				return new Vector3(v.X, v.Z, v.Y);
			} else {
				return new Vector3(v.X, v.Y, v.Z);
			}
		}
	}
}
