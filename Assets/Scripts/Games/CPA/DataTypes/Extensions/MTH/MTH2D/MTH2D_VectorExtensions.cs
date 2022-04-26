using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA {
	public static class MTH2D_VectorExtensions {
		public static Vector2 GetUnityVector(this MTH2D_Vector v) {
			return new Vector2(v.X, v.Y);
		}
	}
}
