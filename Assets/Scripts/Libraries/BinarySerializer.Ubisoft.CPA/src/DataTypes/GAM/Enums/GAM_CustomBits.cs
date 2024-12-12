﻿using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GAM_CustomBits : uint {
		None = 0,
		UnseenFrozenAnimPlayer = 1 << 0,
		NeedModuleMatrices = 1 << 1,
		Movable = 1 << 2,
		Projectile = 1 << 3,
		RayTraceHit = 1 << 4,
		Targetable = 1 << 5,
		CannotCrushPrincipalActor = 1 << 6,
		Pickable = 1 << 7,
		ActorHasAShadow = 1 << 8,
		ShadowOnMe = 1 << 9,
		Prunable = 1 << 10,
		OutOfVisibility = 1 << 11, // Internal use only, unchangeable
		UnseenFrozen = 1 << 12,
		NoAnimPlayer = 1 << 13,
		Fightable = 1 << 14,
		NoMeca = 1 << 15,
		NoAI = 1 << 16,
		DestroyWhenAnimEnded = 1 << 17,
		NoAnimPlayerWhenTooFar = 1 << 18,
		NoAIPlayerWhenTooFar = 1 << 19,
		Unfreezable = 1 << 20,
		UsesTransparencyZone = 1 << 21,
		NoMecaWhenTooFar = 1 << 22,
		SoundWhenInvisible = 1 << 23,
		Protection = 1 << 24, // Internal use only, unchangeable
		CameraHit = 1 << 25,
		CanPushPrincipalActor = 1 << 26,
		DesignerBit1 = 1 << 27,
		DesignerBit2 = 1 << 28,
		DesignerBit3 = 1 << 29,
		DesignerBit4 = 1 << 30,
		PrincipalActor = (uint)1 << 31, // Internal use only, unchangeable
	}
}