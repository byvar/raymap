using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public enum HIE_ObjectType_98 : uint {
		SuperObject = 0,
		Physical,
		Light,
		Camera,
		Character,
		Animation,
		StaticWorld,
		StandardPlatForm,
		Sector,
		Waypoint,
		Geometric,
		PhysicalObject,
		SpecialEffect,
		InstanciatedPhysicalObject,
		A3dPickableObject,
		A3dUnpickableObject,
		A3dInvisibleObject,
		Mirror,
		PoMirror,
		IpoMirror,
		World = 64,
		StaticSubWorld,
		EngineSubWorld,
		InactiveEngineSubWorld,
		NoAction = 128
	}
}
