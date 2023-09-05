using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GAM_MiscFlags : byte {
		None = 0,
		DesactivateAtAll  = 1 << 0,
		Activable         = 1 << 1,
		Active            = 1 << 2,
		AllSecondPassDone = 1 << 3,
		Always            = 1 << 4,
		UselessCulling    = 1 << 5,
		AlwaysActive      = 1 << 6,
		TooFar            = 1 << 7

	}
}
