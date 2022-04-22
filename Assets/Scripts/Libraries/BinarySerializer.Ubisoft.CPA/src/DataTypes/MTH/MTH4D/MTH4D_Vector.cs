using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class MTH4D_Vector : BinarySerializable {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<float>(X, name: nameof(X));
			Y = s.Serialize<float>(Y, name: nameof(Y));
			Z = s.Serialize<float>(Z, name: nameof(Z));
			W = s.Serialize<float>(W, name: nameof(W));
		}
		public override bool UseShortLog => true;
		public override string ToString() => $"Vector({X}, {Y}, {Z}, {W})";

		public MTH4D_Vector() { }
		public MTH4D_Vector(float x, float y, float z, float w) {
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public double Magnitude => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2) + Math.Pow(W, 2));
		public static MTH4D_Vector IdentityQuaternion => new MTH4D_Vector(0,0,0,1);
		public bool IsUniform => X == Y && X == Z && X == W;
	}
}
