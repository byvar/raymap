using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class MTH3D_Vector_PS1Extensions {
		public static Vector3 GetUnityVector(this MTH3D_Vector_PS1_Short v, bool convertAxes = false) {
			if (convertAxes) {
				return new Vector3(v.X, v.Z, v.Y);
			} else {
				return new Vector3(v.X, v.Y, v.Z);
			}
		}
		public static Vector3 GetUnityVector(this MTH3D_Vector_PS1_Int v, bool convertAxes = false) {
			if (convertAxes) {
				return new Vector3(v.X, v.Z, v.Y);
			} else {
				return new Vector3(v.X, v.Y, v.Z);
			}
		}
	}
}
