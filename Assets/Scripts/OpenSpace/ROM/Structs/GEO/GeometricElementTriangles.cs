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
				GameObject gao = new GameObject("Element @ " + Offset);
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
					gao.name += " - Verts ( " + sz_vertices + "):" + vertices.Value.Offset + " - Tris ( " + sz_triangles + " ):" + triangles.Value.Offset + " - " + Index + " - " + flags;
					//gao.name += " - Flags: " + string.Format("{0:X4}", visualMaterial.Value.textures.Value.vmTex[0].texRef.Value.texInfo.Value.flags);
				} else if (Settings.s.platform == Settings.Platform.DS) {
					if (triangles.Value != null) {
						mesh = DS3D.GeometryParser.Parse(triangles.Value.ds3dCommands, go, backfaceCulling, mr.material);
						gao.name += " - Tris ( " + sz_triangles + " ):" + triangles.Value.Offset + " - " + Index + " - " + flags;
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
	}
}
