using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_FloatColor : BaseColor {
		public GLI_FloatColor() { }
		public GLI_FloatColor(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

		public override void SerializeImpl(SerializerObject s) {
			Red = s.Serialize<float>(Red, name: nameof(Red));
			Green = s.Serialize<float>(Green, name: nameof(Green));
			Blue = s.Serialize<float>(Blue, name: nameof(Blue));
			Alpha = s.Serialize<float>(Alpha, name: nameof(Alpha));
		}
	}
}
