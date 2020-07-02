using OpenSpace.Loader;
using System.Collections.Generic;
using UnityEngine;
using CollideType = OpenSpace.PS1.CollSet.CollideType;

namespace OpenSpace.PS1 {
	public class ZdxBox : OpenSpaceStruct {
		public short x0;
		public short y0;
		public short z0;
		public short padding0;
		public short x1;
		public short y1;
		public short z1;
		public short padding1;
		public uint gameMaterial;

		protected override void ReadInternal(Reader reader) {
			x0 = reader.ReadInt16();
			y0 = reader.ReadInt16();
			z0 = reader.ReadInt16();
			padding0 = reader.ReadInt16();
			x1 = reader.ReadInt16();
			y1 = reader.ReadInt16();
			z1 = reader.ReadInt16();
			padding1 = reader.ReadInt16();
			gameMaterial = reader.ReadUInt32();
		}

		public GameObject GetGameObject(GameObject parent, int index, CollideType collideType = CollideType.None) {
			GameObject gao = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gao.name = "Box " + index + " - " + Offset;
			gao.transform.SetParent(parent.transform);
			MeshRenderer mr = gao.GetComponent<MeshRenderer>();
			Vector3 minVertex = new Vector3(x0 / 256f, z0 / 256f, y0 / 256f);
			Vector3 maxVertex = new Vector3(x1 / 256f, z1 / 256f, y1 / 256f);
			Vector3 center = Vector3.Lerp(minVertex, maxVertex, 0.5f);
			gao.transform.localPosition = center;
			gao.transform.localScale = maxVertex - minVertex;
			gao.layer = LayerMask.NameToLayer("Collide");

			/*CollideComponent cc = sphere_gao.AddComponent<CollideComponent>();
			cc.collideROM = this;
			cc.type = collideType;
			cc.index = i;*/

			GameMaterial gm = (Load as R2PS1Loader).levelHeader.gameMaterials?[gameMaterial];
			CollideMaterial cm = gm?.collideMaterial;
			if (cm != null) {
				mr.material = cm.CreateMaterial();
			} else {
				mr.material = new Material(MapLoader.Loader.collideMaterial);
			}
			if (collideType != CollideType.None) {
				Color col = mr.material.color;
				mr.material = new Material(MapLoader.Loader.collideTransparentMaterial);
				mr.material.color = new Color(col.r, col.g, col.b, col.a * 0.7f);
				switch (collideType) {
					case CollideType.ZDD:
						mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdd")); break;
					case CollideType.ZDM:
						mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdm")); break;
					case CollideType.ZDE:
						mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zde")); break;
					case CollideType.ZDR:
						mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdr")); break;
				}
			}
			return gao;
		}
	}
}
