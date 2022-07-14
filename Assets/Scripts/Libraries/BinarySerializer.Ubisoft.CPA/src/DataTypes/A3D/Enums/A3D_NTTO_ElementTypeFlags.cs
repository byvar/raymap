using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum A3D_NTTO_ElementTypeFlags : byte {
		None                = 0,
		WrapLastKey			= 1 << 1,
		ChangeOfHierarchy	= 1 << 2,
		Hierarchized		= 1 << 3,
		SoundEvent			= 1 << 4,
	}
}