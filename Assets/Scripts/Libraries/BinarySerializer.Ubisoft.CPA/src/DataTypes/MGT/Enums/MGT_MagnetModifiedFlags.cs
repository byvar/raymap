using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum MGT_MagnetModifiedFlags : byte {
		None = 0,
		Strength = 1 << 0,
		NearFar  = 1 << 1,
		Position = 1 << 2
	}
}
