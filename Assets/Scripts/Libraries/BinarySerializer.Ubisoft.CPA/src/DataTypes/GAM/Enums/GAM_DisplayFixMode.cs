using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GAM_DisplayFixMode : byte {
		Nothing = 0,
		HitPoints = 1,
		GameSave = 2,
		All = 0xFF,
	}
}
