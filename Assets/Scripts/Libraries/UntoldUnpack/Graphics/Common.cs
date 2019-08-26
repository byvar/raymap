using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UntoldUnpack.Graphics {
	public enum PicaDataTypes : uint {
		Byte = 0x1400,
		UnsignedByte = 0x1401,
		Short = 0x1402,
		UnsignedShort = 0x1403,
		Int = 0x1404,
		UnsignedInt = 0x1405,
		Float = 0x1406,
		UnsignedByte44DMP = 0x6760,
		Unsigned4BitsDMP = 0x6761,
		UnsignedShort4444 = 0x8033,
		UnsignedShort5551 = 0x8034,
		UnsignedShort565 = 0x8363
	}

	public enum PicaPixelFormats : uint {
		RGBANativeDMP = 0x6752,
		RGBNativeDMP = 0x6754,
		AlphaNativeDMP = 0x6756,
		LuminanceNativeDMP = 0x6757,
		LuminanceAlphaNativeDMP = 0x6758,
		ETC1RGB8NativeDMP = 0x675A,
		ETC1AlphaRGB8A4NativeDMP = 0x675B
	}

	public static class Common {
		public static byte ResampleChannel(int value, int sourceBits, int targetBits) {
			byte sourceMask = (byte)((1 << sourceBits) - 1);
			byte targetMask = (byte)((1 << targetBits) - 1);
			return (byte)((((value & sourceMask) * targetMask) + (sourceMask >> 1)) / sourceMask);
		}
	}
}
