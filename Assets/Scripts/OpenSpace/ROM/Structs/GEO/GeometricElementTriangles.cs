using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricElementTriangles : ROMStruct {
		public Reference<VisualMaterial> visualMaterial;
		public Reference<GeometricElementTrianglesData> triangles;
		public Reference<VertexArray> vertices;
		public ushort sz_triangles;
		public ushort sz_triangles_compressed;
		public ushort sz_vertices;
		public ushort flags;
		public ushort unk0;

		protected override void ReadInternal(Reader reader) {
			visualMaterial = new Reference<VisualMaterial>(reader, true);
			if (Settings.s.platform == Settings.Platform.N64) {
				//MapLoader.Loader.print("Triangles: " + Pointer.Current(reader));
				triangles = new Reference<GeometricElementTrianglesData>(reader);
				vertices = new Reference<VertexArray>(reader);
				sz_triangles = reader.ReadUInt16();
				sz_vertices = reader.ReadUInt16();
				flags = reader.ReadUInt16();

				vertices.Resolve(reader, v => v.length = sz_vertices);
			} else if (Settings.s.platform == Settings.Platform.DS) {
				if (Settings.s.game == Settings.Game.RRR) {
					unk0 = reader.ReadUInt16();
					sz_triangles = reader.ReadUInt16();
					triangles = new Reference<GeometricElementTrianglesData>(reader);
					sz_triangles_compressed = reader.ReadUInt16();
				} else {
					triangles = new Reference<GeometricElementTrianglesData>(reader);
					sz_triangles = reader.ReadUInt16();
				}
			} else if (Settings.s.platform == Settings.Platform._3DS) {
				triangles = new Reference<GeometricElementTrianglesData>(reader);
				sz_triangles = reader.ReadUInt16();
				sz_vertices = reader.ReadUInt16();
			}
			triangles.Resolve(reader, (t) => { t.length = sz_triangles; t.num_vertices = sz_vertices; t.compressedLength = sz_triangles_compressed; });

		}

		public GameObject GetGameObject(GeometricObject.Type type, GeometricObject go) {
			if (type == GeometricObject.Type.Visual) {
				GameObject gao = new GameObject("ElementTriangles @ " + Offset);
				gao.layer = LayerMask.NameToLayer("Visual");
				bool backfaceCulling = !visualMaterial.Value.RenderBackFaces;
				gao.transform.localPosition = Vector3.zero;
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				mr.material = visualMaterial.Value.GetMaterial(VisualMaterial.Hint.None);
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				Mesh mesh = new Mesh();
				if (Settings.s.platform == Settings.Platform._3DS) {
					if (sz_vertices == 0) {
						mesh.vertices = go.verticesVisual.Value.GetVectors(go.ScaleFactor);
						mesh.normals = go.normals.Value.GetVectors(Int16.MaxValue);
					} else {
						// Use vertices located in element
						mesh.vertices = triangles.Value.verts.Select(v => v.GetVector(go.ScaleFactor)).ToArray();
						mesh.normals = triangles.Value.normals.Select(v => v.GetVector(Int16.MaxValue)).ToArray();
					}
					mesh.SetUVs(0, triangles.Value.uvs.Select(u => new Vector3(u.x, u.y, 1f)).ToList());
					mesh.triangles = triangles.Value.triangles.SelectMany(t => backfaceCulling ? new int[] { t.v2, t.v1, t.v3 } : new int[] { t.v2, t.v1, t.v3, t.v1, t.v2, t.v3 }).ToArray();
				} else if (Settings.s.platform == Settings.Platform.N64) {
					mesh = RSP.RSPParser.Parse(triangles.Value.rspCommands, vertices.Value.vertices, go, backfaceCulling, mr.material);
					//gao.name += " - Verts ( " + sz_vertices + "):" + vertices.Value.Offset + " - Tris ( " + sz_triangles + " ):" + triangles.Value.Offset + " - " + Index + " - " + flags;
					//gao.name += " - Flags: " + string.Format("{0:X4}", visualMaterial.Value.textures.Value.vmTex[0].texRef.Value.texInfo.Value.flags);
				} else if (Settings.s.platform == Settings.Platform.DS) {
					if (triangles.Value != null) {
						mesh = DS3D.GeometryParser.Parse(triangles.Value.ds3dCommands, go, backfaceCulling, mr.material);
						//gao.name += " - Tris ( " + sz_triangles + " ):" + triangles.Value.Offset + " - " + Index + " - " + flags;
					}
				}
				mf.mesh = mesh;
				if (Settings.s.platform == Settings.Platform.N64 || Settings.s.platform == Settings.Platform.DS) {
					// Apply vertex colors
					mr.sharedMaterial.SetVector("_Tex2Params", new Vector4(60, 0, 0, 0));
				}
				return gao;
			} else {
				return null;
			}
		}

		public void MorphVertices(Mesh mesh, GeometricElementTriangles tris2, GeometricObject go1, GeometricObject go2, float lerp) {
			CompressedVector3Array v1, v2;
			if (Settings.s.platform == Settings.Platform._3DS) {
				v1 = go1.verticesVisual.Value;
				v2 = go2.verticesVisual.Value;
			}
			GeometricElementTriangles tris1 = this;
			if (Settings.s.platform == Settings.Platform.N64) {
				if (tris1.sz_vertices != tris2.sz_vertices) return;
				Vector3[] verts1 = RSP.RSPParser.ParseVerticesOnly(tris1.triangles.Value.rspCommands, tris1.vertices.Value.vertices, go1);
				Vector3[] verts2 = RSP.RSPParser.ParseVerticesOnly(tris2.triangles.Value.rspCommands, tris2.vertices.Value.vertices, go2);
				for (int i = 0; i < verts1.Length; i++) {
					verts1[i] = Vector3.Lerp(verts1[i], verts2[i], lerp);
				}
				mesh.vertices = verts1;
			} else if (Settings.s.platform == Settings.Platform._3DS) {
				Vector3[] verts1, verts2;
				if (tris1.sz_vertices == 0) {
					verts1 = go1.verticesVisual.Value.GetVectors(go1.ScaleFactor);
				} else {
					// Use vertices located in element
					verts1 = tris1.triangles.Value.verts.Select(v => v.GetVector(go1.ScaleFactor)).ToArray();
				}
				if (tris2.sz_vertices == 0) {
					verts2 = go2.verticesVisual.Value.GetVectors(go2.ScaleFactor);
				} else {
					// Use vertices located in element
					verts2 = tris2.triangles.Value.verts.Select(v => v.GetVector(go2.ScaleFactor)).ToArray();
				}
				if (verts1.Length != verts2.Length) return;
				for (int i = 0; i < verts1.Length; i++) {
					verts1[i] = Vector3.Lerp(verts1[i], verts2[i], lerp);
				}
				mesh.vertices = verts1;
			} else if (Settings.s.platform == Settings.Platform.DS) {
				Vector3[] verts1 = DS3D.GeometryParser.ParseVerticesOnly(tris1.triangles.Value.ds3dCommands, go1);
				Vector3[] verts2 = DS3D.GeometryParser.ParseVerticesOnly(tris2.triangles.Value.ds3dCommands, go2);
				for (int i = 0; i < verts1.Length; i++) {
					verts1[i] = Vector3.Lerp(verts1[i], verts2[i], lerp);
				}
				mesh.vertices = verts1;
			}
		}

		public void ResetMorph(Mesh mesh, GeometricObject go) {
			if (Settings.s.platform == Settings.Platform.N64) {
				Vector3[] verts = RSP.RSPParser.ParseVerticesOnly(triangles.Value.rspCommands, vertices.Value.vertices, go);
				mesh.vertices = verts;
			} else if (Settings.s.platform == Settings.Platform._3DS) {
				Vector3[] verts;
				if (sz_vertices == 0) {
					verts = go.verticesVisual.Value.GetVectors(go.ScaleFactor);
				} else {
					// Use vertices located in element
					verts = triangles.Value.verts.Select(v => v.GetVector(go.ScaleFactor)).ToArray();
				}
				mesh.vertices = verts;
			} else if (Settings.s.platform == Settings.Platform.DS) {
				Vector3[] verts = DS3D.GeometryParser.ParseVerticesOnly(triangles.Value.ds3dCommands, go);
				mesh.vertices = verts;
			}
		}
	}
}
