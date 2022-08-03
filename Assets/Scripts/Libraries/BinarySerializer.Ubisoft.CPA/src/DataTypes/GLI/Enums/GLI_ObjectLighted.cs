using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GLI_ObjectLighted : byte {
		None = 0,
		Map = 1 << 0,
		Perso = 1 << 1,
	}
}
