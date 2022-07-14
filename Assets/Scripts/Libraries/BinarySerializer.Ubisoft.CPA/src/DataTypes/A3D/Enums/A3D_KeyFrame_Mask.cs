using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum A3D_KeyFrame_Mask : ushort {
		None                          = 0,
		QuaternionOrientation         = 1 << 0,
		QuaternionScale               = 1 << 1,
		ScaleValues                   = 1 << 2,
		Position                      = 1 << 3,
		AllLow                        = QuaternionOrientation | QuaternionScale | ScaleValues | Position,

		// Identity: no interpolation between this key and the next
		IdentityQuaternionOrientation = 1 << 4,
		IdentityQuaternionScale       = 1 << 5,
		IdentityScaleValues           = 1 << 6,

		LastKey                       = 1 << 7,
		ZeroAngleCenterPosition       = 1 << 8, // Interpolation is baded on Key0.PivotPosition + linear interpolation of DistMaster, instead of geodesic interpolation between positions.
		WrapLastKey			          = 1 << 9, // When LastKey is reached, interpolate with the first key

		ChangeOfHierarchy	          = 1 << 10,
		Hierarchized		          = 1 << 11,
		SoundEvent			          = 1 << 12,

		Unknown13 = 1 << 13,
		Unknown14 = 1 << 14,
		Unknown15 = 1 << 15,
	}
}