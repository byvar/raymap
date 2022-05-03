using Raymap;
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Unity;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class COL_GeometricObjectCollideExtensions {
		public static GameObject GetGameObject(this COL_GeometricObjectCollide geo) {
			GameObject parentGao = new GameObject(geo.Offset.ToString());
			parentGao.AddBinarySerializableData(geo);

			// First pass
			Dictionary<byte, List<COL_GeometricObjectCollidePolygon>> elementsDict = new Dictionary<byte, List<COL_GeometricObjectCollidePolygon>>();
			void Process(COL_GeometricObjectCollidePolygon poly) {
				byte gmi = poly.GameMaterial;
				if (!elementsDict.ContainsKey(gmi)) elementsDict[gmi] = new List<COL_GeometricObjectCollidePolygon>();
				elementsDict[gmi].Add(poly);
			}
			GMT_GameMaterial GetMaterial(int index) => geo.Context.GetLevel().GlobalPointerTable.GameMaterials[index];

			if(geo.Triangles != null)
				foreach (COL_GeometricObjectCollidePolygon t in geo.Triangles) Process(t);
			if(geo.Quads != null)
				foreach (COL_GeometricObjectCollidePolygon q in geo.Quads) Process(q);

			// Second pass
			byte[] elements = elementsDict.Keys.ToArray();
			for (int i = 0; i < elements.Length; i++) {
				byte gmi = elements[i];
				GMT_GameMaterial gmt = GetMaterial(gmi);
				var cmt = gmt?.CollideMaterial?.Value;

				COL_GeometricObjectCollidePolygon pf = elementsDict[gmi].FirstOrDefault();
				GameObject gao = new GameObject(geo.Offset.ToString());
				gao.AddBinarySerializableData(geo);
				if (cmt != null) {
					/*+ " - " + i
					+ " - " + pf?.Offset*/
					gao.name += " - " + cmt.ZoneType
					+ "|" + string.Format("{0:X2}", cmt.Identifier);
				}
				gao.transform.SetParent(parentGao.transform);
				gao.transform.localPosition = Vector3.zero;
				gao.layer = LayerMask.NameToLayer("Collide");
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();

				List<int> vertIndices = new List<int>();
				List<int> triIndices = new List<int>();
				List<int> normalIndices = new List<int>();
				List<Vector3> normals2 = new List<Vector3>();
				foreach (COL_GeometricObjectCollidePolygon p in elementsDict[gmi]) {
					int currentCount = vertIndices.Count;
					vertIndices.Add(p.V0);
					vertIndices.Add(p.V1);
					vertIndices.Add(p.V2);

					normalIndices.Add(p.Normal);
					normalIndices.Add(p.Normal);
					normalIndices.Add(p.Normal);

					// First triangle
					triIndices.Add(currentCount);
					triIndices.Add(currentCount + 1);
					triIndices.Add(currentCount + 2);

					if (p.Pre_IsQuad) {
						vertIndices.Add(p.V3);
						normalIndices.Add(p.Normal);

						// Second triangle
						triIndices.Add(currentCount + 2);
						triIndices.Add(currentCount + 1);
						triIndices.Add(currentCount + 3);
					}
				}
				COL_GeometricObjectCollideVector[] v = vertIndices.Select(vi => geo.Vertices[vi]).ToArray();
				COL_GeometricObjectCollideVector[] n = normalIndices.Select(ni => geo.Normals[ni]).ToArray();
				Mesh m = new Mesh();
				m.vertices = v.Select(s => s.Vector.GetUnityVector(convertAxes: true)).ToArray();
				//m.vertices = verts2.ToArray();
				m.normals = n.Select(s => s.Vector.GetUnityVector(convertAxes: true)).ToArray();
				/*for(int j = 0; j < m.normals.Length; j++) {
					GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					g.transform.parent = gao.transform;
					g.transform.localPosition = m.vertices[j] + m.normals[j];
					g.transform.localScale = Vector3.one * 0.1f;
					Load.print(m.normals[j].magnitude);
				}*/
				//m.SetUVs(0, uvs.Select(s => new Vector4(s.x, s.y, alpha, 0f)).ToList());
				m.triangles = triIndices.ToArray();

				Vector2[] uvs = new Vector2[m.vertexCount];

				// Generate simple UVs for collision checkerboard (basically a box projection)
				for (int j = 0; j < m.vertexCount; j++) {
					Vector3 normal = m.normals[j];
					normal = new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z));
					float biggestNorm = Mathf.Max(normal.x, normal.y, normal.z);

					float uvX = (m.vertices[j].x / 20.0f);
					float uvY = (m.vertices[j].y / 20.0f);
					float uvZ = (m.vertices[j].z / 20.0f);

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
				m.uv = uvs;

				mf.mesh = m;

				mr.material = gmt.CreateMaterial();

				try {
					MeshCollider mc = gao.AddComponent<MeshCollider>();
					//mc.cookingOptions = MeshColliderCookingOptions.None;
					//mc.sharedMesh = mf.sharedMesh;
				} catch (Exception) { }
			}
			return parentGao;
		}
	}
}
