using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum DNM_SurfaceState : int {
		Error                   = -1,
		NoObstacle              = 0x00000000,
		Ground                  = 0x00000001,
		Slope                   = 0x00000002, // Removed
		Wall                    = 0x00000004,
		Attic                   = 0x00000008, // Removed
		Ceiling                 = 0x00000010,
		Trap                    = 0x00000020, // Removed
		Water                   = 0x00000040,
		ForceMobile             = 0x00000080,
		Mobile                  = 0x00010000,
		Father                  = 0x00030000,
	}
}