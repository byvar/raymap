using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class WayPoint : ROMStruct {
		// Size: 10
		public short radius;
		public short x;
		public short y;
		public short z;
		public Reference<Perso> perso; // creepy, what is a perso reference doing here

		protected override void ReadInternal(Reader reader) {
			radius = reader.ReadInt16();
			x = reader.ReadInt16();
			y = reader.ReadInt16();
			z = reader.ReadInt16();
			perso = new Reference<Perso>(reader, true);
			Loader.waypointsROM.Add(this);
		}

		public WaypointBehaviour GetGameObject() {
			GameObject gao = new GameObject("WayPoint (" + Offset + ")");
			if (perso.Value != null) gao.name += " - Perso: " + perso.Value.Offset;
			float factor = 32f;
			Vector3 pos = Position;
			float radius_conv = radius / factor;

			gao.transform.position = pos;
			WaypointBehaviour wpBehaviour = gao.AddComponent<WaypointBehaviour>();
			wpBehaviour.wpROM = this;
			if (radius_conv > 1) {
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.localScale = new Vector3(radius_conv * 2, radius_conv * 2, radius_conv * 2);
				sphere.transform.parent = gao.transform;
				sphere.transform.localPosition = Vector3.zero;
				// No collider necessary
				GameObject.Destroy(sphere.GetComponent<SphereCollider>());
				MeshRenderer sphereRenderer = sphere.GetComponent<MeshRenderer>();
				sphereRenderer.material = MapLoader.Loader.collideTransparentMaterial;
				sphereRenderer.material.color = new Color(0.7f, 0f, 0.7f, 0.5f);
			}
			return wpBehaviour;
		}
		public Vector3 Position {
			get {
				float factor = 32f;
				Vector3 pos = new Vector3(x / factor, z / factor, y / factor);
				return pos;
			}
		}
	}
}
