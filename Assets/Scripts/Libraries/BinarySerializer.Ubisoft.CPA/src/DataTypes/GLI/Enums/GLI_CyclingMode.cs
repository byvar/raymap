using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GLI_CyclingMode : byte {
		Off = 0,

		CyclingU = 1 << 0,
		CyclingV = 1 << 1,
		CyclingUV = CyclingU | CyclingV,

		MirrorU = 1 << 2,
		MirrorV = 1 << 3,
		MirrorUV = MirrorU | MirrorV,

		SpecialMirrorForShadow = 1 << 4
	}
}
