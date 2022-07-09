using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum GAM_ActorCapabilities : uint {
		None = 0,
		Translate = 1 << 0,
		Walk = 1 << 1,
		Run = 1 << 2,
		Retreat = 1 << 3,
		Jump = 1 << 4,
		Fly = 1 << 5,
		HangHook = 1 << 6, // Accrocher
		HangSuspend = 1 << 7, // Suspendre
		Swim = 1 << 8,
		Balance = 1 << 9,
		Block = 1 << 10,
		Teleport = 1 << 11,
		Slide = 1 << 12,
		Inhale = 1 << 13,
		Custom14 = 1 << 14,
		Custom15 = 1 << 15,
		Custom16 = 1 << 16,
		Custom17 = 1 << 17,
		Custom18 = 1 << 18,
		Custom19 = 1 << 19,
		Custom20 = 1 << 20,
		Custom21 = 1 << 21,
		Custom22 = 1 << 22,
		Custom23 = 1 << 23,
		Custom24 = 1 << 24,
		Custom25 = 1 << 25,
		Custom26 = 1 << 26,
		Custom27 = 1 << 27,
		Custom28 = 1 << 28,
		Custom29 = 1 << 29,
		Custom30 = 1 << 30,
		HasAllCapabilities = (uint)1 << 31,
	}
}
