using System;

namespace BinarySerializer.Ubisoft.CPA {
	public enum GAM_ObjectsTableTargetType : ushort {
		PhysicalObject = 0,
		Animation = 1,
		Light = 2,
		Camera = 3,
		Mirror = 4,
		Event = 5,

		Undefined = 0xFFFF,
	}
}
