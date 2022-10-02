using System;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class MTH3D_Vector_PS1_Int : BinarySerializable, ISerializerShortLog {
		public int X_Int { get; set; }
		public int Y_Int { get; set; }
		public int Z_Int { get; set; }

		public float CoordinateScale => Context.GetLevel().CoordinateFactor;
		public float X {
			get => X_Int / CoordinateScale;
			set => X_Int = (int)(value * CoordinateScale);
		}
		public float Y {
			get => Y_Int / CoordinateScale;
			set => Y_Int = (int)(value * CoordinateScale);
		}
		public float Z {
			get => Z_Int / CoordinateScale;
			set => Z_Int = (int)(value * CoordinateScale);
		}

		public override void SerializeImpl(SerializerObject s) {
			X_Int = s.Serialize<int>(X_Int, name: nameof(X_Int));
			Y_Int = s.Serialize<int>(Y_Int, name: nameof(Y_Int));
			Z_Int = s.Serialize<int>(Z_Int, name: nameof(Z_Int));
		}

		public string ShortLog => ToString();
		public override string ToString() => $"Vector({X}, {Y}, {Z})";

		public MTH3D_Vector_PS1_Int() { }
		public MTH3D_Vector_PS1_Int(float x, float y, float z) {
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
