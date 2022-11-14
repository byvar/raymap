using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class MTH2D_Vector : BinarySerializable, ISerializerShortLog {
		public float X { get; set; }
		public float Y { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<float>(X, name: nameof(X));
			Y = s.Serialize<float>(Y, name: nameof(Y));
		}
		public string ShortLog => ToString();
		public override string ToString() => $"Vector({X}, {Y})";

		public MTH2D_Vector() { }
		public MTH2D_Vector(float x, float y) {
			X = x;
			Y = y;
		}

		public double Magnitude => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
		public bool IsUniform => X == Y;
	}
}
