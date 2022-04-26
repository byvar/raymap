using UnityEngine;
using Raymap;
using static BinarySerializer.Ubisoft.CPA.PS1.GLI_VisualMaterial;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class GMT_GameMaterialExtensions {
		public static Material CreateMaterial(this GMT_GameMaterial gmt) {
			if (gmt.CollisionMaterial?.Value != null) return gmt.CollisionMaterial.Value.CreateMaterial();
			Material mat = new Material(((Unity_Environment_CPA)gmt.Context.GetUnityEnvironment()).MaterialCollision);
			return mat;
		}
	}
}
