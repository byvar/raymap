using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_FloatColor_RGB : BaseColor {
		public GLI_FloatColor_RGB() { }
		public GLI_FloatColor_RGB(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

		public override float Alpha {
			get => 1f;
			set => _ = value;
		}

		public override void SerializeImpl(SerializerObject s) {
			Red = s.Serialize<float>(Red, name: nameof(Red));
			Green = s.Serialize<float>(Green, name: nameof(Green));
			Blue = s.Serialize<float>(Blue, name: nameof(Blue));
		}
	}
}
