using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum USND_WaveFileFlags : byte {
		None = 0,
		External = 1 << 0,
		Flag1 = 1 << 1,
		Flag2 = 1 << 2,
		Flag3 = 1 << 3,
		Flag4 = 1 << 4,
		Flag5 = 1 << 5,
		Flag6 = 1 << 6,
		Flag7 = 1 << 7,
	}
}
