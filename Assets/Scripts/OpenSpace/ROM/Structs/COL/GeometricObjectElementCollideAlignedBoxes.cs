using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.ROM {
	public class GeometricObjectElementCollideAlignedBoxes : ROMStruct, IGeometricObjectElementCollide {
		public Reference<GeometricObjectElementCollideAlignedBoxArray> boxes;
		public ushort num_boxes;

		protected override void ReadInternal(Reader reader) {
			boxes = new Reference<GeometricObjectElementCollideAlignedBoxArray>(reader);
			num_boxes = reader.ReadUInt16();
			boxes.Resolve(reader, a => a.length = num_boxes);
		}

		public GameObject GetGameObject(GeometricObject.Type type, GeometricObject go, CollideType collideType = CollideType.None) {
			GameObject gao = null;
			if (type == GeometricObject.Type.Collide) {
				gao = new GameObject("Element Boxes @ " + Offset);
				gao.layer = LayerMask.NameToLayer("Collide");
				gao.transform.localPosition = Vector3.zero;
				if (boxes.Value != null) {
					Vector3[] verts = go.verticesCollide.Value?.GetVectors(go.ScaleFactor);
					GeometricObjectElementCollideAlignedBoxArray.GeometricElementCollideAlignedBox[] b = boxes.Value.boxes;
					for (int i = 0; i < num_boxes; i++) {
						GameObject box_gao = GameObject.CreatePrimitive(PrimitiveType.Cube);
						box_gao.layer = LayerMask.NameToLayer("Collide");
						box_gao.name = "Box " + i;
						box_gao.transform.SetParent(gao.transform);
						MeshFilter mf = box_gao.GetComponent<MeshFilter>();
						MeshRenderer mr = box_gao.GetComponent<MeshRenderer>();
						Vector3 center = Vector3.Lerp(verts[b[i].minVertex], verts[b[i].maxVertex], 0.5f);
						box_gao.transform.localPosition = center;
						box_gao.transform.localScale = verts[b[i].maxVertex] - verts[b[i].minVertex];

						CollideComponent cc = box_gao.AddComponent<CollideComponent>();
						cc.collideROM = this;
						cc.type = collideType;
						cc.index = i;

						mr.material = MapLoader.Loader.collideMaterial;
						GameMaterial gmt = boxes.Value.boxes[i].material.Value;
						if (gmt != null && gmt.collideMaterial.Value != null) {
							gmt.collideMaterial.Value.SetMaterial(mr);
						}
						if (collideType != CollideType.None) {
							Color col = mr.material.color;
							mr.material = MapLoader.Loader.collideTransparentMaterial;
							mr.material.color = new Color(col.r, col.g, col.b, col.a * 0.7f);
							switch (collideType) {
								case CollideType.ZDD:
									mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdd")); break;
								case CollideType.ZDE:
									mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zde")); break;
								case CollideType.ZDM:
									mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdm")); break;
								case CollideType.ZDR:
									mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdr")); break;
							}
						}
					}
				}
			}
			return gao;
		}

		public GameMaterial GetMaterial(int? index) {
			if (!index.HasValue) return null;
			return boxes.Value?.boxes[index.Value].material;
		}
	}
}
