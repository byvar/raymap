using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class MTH3D_Vector_PS1_Short : BinarySerializable {
		public short X_Short { get; set; }
		public short Y_Short { get; set; }
		public short Z_Short { get; set; }

		public float CoordinateScale => 100f;
		public float X {
			get => X_Short / CoordinateScale;
			set => X_Short = (short)(value * CoordinateScale);
		}
		public float Y {
			get => Y_Short / CoordinateScale;
			set => Y_Short = (short)(value * CoordinateScale);
		}
		public float Z {
			get => Z_Short / CoordinateScale;
			set => Z_Short = (short)(value * CoordinateScale);
		}

		public override void SerializeImpl(SerializerObject s) {
			X_Short = s.Serialize<short>(X_Short, name: nameof(X_Short));
			Y_Short = s.Serialize<short>(Y_Short, name: nameof(Y_Short));
			Z_Short = s.Serialize<short>(Z_Short, name: nameof(Z_Short));
		}

		public override bool UseShortLog => true;
		public override string ToString() => $"Vector({X}, {Y}, {Z})";

		public MTH3D_Vector_PS1_Short() { }
		public MTH3D_Vector_PS1_Short(float x, float y, float z) {
			X = x;
			Y = y;
			Z = z;
		}

		public double Magnitude => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
		public static MTH3D_Vector Zero => new MTH3D_Vector();
		public static MTH3D_Vector One => new MTH3D_Vector(1, 1, 1);

		public bool IsUniform => X == Y && X == Z;
	}
}
