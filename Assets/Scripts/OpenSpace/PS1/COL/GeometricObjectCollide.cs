using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class GeometricObjectCollide : OpenSpaceStruct, IGeometricObjectElementCollide {
		public ushort num_vertices;
		public ushort num_normals;
		public ushort num_triangles;
		public ushort num_quads;
		public byte[] unknownBytes;
		public LegacyPointer off_vertices;
		public LegacyPointer off_normals;
		public LegacyPointer off_triangles;
		public LegacyPointer off_quads;
		public uint uint_38;

		// Parsed
		public GeometricObjectCollideVector[] vertices;
		public GeometricObjectCollideVector[] normals;
		public GeometricObjectCollideTriangle[] triangles;
		public GeometricObjectCollideQuad[] quads;

		protected override void ReadInternal(Reader reader) {
			//Load.print(Offset);
			num_vertices = reader.ReadUInt16();
			num_normals = reader.ReadUInt16();
			num_triangles = reader.ReadUInt16();
			num_quads = reader.ReadUInt16();
			unknownBytes = reader.ReadBytes(0x20);
			off_vertices = LegacyPointer.Read(reader);
			off_normals = LegacyPointer.Read(reader);
			off_triangles = LegacyPointer.Read(reader);
			off_quads = LegacyPointer.Read(reader);
			uint_38 = reader.ReadUInt32();
			if (uint_38 != 0) Debug.LogWarning("Uint_38 wasn't 0 at " + Offset + ": " + uint_38);

			vertices = Load.ReadArray<GeometricObjectCollideVector>(num_vertices, reader, off_vertices);
			normals = Load.ReadArray<GeometricObjectCollideVector>(num_normals, reader, off_normals);
			triangles = Load.ReadArray<GeometricObjectCollideTriangle>(num_triangles, reader, off_triangles);
			quads = Load.ReadArray<GeometricObjectCollideQuad>(num_quads, reader, off_quads);
		}


		public GameObject GetGameObject() {
			GameObject parentGao = new GameObject(Offset.ToString());

			// First pass

			Dictionary<byte, List<IPS1PolygonCollide>> elementsDict = new Dictionary<byte, List<IPS1PolygonCollide>>();
			foreach (GeometricObjectCollideTriangle t in triangles) {
				byte gmi = t.MaterialIndex;
				if (!elementsDict.ContainsKey(gmi)) elementsDict[gmi] = new List<IPS1PolygonCollide>();
				elementsDict[gmi].Add(t);
			}
			foreach (GeometricObjectCollideQuad q in quads) {
				byte gmi = q.MaterialIndex;
				if (!elementsDict.ContainsKey(gmi)) elementsDict[gmi] = new List<IPS1PolygonCollide>();
				elementsDict[gmi].Add(q);
			}

			// Second pass
			byte[] elements = elementsDict.Keys.ToArray();
			for (int i = 0; i < elements.Length; i++) {
				byte gmi = elements[i];
				GameMaterial gm = (Load as R2PS1Loader).levelHeader.gameMaterials?[gmi];
				CollideMaterial cm = gm?.collideMaterial;

				IPS1PolygonCollide pf = elementsDict[gmi].FirstOrDefault();
				GameObject gao = new GameObject(Offset.ToString());
				if (cm != null) {
					/*+ " - " + i
					+ " - " + pf?.Offset*/
					gao.name += " - " + string.Format("{0:X2}", cm.type)
					+ "|" + string.Format("{0:X2}", cm.identifier);
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
				foreach (IPS1PolygonCollide p in elementsDict[gmi]) {
					int currentCount = vertIndices.Count;
					switch (p) {
						case GeometricObjectCollideTriangle t:
							vertIndices.Add(t.v0);
							vertIndices.Add(t.v1);
							vertIndices.Add(t.v2);

							normalIndices.Add(t.normal);
							normalIndices.Add(t.normal);
							normalIndices.Add(t.normal);

							triIndices.Add(currentCount);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 2);
							break;
						case GeometricObjectCollideQuad q:
							vertIndices.Add(q.v0);
							vertIndices.Add(q.v1);
							vertIndices.Add(q.v2);
							vertIndices.Add(q.v3);

							normalIndices.Add(q.normal);
							normalIndices.Add(q.normal);
							normalIndices.Add(q.normal);
							normalIndices.Add(q.normal);

							triIndices.Add(currentCount);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 2);

							triIndices.Add(currentCount + 2);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 3);
							break;
					}
				}
				GeometricObjectCollideVector[] v = vertIndices.Select(vi => vertices[vi]).ToArray();
				GeometricObjectCollideVector[] n = normalIndices.Select(ni => normals[ni]).ToArray();
				Mesh m = new Mesh();
				m.vertices = v.Select(s => new Vector3(s.x, s.z, s.y) / R2PS1Loader.CoordinateFactor).ToArray();
				//m.vertices = verts2.ToArray();
				m.normals = n.Select(s => new Vector3(
					s.x / (short.MaxValue / 8f),
					s.z / (short.MaxValue / 8f),
					s.y / (short.MaxValue / 8f))).ToArray();
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

				mr.material = gm.CreateMaterial();

				try {
					MeshCollider mc = gao.AddComponent<MeshCollider>();
					//mc.cookingOptions = MeshColliderCookingOptions.None;
					//mc.sharedMesh = mf.sharedMesh;
				} catch (Exception) { }

				CollideComponent cc = gao.AddComponent<CollideComponent>();
				cc.collidePS1 = this;
				cc.index = gmi; // Abuse the index for the game material index
			}
			return parentGao;
		}

		public GameMaterial GetMaterial(int? index) {
			if (!index.HasValue) {
				return null;
			} else {
				GameMaterial gm = (Load as R2PS1Loader).levelHeader.gameMaterials?[index.Value];
				return gm;
			}
		}
	}
}
