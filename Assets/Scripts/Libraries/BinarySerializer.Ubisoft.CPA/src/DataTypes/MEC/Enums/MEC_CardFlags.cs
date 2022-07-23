using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum MEC_CardFlags : uint {
		Animation        = 0x00000001, // animation speed?
		Collide          = 0x00000002, // collide with the map?
		Gravity          = 0x00000004, // gravity on?
		Tilt             = 0x00000008, // tilt in turn
		Gi               = 0x00000010, // gi
		OnGround         = 0x00000020, // put on ground
		Climb            = 0x00000040, // climb
		Spider           = 0x00000080, // Spider
		Shoot            = 0x00000100, // Shoot
		CollisionControl = 0x00000200, // use dynamics parameters when collide
		KeepWallSpeedZ   = 0x00000400, // keep z speed when collide a wall
		SpeedLimit       = 0x00000800, // limit speed?
		Inertia          = 0x00001000, // inertia?
		Stream           = 0x00002000, // stream?
		StickOnPlatform  = 0x00004000, // stick on platform?
		Scale            = 0x00008000, // use scale?
		PlatForm         = 0x00010000, // Platform?
		Swim             = 0x00020000, // Swim?
	}
}