using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.ROM.RSP {
	public class RSPParser {
		public static Mesh Parse(RSPCommand[] commands, VertexArray.Vertex[] vertices, GeometricObject go, bool backfaceCulling, Material mat) {
			List<VertexArray.Vertex> verts = new List<VertexArray.Vertex>();
			Dictionary<int, int> vertexBufferMapping = new Dictionary<int, int>();
			List<GeometricObjectElementTrianglesData.Triangle> triangles = new List<GeometricObjectElementTrianglesData.Triangle>();
			Texture tex = mat.GetTexture("_Tex0");
			float wFactor = 64f, hFactor = 64f;
			if (tex != null) {
				wFactor = tex.width;
				hFactor = tex.height;
			}
			Mesh mesh = new Mesh();
			for (int i = 0; i < Math.Min(32, vertices.Length); i++) {
				int curVertsCount = i;
				verts.Add(vertices[i].WorkingCopy);
				vertexBufferMapping[i] = i;
			}
			bool parsing = true;
			foreach (RSPCommand c in commands) {
				//if (c.Command == RSPCommand.Type.RSP_GBI1_Vtx) break;
				switch (c.Command) {
					case RSPCommand.Type.RSP_GBI1_Vtx:
						if (c.vtx.address % 16 != 0) {
							Debug.LogError("Mom, RSP is doing weird memory saving tricks!");
						}
						uint startVtx = c.vtx.address / 16;
						if (c.vtx.segment != 6) {
							Debug.LogWarning(c.vtx.segment);
						}
						//MapLoader.Loader.print(c.vtx.n + " - " + c.vtx.length);
						//int numVertices = (c.vtx.length + 1) / 16;
						int numVertices = c.vtx.n;
						for (int i = 0; i < numVertices; i++) {
							int curVertsCount = verts.Count;
							verts.Add(vertices[startVtx + i].WorkingCopy);
							vertexBufferMapping[c.vtx.v0 + i] = curVertsCount;
						}
						break;
					case RSPCommand.Type.RSP_GBI1_ModifyVtx:
						ushort ind = c.modifyVtx.vertex;
						if (ind > 32) {
							Debug.LogError("Mom, my RSP implementation is wrong!");
						}
						VertexArray.Vertex og = verts[vertexBufferMapping[ind]].WorkingCopy;
						og.PrepareWorkingCopy();
						VertexArray.Vertex vtx = og.WorkingCopy;
						switch (c.modifyVtx.Command) {
							case RSPCommand.GBI1_ModifyVtx.Type.RSP_MV_WORD_OFFSET_POINT_RGBA:
								vtx.r = c.modifyVtx.r;
								vtx.g = c.modifyVtx.g;
								vtx.b = c.modifyVtx.b;
								vtx.a = c.modifyVtx.a;
								break;
							case RSPCommand.GBI1_ModifyVtx.Type.RSP_MV_WORD_OFFSET_POINT_ST:
								vtx.u = c.modifyVtx.u;
								vtx.v = c.modifyVtx.v;
								/*og.u = c.modifyVtx.u;
								og.v = c.modifyVtx.v;*/
								break;
							case RSPCommand.GBI1_ModifyVtx.Type.RSP_MV_WORD_OFFSET_POINT_XYSCREEN:
							case RSPCommand.GBI1_ModifyVtx.Type.RSP_MV_WORD_OFFSET_POINT_ZSCREEN:
								Debug.LogWarning(c.modifyVtx.Command);
								break;
						}
						vertexBufferMapping[ind] = verts.Count;
						verts.Add(vtx);
						break;
					case RSPCommand.Type.RSP_GBI1_Tri1:
						triangles.Add(new GeometricObjectElementTrianglesData.Triangle() {
							v1 = (ushort)vertexBufferMapping[c.tri1.v0],
							v2 = (ushort)vertexBufferMapping[c.tri1.v1],
							v3 = (ushort)vertexBufferMapping[c.tri1.v2],
						});
						break;
					case RSPCommand.Type.RSP_GBI1_Tri2:
						triangles.Add(new GeometricObjectElementTrianglesData.Triangle() {
							v1 = (ushort)vertexBufferMapping[c.tri2.v0],
							v2 = (ushort)vertexBufferMapping[c.tri2.v1],
							v3 = (ushort)vertexBufferMapping[c.tri2.v2],
						});
						triangles.Add(new GeometricObjectElementTrianglesData.Triangle() {
							v1 = (ushort)vertexBufferMapping[c.tri2.v3],
							v2 = (ushort)vertexBufferMapping[c.tri2.v4],
							v3 = (ushort)vertexBufferMapping[c.tri2.v5],
						});
						break;
					case RSPCommand.Type.RSP_GBI1_EndDL:
						parsing = false;
						break;
					default:
						Debug.LogError("Unparsed command: " + c.Command);
						break;
				}
				if (!parsing) break;
			}

			mesh.vertices = verts.Select(v => v.GetVector(go.ScaleFactor)).ToArray();
			//mesh.normals = verts.Select(v => v.GetVector(Int16.MaxValue)).ToArray();
			mesh.SetUVs(0, verts.Select(v => new Vector3(v.u / 32f / wFactor, v.v / 32f / hFactor, 1f)).ToList());
			mesh.SetColors(verts.Select(v => new Color(v.color.r, v.color.g, v.color.b, v.color.a)).ToList());
			mesh.triangles = triangles.SelectMany(t => backfaceCulling ? new int[] { t.v1, t.v2, t.v3 } : new int[] { t.v1, t.v2, t.v3, t.v2, t.v1, t.v3 }).ToArray();
			mesh.RecalculateNormals();

			return mesh;
		}

		public static Vector3[] ParseVerticesOnly(RSPCommand[] commands, VertexArray.Vertex[] vertices, GeometricObject go) {
			List<VertexArray.Vertex> verts = new List<VertexArray.Vertex>();
			Dictionary<int, int> vertexBufferMapping = new Dictionary<int, int>();
			List<GeometricObjectElementTrianglesData.Triangle> triangles = new List<GeometricObjectElementTrianglesData.Triangle>();
			Mesh mesh = new Mesh();
			for (int i = 0; i < Math.Min(32, vertices.Length); i++) {
				int curVertsCount = i;
				verts.Add(vertices[i].WorkingCopy);
				vertexBufferMapping[i] = i;
			}
			bool parsing = true;
			foreach (RSPCommand c in commands) {
				//if (c.Command == RSPCommand.Type.RSP_GBI1_Vtx) break;
				switch (c.Command) {
					case RSPCommand.Type.RSP_GBI1_Vtx:
						if (c.vtx.address % 16 != 0) {
							Debug.LogError("Mom, RSP is doing weird memory saving tricks!");
						}
						uint startVtx = c.vtx.address / 16;
						if (c.vtx.segment != 6) {
							Debug.LogWarning(c.vtx.segment);
						}
						//MapLoader.Loader.print(c.vtx.n + " - " + c.vtx.length);
						//int numVertices = (c.vtx.length + 1) / 16;
						int numVertices = c.vtx.n;
						for (int i = 0; i < numVertices; i++) {
							int curVertsCount = verts.Count;
							verts.Add(vertices[startVtx + i].WorkingCopy);
							vertexBufferMapping[c.vtx.v0 + i] = curVertsCount;
						}
						break;
					case RSPCommand.Type.RSP_GBI1_ModifyVtx:
						ushort ind = c.modifyVtx.vertex;
						if (ind > 32) {
							Debug.LogError("Mom, my RSP implementation is wrong!");
						}
						VertexArray.Vertex og = verts[vertexBufferMapping[ind]].WorkingCopy;
						og.PrepareWorkingCopy();
						VertexArray.Vertex vtx = og.WorkingCopy;
						switch (c.modifyVtx.Command) {
							case RSPCommand.GBI1_ModifyVtx.Type.RSP_MV_WORD_OFFSET_POINT_RGBA:
								vtx.r = c.modifyVtx.r;
								vtx.g = c.modifyVtx.g;
								vtx.b = c.modifyVtx.b;
								vtx.a = c.modifyVtx.a;
								break;
							case RSPCommand.GBI1_ModifyVtx.Type.RSP_MV_WORD_OFFSET_POINT_ST:
								vtx.u = c.modifyVtx.u;
								vtx.v = c.modifyVtx.v;
								/*og.u = c.modifyVtx.u;
								og.v = c.modifyVtx.v;*/
								break;
							case RSPCommand.GBI1_ModifyVtx.Type.RSP_MV_WORD_OFFSET_POINT_XYSCREEN:
							case RSPCommand.GBI1_ModifyVtx.Type.RSP_MV_WORD_OFFSET_POINT_ZSCREEN:
								Debug.LogWarning(c.modifyVtx.Command);
								break;
						}
						vertexBufferMapping[ind] = verts.Count;
						verts.Add(vtx);
						break;
					case RSPCommand.Type.RSP_GBI1_Tri1:
						triangles.Add(new GeometricObjectElementTrianglesData.Triangle() {
							v1 = (ushort)vertexBufferMapping[c.tri1.v0],
							v2 = (ushort)vertexBufferMapping[c.tri1.v1],
							v3 = (ushort)vertexBufferMapping[c.tri1.v2],
						});
						break;
					case RSPCommand.Type.RSP_GBI1_Tri2:
						triangles.Add(new GeometricObjectElementTrianglesData.Triangle() {
							v1 = (ushort)vertexBufferMapping[c.tri2.v0],
							v2 = (ushort)vertexBufferMapping[c.tri2.v1],
							v3 = (ushort)vertexBufferMapping[c.tri2.v2],
						});
						triangles.Add(new GeometricObjectElementTrianglesData.Triangle() {
							v1 = (ushort)vertexBufferMapping[c.tri2.v3],
							v2 = (ushort)vertexBufferMapping[c.tri2.v4],
							v3 = (ushort)vertexBufferMapping[c.tri2.v5],
						});
						break;
					case RSPCommand.Type.RSP_GBI1_EndDL:
						parsing = false;
						break;
					default:
						Debug.LogError("Unparsed command: " + c.Command);
						break;
				}
				if (!parsing) break;
			}

			return verts.Select(v => v.GetVector(go.ScaleFactor)).ToArray();
		}

	}
}
