using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum DNM_ReportFlags : byte {
		None      = 0,
		Collision = 1 << 0,
		Wall      = 1 << 1, // Collision with wall
		Character = 1 << 2, // Contact with other character
	}
}