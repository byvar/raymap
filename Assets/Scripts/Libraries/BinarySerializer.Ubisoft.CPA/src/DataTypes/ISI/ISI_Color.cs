namespace BinarySerializer.Ubisoft.CPA {
	public class ISI_Color : BaseColor {
		public ISI_Color() { }
		public ISI_Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

		public short R { get; set; }
		public short G { get; set; }
		public short B { get; set; }
		public short A { get; set; }

		public override float Red {
			get => R / 255f;
			set => R = (short)(value * 255);
		}
		public override float Green {
			get => G / 255f;
			set => G = (short)(value * 255);
		}
		public override float Blue {
			get => B / 255f;
			set => B = (short)(value * 255);
		}
		public override float Alpha {
			get => A / 255f;
			set => A = (short)(value * 255);
		}

		public override void SerializeImpl(SerializerObject s) {
			R = s.Serialize<short>(R, name: nameof(R));
			G = s.Serialize<short>(G, name: nameof(G));
			B = s.Serialize<short>(B, name: nameof(B));
			A = s.Serialize<short>(A, name: nameof(A));
		}
	}
}
