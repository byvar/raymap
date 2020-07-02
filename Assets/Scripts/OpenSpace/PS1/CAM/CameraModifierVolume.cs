using OpenSpace.Loader;
using System.Collections.Generic;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class CameraModifierVolume : OpenSpaceStruct {
		public uint int_00;
		public uint flags;
		public int x0;
		public int y0;
		public int z0;
		public int int_14;
		public int radius;
		public int x1;
		public int y1;
		public int z1;
		public int int_28;
		public int int_2C;
		public int int_30;
		public int int_34;

		protected override void ReadInternal(Reader reader) {
			int_00 = reader.ReadUInt32();
			flags = reader.ReadUInt32();
			x0 = reader.ReadInt32();
			y0 = reader.ReadInt32();
			z0 = reader.ReadInt32();
			int_14 = reader.ReadInt32();
			radius = reader.ReadInt32();
			x1 = reader.ReadInt32();
			y1 = reader.ReadInt32();
			z1 = reader.ReadInt32();
			int_28 = reader.ReadInt32();
			int_2C = reader.ReadInt32();
			int_30 = reader.ReadInt32();
			int_34 = reader.ReadInt32();
		}


		public GameObject GetGameObject(GameObject parent) {
			GameObject gao = null;
			if ((flags & 0x100) == 0x100) {
				gao = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gao.transform.SetParent(parent.transform);
				Vector3 center = new Vector3(x0 / 256f, z0 / 256f, y0 / 256f);
				Vector3 scale = new Vector3(x1 / 256f, z1 / 256f, y1 / 256f);
				gao.transform.position = center;
				gao.transform.localScale = scale * 2f;
			} else {
				gao = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gao.transform.SetParent(parent.transform);
				Vector3 center = new Vector3(x0 / 256f, z0 / 256f, y0 / 256f);
				gao.transform.localPosition = center;
				gao.transform.localScale = Vector3.one * (radius / 256f) * 2; // default Unity sphere radius is 0.5
			}
			gao.name = "CameraModifierVolume - " + Offset;
			MeshRenderer mr = gao.GetComponent<MeshRenderer>();
			gao.layer = LayerMask.NameToLayer("Collide");

			/*CollideComponent cc = sphere_gao.AddComponent<CollideComponent>();
			cc.collideROM = this;
			cc.type = collideType;
			cc.index = i;*/

			mr.material = new Material(MapLoader.Loader.collideTransparentMaterial);
			mr.material.color = new Color(1f,1f,1f,1f * 0.7f);
			return gao;
		}
	}
}
