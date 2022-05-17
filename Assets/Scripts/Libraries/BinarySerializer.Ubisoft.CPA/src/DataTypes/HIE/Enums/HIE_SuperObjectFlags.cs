using System;
namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
	public enum HIE_SuperObjectFlags : uint {
		NotPickable            = 0x00000001, // no collision
		Hidden                 = 0x00000002, // not visible
		NoTransformationMatrix = 0x00000004, // The transformation matrix is always the identity 
		ZoomInsteadOfScale     = 0x00000008, // The scale factor is the same for the three axis
		TypeOfBoundingVolume   = 0x00000010, // bounding volume is a sphere instead of a box
		Superimposed           = 0x00000020, // displayed over all C - 0 ; non collisionnable
		NotHitByRayTrace       = 0x00000040, // cannot be hit by ray-tracing
		NoShadowOnMe           = 0x00000080, // cannot have a shadow projected on it
		SemiLookAt             = 0x00000100, 
		CheckChildren          = 0x00000200, // SuperObject that has one or more children that doesn't have their bounding volume included in the parent's BV
		RenderOnNearPlane      = 0x00000400, // Render on near plane, for Hud, etc
		MagnetModification     = 0x00008000, 
		ModuleTransparency     = 0x00010000, // module transparency is setting
		ExcluLight             = 0x00020000, 
	}
}
