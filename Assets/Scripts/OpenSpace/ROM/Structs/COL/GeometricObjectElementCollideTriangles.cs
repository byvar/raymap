using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.ROM {
	public class GeometricObjectElementCollideTriangles : ROMStruct, IGeometricObjectElementCollide {
		public GenericReference material;
		public ushort ind_material;
		public Reference<GeometricObjectElementCollideTrianglesData> triangles;
		public ushort ind_37;
		public ushort num_triangles;
		public ushort type_material;
		public ushort unk;

		protected override void ReadInternal(Reader reader) {
			ind_material = reader.ReadUInt16();
			triangles = new Reference<GeometricObjectElementCollideTrianglesData>(reader);
			ind_37 = reader.ReadUInt16();
			num_triangles = reader.ReadUInt16();
			unk = reader.ReadUInt16();
			type_material = reader.ReadUInt16();
			
			triangles.Resolve(reader, t => t.length = num_triangles);
			material = new GenericReference(type_material, ind_material, reader, true);
		}

		public GameObject GetGameObject(GeometricObject.Type type, GeometricObject go, CollideType collideType = CollideType.None) {
			GameObject gao = null;
			if (type == GeometricObject.Type.Collide) {
				gao = new GameObject("Element @ " + Offset);
				gao.layer = LayerMask.NameToLayer("Collide");
				gao.transform.localPosition = Vector3.zero;
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				mr.material = MapLoader.Loader.collideMaterial;
				if (material.Value != null && material.Value is GameMaterial) {
					GameMaterial gmt = material.Value as GameMaterial;
					//MapLoader.Loader.print(gmt.collideMaterial);
					if (gmt.collideMaterial.Value != null) {
						gmt.collideMaterial.Value.SetMaterial(mr);
					}
				} else {
					MapLoader.Loader.print("Type: " + type_material + " - Ind: " + ind_material);
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
				Mesh mesh = new Mesh();
				mesh.vertices = go.verticesCollide.Value.GetVectors(go.ScaleFactor);
				//mesh.normals = go.normals.Value.GetVectors(Int16.MaxValue);
				//mesh.SetUVs(0, triangles.Value.uvs.Select(u => new Vector3(u.x, u.y, 1f)).ToList());
				mesh.triangles = triangles.Value.triangles.SelectMany(t => new int[] { t.v2, t.v1, t.v3 }).ToArray();
				mesh.RecalculateNormals();

                Vector2[] uvs = new Vector2[mesh.vertexCount];
                
                // Generate simple UVs for collision checkerboard (basically a box projection)
                for (int j = 0; j < mesh.vertexCount; j++) {
                    Vector3 normal = mesh.normals[j];
                    float biggestNorm = Mathf.Max(Mathf.Max(Mathf.Abs(normal.x), Mathf.Abs(normal.y)), Mathf.Abs(normal.z));

                    float uvX = (mesh.vertices[j].x / 20.0f);
                    float uvY = (mesh.vertices[j].y / 20.0f);
                    float uvZ = (mesh.vertices[j].z / 20.0f);

                    //Debug.Log("Norms: " + normal.x+","+normal.y+","+normal.z);
                    //Debug.Log("Biggest norm: " + biggestNorm);
                    if (biggestNorm == Mathf.Abs(normal.x)) {
                        uvs[j] = new Vector2(uvY, uvZ);
                    } else if (biggestNorm == Mathf.Abs(normal.y)) {
                        uvs[j] = new Vector2(uvX, uvZ);
                    } else if (biggestNorm == Mathf.Abs(normal.z)) {
                        uvs[j] = new Vector2(uvX, uvY);
                    } else {
                        Debug.LogError("HALP");
                    }
                }
                mesh.uv = uvs;

                mf.mesh = mesh;

				try {
					MeshCollider mc = gao.AddComponent<MeshCollider>();
					//mc.cookingOptions = MeshColliderCookingOptions.None;
					//mc.sharedMesh = mf.sharedMesh;
				} catch (Exception) { }

				CollideComponent cc = gao.AddComponent<CollideComponent>();
				cc.collideROM = this;
				cc.type = collideType;
			}
			return gao;
		}

		public GameMaterial GetMaterial(int? index) {
			if (material.Value != null && material.Value is GameMaterial) {
				return material.Value as GameMaterial;
			}
			return null;
		}
	}
}
