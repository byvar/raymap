using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_FloatColor_RGBA : BaseColor {
		public GLI_FloatColor_RGBA() { }
		public GLI_FloatColor_RGBA(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

		public override void SerializeImpl(SerializerObject s) {
			Red = s.Serialize<float>(Red, name: nameof(Red));
			Green = s.Serialize<float>(Green, name: nameof(Green));
			Blue = s.Serialize<float>(Blue, name: nameof(Blue));
			Alpha = s.Serialize<float>(Alpha, name: nameof(Alpha));
		}
	}
}
