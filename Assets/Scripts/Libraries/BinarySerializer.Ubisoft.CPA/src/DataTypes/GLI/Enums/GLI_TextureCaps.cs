using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GLI_TextureCaps : uint {
		None            = 0,
		Tiled           = 1 << 0,
		NZ              = 1 << 1,
		MipMap          = 1 << 2,
		Alpha           = 1 << 3,
		NZFiltered      = 1 << 4,
		AddTransparency = 1 << 5,
		NoZBufferWrite  = 1 << 6,
		Palette         = 1 << 7,
		AlphaTest       = 1 << 8,
		AAA             = 1 << 9,
		MAA             = 1 << 10,
		Procedural      = 1 << 11,
		
		BMP             = 1 << 30,
		TGA             = (uint)1 << 31,
	}
}
