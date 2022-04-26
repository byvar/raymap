using Raymap;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class CS_PhysicalObjectCollisionExtensions {
		public static GameObject GetGameObject(this CS_PhysicalObjectCollision cs) {
			GameObject parentGao = new GameObject(cs.Offset.ToString());
			var gao = cs.GeoCollide?.GetGameObject();
			gao.transform.SetParent(parentGao.transform, false);
			return parentGao;
		}
	}
}
