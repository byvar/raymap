using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum CPA_EngineDefines {
		None = 0,
		Debug = 1 << 0,
		DebugAI = 1 << 1,
		DebugStringForPLA = 1 << 2,

		AllDebug = Debug | DebugAI | DebugStringForPLA
	}
}