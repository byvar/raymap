using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class WayPoint : OpenSpaceStruct {
		public int x;
		public int y;
		public int z;
		public short short_0C;
		public short short_0E;
		public short radius;
		public short short_12;

		protected override void ReadInternal(Reader reader) {
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
			short_0C = reader.ReadInt16();
			short_0E = reader.ReadInt16();
			radius = reader.ReadInt16();
			short_12 = reader.ReadInt16();
		}



		public WayPointBehaviour GetGameObject() {
			GameObject gao = new GameObject("WayPoint (" + Offset + ")");
			//if (perso.Value != null) gao.name += " - Perso: " + perso.Value.Offset;
			float factor = 100f;
			Vector3 pos = Position;
			float radius_conv = radius / factor;

			gao.transform.position = pos;
			WayPointBehaviour wpBehaviour = gao.AddComponent<WayPointBehaviour>();
			wpBehaviour.wpPS1 = this;
			if (radius_conv > 1) {
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.localScale = new Vector3(radius_conv * 2, radius_conv * 2, radius_conv * 2);
				sphere.transform.parent = gao.transform;
				sphere.transform.localPosition = Vector3.zero;
				// No collider necessary
				GameObject.Destroy(sphere.GetComponent<SphereCollider>());
				MeshRenderer sphereRenderer = sphere.GetComponent<MeshRenderer>();
				sphereRenderer.material = new Material(MapLoader.Loader.collideTransparentMaterial);
				sphereRenderer.material.color = new Color(0.7f, 0f, 0.7f, 0.5f);
			}
			return wpBehaviour;
		}
		public Vector3 Position {
			get {
				float factor = 256f;
				Vector3 pos = new Vector3(x / factor, z / factor, y / factor);
				return pos;
			}
		}
	}
}
