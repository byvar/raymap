using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class MTH3D_Vector : BinarySerializable {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<float>(X, name: nameof(X));
			Y = s.Serialize<float>(Y, name: nameof(Y));
			Z = s.Serialize<float>(Z, name: nameof(Z));
		}
		public override bool UseShortLog => true;
		public override string ToString() => $"Vector({X}, {Y}, {Z})";

		public MTH3D_Vector() { }
		public MTH3D_Vector(float x, float y, float z) {
			X = x;
			Y = y;
			Z = z;
		}

		public double Magnitude => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
		public static MTH3D_Vector Zero => new MTH3D_Vector();
		public static MTH3D_Vector One => new MTH3D_Vector(1,1,1);

		public bool IsUniform => X == Y && X == Z;
	}
}
