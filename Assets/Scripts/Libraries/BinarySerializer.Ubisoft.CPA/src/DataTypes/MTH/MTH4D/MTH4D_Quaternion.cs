using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class MTH4D_Quaternion : MTH4D_Vector {
		public override string ToString() => $"Quaternion({X}, {Y}, {Z}, {W})";

		public MTH4D_Quaternion() : base() {}
		public MTH4D_Quaternion(float x, float y, float z, float w) : base(x,y,z,w) {}
	}
}
