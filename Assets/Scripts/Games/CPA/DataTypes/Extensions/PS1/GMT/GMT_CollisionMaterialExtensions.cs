using UnityEngine;
using Raymap;
using static BinarySerializer.Ubisoft.CPA.PS1.GLI_VisualMaterial;
using Colors = OpenSpace.Collide.CollideMaterial.Colors;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class GMT_CollisionMaterialExtensions {
		public static Material CreateMaterial(this GMT_CollisionMaterial cmt) {
			var env = ((Unity_Environment_CPA)cmt.Context.GetUnityEnvironment());
			Material mat = new Material(env.MaterialCollision);
			// TODO: use col types
			var colTypes = cmt.Context.GetCPASettings().COLTypes;
			/*if (NoCollision) {
				mat = new Material(env.MaterialCollisionTransparent);
				//mat.SetTexture("_MainTex", Util.CreateDummyCheckerTexture());
				mat.color = Colors.NoCollision;
			}
			if (Slide) mat.color = Colors.Slide;
			if (Water) {
				mat = new Material(env.MaterialCollisionTransparent);
				//mat.SetTexture("_MainTex", Util.CreateDummyCheckerTexture());
				mat.color = Colors.Water;
			}
			if (WaterSplash) {
				mat.color = Colors.Water;
			}
			if (ClimbableWall || HangableCeiling) {
				mat.color = Colors.Climbable;
			}
			if (LavaDeathWarp || DeathWarp) {
				mat.color = Colors.DeathWarp;
			}
			if (HurtTrigger) mat.color = Colors.HurtTrigger;
			//if (FallTrigger) mat.color = Colors.FallTrigger;
			if (Trampoline) mat.color = Colors.Trampoline;
			if (Electric) mat.color = Colors.Electric;
			if (Wall) mat.color = Colors.Wall;
			if (GrabbableLedge) mat.color = Colors.GrabbableLedge;
			if (FlagUnk2) mat.color = Colors.FlagUnknown;*/
			return mat;
		}
	}
}
