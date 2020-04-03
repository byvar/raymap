using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.ROM {
	public class GeometricObjectElementCollideSpheres : ROMStruct, IGeometricObjectElementCollide {
		public Reference<GeometricObjectElementCollideSphereArray> spheres;
		public ushort num_spheres;

		protected override void ReadInternal(Reader reader) {
			spheres = new Reference<GeometricObjectElementCollideSphereArray>(reader);
			num_spheres = reader.ReadUInt16();
			spheres.Resolve(reader, a => a.length = num_spheres);
		}

		public GameObject GetGameObject(GeometricObject.Type type, GeometricObject go, CollideType collideType = CollideType.None) {
			GameObject gao = null;
			if (type == GeometricObject.Type.Collide) {
				gao = new GameObject("Element Spheres @ " + Offset);
				gao.layer = LayerMask.NameToLayer("Collide");
				gao.transform.localPosition = Vector3.zero;
				if (spheres.Value != null) {
					Vector3[] verts = go.verticesCollide.Value?.GetVectors(go.ScaleFactor);
					GeometricObjectElementCollideSphereArray.GeometricElementCollideSphere[] sphere = spheres.Value.spheres;
					for (int i = 0; i < num_spheres; i++) {
						GameObject sphere_gao = GameObject.CreatePrimitive(PrimitiveType.Sphere);
						sphere_gao.name = "Sphere " + i;
						sphere_gao.transform.SetParent(gao.transform);
						MeshRenderer mr = sphere_gao.GetComponent<MeshRenderer>();
						sphere_gao.transform.localPosition = verts[sphere[i].centerPoint];
						sphere_gao.transform.localScale = Vector3.one * sphere[i].radius * 2; // default Unity sphere radius is 0.5
						sphere_gao.layer = LayerMask.NameToLayer("Collide");

						BillboardBehaviour bill = sphere_gao.AddComponent<BillboardBehaviour>();
						bill.mode = BillboardBehaviour.LookAtMode.CameraPosXYZ;

						CollideComponent cc = sphere_gao.AddComponent<CollideComponent>();
						cc.collideROM = this;
						cc.type = collideType;
						cc.index = i;

						mr.material = MapLoader.Loader.collideMaterial;
						GameMaterial gmt = spheres.Value.spheres[i].material.Value;
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
			return spheres.Value?.spheres[index.Value].material;
		}
	}
}
