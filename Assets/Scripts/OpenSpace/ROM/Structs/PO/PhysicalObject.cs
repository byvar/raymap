using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class PhysicalObject : ROMStruct {
		public Reference<GeometricObject> visual;
		public Reference<PhysicalCollSet> collide;

		protected override void ReadInternal(Reader reader) {
			visual = new Reference<GeometricObject>(reader, true);
			collide = new Reference<PhysicalCollSet>(reader, true);
		}

		public PhysicalObjectComponent GetGameObject(GameObject gao = null) {
			if (gao == null) {
				gao = new GameObject("PO @ " + Offset);
			} else {
				gao.name += " - PO @ " + Offset;
				if (FileSystem.mode == FileSystem.Mode.Web) {
					gao.name = "PO @ " + Offset;
				}
			}
			PhysicalObjectComponent poc = gao.AddComponent<PhysicalObjectComponent>();
			if (visual.Value != null) {
				GameObject child = visual.Value.GetGameObject(GeometricObject.Type.Visual);
				child.transform.SetParent(gao.transform);
				child.name = "[Visual] " + child.name;
				poc.visual = child;
			}
			if (collide.Value != null && collide.Value.mesh.Value != null) {
				GameObject child = collide.Value.mesh.Value.GetGameObject(GeometricObject.Type.Collide);
				child.transform.SetParent(gao.transform);
				child.name = "[Collide] " + child.name;
				poc.collide = child;
				poc.collide.SetActive(false);
			}
			poc.Init(MapLoader.Loader.controller);
			return poc;
		}
	}
}
