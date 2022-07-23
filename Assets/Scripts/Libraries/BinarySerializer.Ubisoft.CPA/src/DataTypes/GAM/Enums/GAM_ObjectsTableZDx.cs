using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GAM_ObjectsTableZDx : ushort {
		None = 0,
		HasZDD = 1 << 0,
		HasZDE = 1 << 1,
	}
}
