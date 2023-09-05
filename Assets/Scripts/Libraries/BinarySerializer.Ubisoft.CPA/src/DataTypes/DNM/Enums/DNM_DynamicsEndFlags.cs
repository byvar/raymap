using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum DNM_DynamicsEndFlags : uint {
		None                    = 0,
		BaseSize                = 0x00000001, // Base
		AdvancedSize            = 0x00000002, // Advanced
		ComplexSize             = 0x00000004, // Complex
		Reserved                = 0x00000008, // reserved
		MechanicChange          = 0x00000010, // The machanic has just changed
		PlatformCrash           = 0x00000020, // actor takes a platform on the mug
		CanFall                 = 0x00000040, // actor walk on an edge ground limit
		Init                    = 0x00000080, // mechanic must be initialized
		Spider                  = 0x00000100, // spider mechanic
		Shoot                   = 0x00000200, // shoot option ?
		SafeValid               = 0x00000400, // safe translation valid ?
		ComputeInvertMatrix     = 0x00000800, // compute invert matrix
		ChangeScale             = 0x00001000, // change scale
		Exec                    = 0x00002000, // mechanic execution flag
		CollisionReport         = 0x00004000, // Collision
		NoGravity               = 0x00008000, // No gravity
		Stop                    = 0x00010000, // The mechanic can't realize the request
		SlidingGround           = 0x00020000, // The actor is on a sliding ground
		Always                  = 0x00040000, // always dynamic
		Crash                   = 0x00080000, // actor is crashed by another actor
		Swim                    = 0x00100000, // swim
		NeverFall               = 0x00200000, // if CanFall is set, don't fall
		Hanging                 = 0x00400000, // Hanging mechanic
		WallAdjust              = 0x00800000, // we must check wall collision next time
		ActorMove               = 0x01000000, // actor move ?
		ForceSafeWalk           = 0x02000000, // safe walk mechanic next time ?
		DontUseNewMechanic      = 0x04000000, // Use new mechanic ?
	}
}