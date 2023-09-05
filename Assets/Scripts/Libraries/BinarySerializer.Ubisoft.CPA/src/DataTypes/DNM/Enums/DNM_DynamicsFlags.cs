using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum DNM_DynamicsFlags : uint {
		None                    = 0,
		Animation    	        = 0x00000001, // animation speed ?
		Collide    		        = 0x00000002, // collide with the map ?
		Gravity    		        = 0x00000004, // gravity on ?
		Tilt    		        = 0x00000008, // tilt in turn
		Gi                      = 0x00000010, // gi
		OnGround    	        = 0x00000020, // put on ground
		Climb    		        = 0x00000040, // climb
		CollisionControl        = 0x00000080, // use dynamics parameters when collide
		KeepWallSpeedZ          = 0x00000100, // keep z speed when collide a wall
		SpeedLimit    	        = 0x00000200, // limit speed ?
		Inertia    		        = 0x00000400, // inertia ?
		Stream    		        = 0x00000800, // stream ?
		StickOnPlatform         = 0x00001000, // stick on platform ?
		Scale    		        = 0x00002000, // use scale ?
		AbsoluteImposeSpeed     = 0x00004000, // absolute speed ?
		AbsoluteProposeSpeed	= 0x00008000, // absolute speed ?
		AbsoluteAddSpeed		= 0x00010000, // absolute speed ?
		ImposeSpeedX            = 0x00020000, // impose x ?
		ImposeSpeedY            = 0x00040000, // impose y ?
		ImposeSpeedZ            = 0x00080000, // impose z ?
		ProposeSpeedX           = 0x00100000, // propose x ?
		ProposeSpeedY           = 0x00200000, // propose y ?
		ProposeSpeedZ           = 0x00400000, // propose z ?
		AddSpeedX    	        = 0x00800000, // add x ?
		AddSpeedY    	        = 0x01000000, // add y ?
		AddSpeedZ    	        = 0x02000000, // add z ?
		LimitX    		        = 0x04000000, // limit x ?
		LimitY    		        = 0x08000000, // limit y ?
		LimitZ    		        = 0x10000000, // limit z ?
		ImposeRotation          = 0x20000000, // impose rotation ?
		LockPlatform            = 0x40000000, // lock the link between platform and actor
		ImposeTranslation       = 0x80000000, // impose translation ?
	}
}