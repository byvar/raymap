using System;
using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_RGBA8888Color : BaseBitwiseColor {
		public GLI_RGBA8888Color() { }
		public GLI_RGBA8888Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
		public GLI_RGBA8888Color(uint colorValue) : base(colorValue) { }

		protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

		protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>() {
			[ColorChannel.Red] = new ColorChannelFormat(0, 8),
			[ColorChannel.Green] = new ColorChannelFormat(8, 8),
			[ColorChannel.Blue] = new ColorChannelFormat(16, 8),
			[ColorChannel.Alpha] = new ColorChannelFormat(24, 8)
		};
	}
}
