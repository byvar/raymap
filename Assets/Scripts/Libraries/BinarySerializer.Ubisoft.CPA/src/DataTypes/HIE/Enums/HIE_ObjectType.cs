using System;
namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum HIE_ObjectType : uint {
		SuperObject		 = 0x00000001, // SO with no linked object
		Actor			 = 0x00000002, // actor
		Sector			 = 0x00000004, // sector
		PO				 = 0x00000008, // Physical Object
		PO_Mirror		 = 0x00000010, // Physical Object Mirror (?)
		IPO				 = 0x00000020, // Instanciated Physical Object
		IPO_Mirror		 = 0x00000040, // Instanciated Physical Object Mirror (?)
		SpecialEffect	 = 0x00000080, // Special Effect (?)
		NoAction		 = 0x00000100, // empty SO (?)
		Mirror			 = 0x00000200, // Mirror (?)
		EDT_Geometric	 = 0x00000400, // geometric object (Editor)
		EDT_Light		 = 0x00000800, // light object (Editor)
		EDT_Waypoint	 = 0x00001000, // Waypoint object (Editor)
		EDT_ZdD			 = 0x00002000, // ZdD object (Editor)
		EDT_ZdE			 = 0x00004000, // ZdE object (Editor)
		EDT_ZdM			 = 0x00008000, // ZdM object (Editor)
		EDT_ZdR			 = 0x00010000, // ZdR object (Editor)
		EDT_BdV			 = 0x00020000, // Bounding Volume object (Editor)
		EDT_TestPoint	 = 0x00040000, // TestPoint object (Editor)
	}
}
