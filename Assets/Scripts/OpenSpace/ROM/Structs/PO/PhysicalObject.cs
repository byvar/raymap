using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class PhysicalObject : ROMStruct {
		public Reference<GeometricObject> visual;
		public Reference<CollSet> collide;

		protected override void ReadInternal(Reader reader) {
			visual = new Reference<GeometricObject>(reader, true);
			collide = new Reference<CollSet>(reader, true);
		}

		private GameObject gao;
		public GameObject Gao {
			get {
				if (gao == null) {
					gao = new GameObject("PO @ " + Offset);
					InitGameObject();
				}
				return gao;
			}
		}

		protected void InitGameObject() {
			if (visual.Value != null) {
				GameObject child = visual.Value.Gao;
				child.transform.SetParent(Gao.transform);
				child.name = "[Visual] " + child.name;
			}
			if (collide.Value != null && collide.Value.mesh.Value != null) {
				GameObject child = collide.Value.mesh.Value.Gao;
				child.transform.SetParent(Gao.transform);
				child.name = "[Collide] " + child.name;
			}
		}
	}
}
