using System;
namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum POS_TransformationType : uint {
		Uninitialized        = 0,
		Identity             = 1,
		Translation          = 2,
		Rotation             = 3,
		Complete             = 4,
	}
}
