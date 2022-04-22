using System;
namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum MAT_TransformationType : uint {
		Uninitialized        = 0,
		Identity             = 1,
		Translation          = 2,
		Zoom                 = 3,
		Scale                = 4,
		Rotation             = 5,
		RotationZoom         = 6,
		RotationScale        = 7,
		ComplexRotationScale = 8,
	}
}
