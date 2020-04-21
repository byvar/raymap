using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1StaticGeometricObject : OpenSpaceStruct { // Sectors?
		public uint uint_00;
		public ushort num_vertices;
		public ushort num_triangleLists;
		public short short_08;
		public short short_0A;
		public short short_0C;
		public ushort ushort_0E;
		public Pointer off_vertices;
		public Pointer off_triangleLists;
		public short short_18;
		public short short_1A;
		public short short_1C;
		public short short_1E;

		// Parsed
		public PS1Vertex[] vertices;
		public PS1TriangleList[] triangleLists;

		protected override void ReadInternal(Reader reader) {
			uint_00 = reader.ReadUInt32();
			num_vertices = reader.ReadUInt16();
			num_triangleLists = reader.ReadUInt16();
			short_08 = reader.ReadInt16();
			short_0A = reader.ReadInt16();
			short_0C = reader.ReadInt16();
			ushort_0E = reader.ReadUInt16();
			off_vertices = Pointer.Read(reader);
			off_triangleLists = Pointer.Read(reader);
			short_18 = reader.ReadInt16();
			short_1A = reader.ReadInt16();
			short_1C = reader.ReadInt16();
			short_1E = reader.ReadInt16();

			vertices = Load.ReadArray<PS1Vertex>(num_vertices, reader, off_vertices);
			triangleLists = Load.ReadArray<PS1TriangleList>(num_triangleLists, reader, off_triangleLists);
			//CreateGAO();
		}

		public GameObject CreateGAO() {
			GameObject parentGao = new GameObject(Offset.ToString());
			parentGao.transform.position = new Vector3(
				short_08 / 256f,
				short_0C / 256f,
				short_0A / 256f);
			for (int i = 0; i < triangleLists.Length; i++) {
				PS1TriangleList tris = triangleLists[i];
				GameObject gao = new GameObject(Offset.ToString());
				gao.transform.SetParent(parentGao.transform);
				gao.transform.localPosition = Vector3.zero;
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				//mr.material = Load.collideMaterial;
				Material mat = new Material(Load.baseMaterial);
				mat.SetInt("_NumTextures", 1);
				string textureName = "_Tex0";
				Texture2D tex = new Texture2D(1, 1);
				tex.SetPixel(0, 0, Color.white);
				tex.Apply();
				mat.SetTexture(textureName, tex);
				mat.SetVector("_AmbientCoef", Vector4.one);
				mat.SetFloat("_Prelit", 1f);
				mr.material = mat;
				//mr.material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f); //UnityEngine.Random.ColorHSV(
				Mesh m = new Mesh();
				m.vertices = vertices.Select(s => new Vector3(
					s.x / 256f,
					s.z / 256f,
					s.y / 256f)).ToArray();
				m.colors = vertices.Select(s => new Color(
					s.r / (float)0x80,
					s.g / (float)0x80,
					s.b / (float)0x80,
					1f)).ToArray();
				m.SetUVs(0,vertices.Select(s => new Vector4(0.5f, 0.5f, 0.5f, 1f)).ToList());
				List<int> triangles = new List<int>();
				if (tris.type == 6) {
					for (int j = 0; j < tris.length; j++) {
						triangles.Add(tris.quads[j].v0);
						triangles.Add(tris.quads[j].v1);
						triangles.Add(tris.quads[j].v2);

						triangles.Add(tris.quads[j].v2);
						triangles.Add(tris.quads[j].v1);
						triangles.Add(tris.quads[j].v3);
					}
				} else if (tris.type == 5) {
					for (int j = 0; j < tris.length; j++) {
						triangles.Add(tris.triangles[j].v0);
						triangles.Add(tris.triangles[j].v1);
						triangles.Add(tris.triangles[j].v2);
					}
				}
				m.triangles = triangles.ToArray();
				/*if (tris.type == 6) {
					m.triangles = tris.struct6.SelectMany(s => new int[] { s.ushort_00, s.ushort_02, s.ushort_04 }).ToArray();
				} else if (tris.type == 5) {
					m.triangles = tris.struct5.SelectMany(s => new int[] { s.ushort_00, s.ushort_02, s.ushort_04 }).ToArray();
				}*/
				m.RecalculateNormals();
				mf.mesh = m;
			}
			return parentGao;
		}
	}
}
