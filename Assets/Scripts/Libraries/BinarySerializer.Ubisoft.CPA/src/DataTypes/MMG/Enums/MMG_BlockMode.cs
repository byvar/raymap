using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum MMG_BlockMode : byte {
		Unknown          = 0,
		NoFree           = 1 << 0,
		FillWithCleanKey = 1 << 1,
		CheckAlignment   = 1 << 2,
		CheckOverflow    = 1 << 3,
	}
}
