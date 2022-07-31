using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum GEO_SpriteDrawMode : ushort {
		None = 0,
		Scaled2D = 1 << 0,
		Rotated2D = 1 << 1,
		Disable = 1 << 2,
		SemiLookAt = 1 << 3,
		LensFlare = 1 << 4,
		RevolutionLightCookie = 1 << 5,
	}
}