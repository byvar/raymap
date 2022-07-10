using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GLI_BilinearMode : byte {
		Off = 0,

		U = 1 << 0,
		V = 1 << 1,
		UV = U | V,
	}
}
