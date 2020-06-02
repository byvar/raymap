using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class GeometricObject : OpenSpaceStruct { // Sectors?
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
		public short currentScrollValue;
		public short short_1E;

		// Parsed
		public Vertex[] vertices;
		public PolygonList[] triangleLists;

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
			currentScrollValue = reader.ReadInt16();
			short_1E = reader.ReadInt16();

			vertices = Load.ReadArray<Vertex>(num_vertices, reader, off_vertices);
			triangleLists = Load.ReadArray<PolygonList>(num_triangleLists, reader, off_triangleLists);
			//CreateGAO();
		}

		public GameObject GetGameObject() {
			GameObject parentGao = new GameObject(Offset.ToString());
			// First pass
			
			Dictionary<VisualMaterial, List<IPS1Polygon>> textured = new Dictionary<VisualMaterial, List<IPS1Polygon>>();
			List<IPS1Polygon> untextured = new List<IPS1Polygon>();
			for (int i = 0; i < triangleLists.Length; i++) {
				PolygonList polys = triangleLists[i];
				if (polys.polygons != null) {
					foreach (IPS1Polygon p in polys.polygons) {
						if (p is QuadLOD && (p as QuadLOD).quads?.Length > 0) {
							Quad[] quads = (p as QuadLOD).quads;
							foreach (Quad q in quads) {
								VisualMaterial b = q.Material;
								if (b == null) {
									untextured.Add(q);
								} else {
									if (!textured.ContainsKey(b)) textured[b] = new List<IPS1Polygon>();
									textured[b].Add(q);
								}
							}
						} else {
							VisualMaterial b = p.Material;
							if (b == null) {
								untextured.Add(p);
							} else {
								if (!textured.ContainsKey(b)) textured[b] = new List<IPS1Polygon>();
								textured[b].Add(p);
							}
						}
					}
				}
			}

			// Second pass
			VisualMaterial[] textures = textured.Keys.ToArray();
			for (int i = 0; i < textures.Length; i++) {
				VisualMaterial vm = textures[i];
				TextureBounds b = vm.texture;

				float alpha = 1f;
				//if (!vm.IsLight) {
					/*switch (vm.BlendMode) {
						case VisualMaterial.SemiTransparentMode.Point25:
							alpha = 0.25f * 4f;
							break;
						case VisualMaterial.SemiTransparentMode.Point5:
							alpha = 0.5f * 4f;
							break;
					}*/
				//}

				IPS1Polygon pf = textured[vm].FirstOrDefault();
				GameObject gao = new GameObject(Offset.ToString()
					+ " - " + i
					+ " - " + pf?.Offset
					+ " - " + pf?.GetType()
					+ " - " + string.Format("{0:X2}",vm.materialFlags)
					+ "|" + string.Format("{0:X2}", vm.scroll)
					+ " - " + vm.BlendMode);
				gao.transform.SetParent(parentGao.transform);
				gao.transform.localPosition = Vector3.zero;
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				gao.AddComponent<R3AnimatedMesh>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				
				List<int> vertIndices = new List<int>();
				List<int> triIndices = new List<int>();
				List<Vector2> uvs = new List<Vector2>();
				foreach (IPS1Polygon p in textured[vm]) {
					int currentCount = vertIndices.Count;
					switch (p) {
						case Triangle t:
							vertIndices.Add(t.v0);
							vertIndices.Add(t.v1);
							vertIndices.Add(t.v2);

							uvs.Add(b.CalculateUV(t.x0, t.y0));
							uvs.Add(b.CalculateUV(t.x1, t.y1));
							uvs.Add(b.CalculateUV(t.x2, t.y2));
							/*Vector2 uv0 = b.CalculateUV(t.x0, t.y0);
							Vector2 uv1 = b.CalculateUV(t.x1, t.y1);
							Vector2 uv2 = b.CalculateUV(t.x2, t.y2);
							uvs.Add(uv0);
							uvs.Add(uv1);
							uvs.Add(uv2);*/

							triIndices.Add(currentCount);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 2);
							break;
						case Quad q:
							vertIndices.Add(q.v0);
							vertIndices.Add(q.v1);
							vertIndices.Add(q.v2);
							vertIndices.Add(q.v3);

							uvs.Add(b.CalculateUV(q.x0, q.y0));
							uvs.Add(b.CalculateUV(q.x1, q.y1));
							uvs.Add(b.CalculateUV(q.x2, q.y2));
							uvs.Add(b.CalculateUV(q.x3, q.y3));

							triIndices.Add(currentCount);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 2);
							
							triIndices.Add(currentCount + 2);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 3);
							break;
                        case Sprite s:

                            GameObject spr_gao = new GameObject("Sprite");
                            spr_gao.transform.SetParent(gao.transform);
							Vertex spr_v = this.vertices[s.v0];
							spr_gao.transform.localPosition = new Vector3(spr_v.x / 256, spr_v.z / 256f, spr_v.y / 256f);
                            BillboardBehaviour billboard = spr_gao.AddComponent<BillboardBehaviour>();
                            billboard.mode = BillboardBehaviour.LookAtMode.ViewRotation;
                            MeshFilter sprites_mf = spr_gao.AddComponent<MeshFilter>();
                            MeshRenderer sprites_mr = spr_gao.AddComponent<MeshRenderer>();

                            spr_gao.layer = LayerMask.NameToLayer("Visual");

                            //Material unityMat = sprites[i].visualMaterial.MaterialBillboard;
                            
                            sprites_mr.receiveShadows = false;
                            sprites_mr.material = vm.CreateMaterial();

                            var meshUnity = new Mesh();
                            Vector3[] vertices = new Vector3[4];

                            float scale_x = 1.0f;
                            float scale_y = 1.0f;

                            scale_x = ((float)s.height / 256f) / 2.0f;
                            scale_y = ((float)s.width / 256f) / 2.0f;

                            vertices[0] = new Vector3(0, -scale_y, -scale_x);
                            vertices[1] = new Vector3(0, -scale_y, scale_x);
                            vertices[2] = new Vector3(0, scale_y, -scale_x);
                            vertices[3] = new Vector3(0, scale_y, scale_x);
                            Vector3[] normals = new Vector3[4];
                            normals[0] = Vector3.forward;
                            normals[1] = Vector3.forward;
                            normals[2] = Vector3.forward;
                            normals[3] = Vector3.forward;
                            Vector3[] sprite_uvs = new Vector3[4];

                            bool mirrorX = false;
                            bool mirrorY = false;

                            sprite_uvs[0] = new Vector3(0, 0 - (mirrorY ? 1 : 0), alpha);
                            sprite_uvs[1] = new Vector3(1 + (mirrorX ? 1 : 0), 0 - (mirrorY ? 1 : 0), alpha);
                            sprite_uvs[2] = new Vector3(0, 1, alpha);
                            sprite_uvs[3] = new Vector3(1 + (mirrorX ? 1 : 0), 1, alpha);
                            int[] triangles = new int[] { 0, 2, 1, 1, 2, 3 };

                            meshUnity.vertices = vertices;
                            meshUnity.normals = normals;
                            meshUnity.triangles = triangles;
                            meshUnity.SetUVs(0, sprite_uvs.ToList());
                            sprites_mf.sharedMesh = meshUnity;
                            
                            break;
					}
				}
				Vertex[] v = vertIndices.Select(vi => vertices[vi]).ToArray();
				Mesh m = new Mesh();
				m.vertices = v.Select(s => new Vector3(
					s.x / 256f,
					s.z / 256f,
					s.y / 256f)).ToArray();
				m.colors = v.Select(s => new Color(
					s.r / (float)0x80,
					s.g / (float)0x80,
					s.b / (float)0x80,
					1f)).ToArray();
				m.SetUVs(0, uvs.Select(s => new Vector4(s.x, s.y, alpha, 0f)).ToList());
				m.triangles = triIndices.ToArray();
				m.RecalculateNormals();
				mf.mesh = m;

				mr.material = vm.CreateMaterial();
			}
			// Untextured (some skyboxes, etc)
			if(untextured.Count > 0) {
				GameObject gao = new GameObject(Offset.ToString() + " - Untextured");
				gao.transform.SetParent(parentGao.transform);
				gao.transform.localPosition = Vector3.zero;
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				gao.AddComponent<R3AnimatedMesh>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				List<int> vertIndices = new List<int>();
				List<int> triIndices = new List<int>();

				foreach (IPS1Polygon p in untextured) {
					int currentCount = vertIndices.Count;
					switch (p) {
						case TriangleNoTexture t:
							vertIndices.Add(t.v0);
							vertIndices.Add(t.v1);
							vertIndices.Add(t.v2);

							triIndices.Add(currentCount);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 2);
							break;
						case QuadNoTexture q:
							vertIndices.Add(q.v0);
							vertIndices.Add(q.v1);
							vertIndices.Add(q.v2);
							vertIndices.Add(q.v3);

							triIndices.Add(currentCount);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 2);

							triIndices.Add(currentCount + 2);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 3);
							break;
						default:
							Debug.LogWarning(p.GetType()); break;
					}
				}
				Vertex[] v = vertIndices.Select(vi => vertices[vi]).ToArray();
				Mesh m = new Mesh();
				m.vertices = v.Select(s => new Vector3(
					s.x / 256f,
					s.z / 256f,
					s.y / 256f)).ToArray();
				m.colors = v.Select(s => new Color(
					s.r / (float)0x80,
					s.g / (float)0x80,
					s.b / (float)0x80,
					1f)).ToArray();

				m.SetUVs(0, v.Select(s => new Vector4(0.5f, 0.5f, 1f, 1f)).ToList());
				m.triangles = triIndices.ToArray();
				m.RecalculateNormals();
				mf.mesh = m;


				Material baseMaterial;
				/*if (m.colors.Any(c => c.a != 1f)) {
					baseMaterial = Load.baseTransparentMaterial;
				} else {*/
					baseMaterial = Load.baseMaterial;
				//}
				Material mat = new Material(baseMaterial);
				mat.SetInt("_NumTextures", 1);
				string textureName = "_Tex0";
				Texture2D tex = Util.GrayTexture();
				mat.SetTexture(textureName, tex);
				mat.SetVector("_AmbientCoef", Vector4.one);
				mat.SetFloat("_Prelit", 1f);
				mr.material = mat;
			}
			
			/*for (int i = 0; i < triangleLists.Length; i++) {
				PolygonList tris = triangleLists[i];
				GameObject gao = new GameObject(Offset.ToString() + " - " + tris.Offset + " - " + tris.type);
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
						Quad q = tris.polygons[j] as Quad;
						triangles.Add(q.v0);
						triangles.Add(q.v1);
						triangles.Add(q.v2);

						triangles.Add(q.v2);
						triangles.Add(q.v1);
						triangles.Add(q.v3);
					}
				} else if (tris.type == 1) {
					for (int j = 0; j < tris.length; j++) {
						QuadLOD q = tris.polygons[j] as QuadLOD;
						if (q.length > 0) {
							for (int k = 0; k < q.quads.Length; k++) {
								triangles.Add(q.quads[k].v0);
								triangles.Add(q.quads[k].v1);
								triangles.Add(q.quads[k].v2);

								triangles.Add(q.quads[k].v1);
								triangles.Add(q.quads[k].v3);
								triangles.Add(q.quads[k].v2);
							}
						} else {
							triangles.Add(q.v0);
							triangles.Add(q.v1);
							triangles.Add(q.v2);

							triangles.Add(q.v1);
							triangles.Add(q.v3);
							triangles.Add(q.v2);
						}
					}
				} else if (tris.type == 3) {
					for (int j = 0; j < tris.length; j++) {
						TriangleNoTexture t = tris.polygons[j] as TriangleNoTexture;
						triangles.Add(t.v0);
						triangles.Add(t.v1);
						triangles.Add(t.v2);
					}
				} else if(tris.type == 4) {
					for (int j = 0; j < tris.length; j++) {
						QuadNoTexture q = tris.polygons[j] as QuadNoTexture;
						triangles.Add(q.v0);
						triangles.Add(q.v1);
						triangles.Add(q.v2);

						triangles.Add(q.v2);
						triangles.Add(q.v1);
						triangles.Add(q.v3);
					}
				} else if (tris.type == 5) {
					for (int j = 0; j < tris.length; j++) {
						Triangle t = tris.polygons[j] as Triangle;
						triangles.Add(t.v0);
						triangles.Add(t.v1);
						triangles.Add(t.v2);
					}
				}
				m.triangles = triangles.ToArray();
				m.RecalculateNormals();
				mf.mesh = m;
			}*/
			return parentGao;
		}
	}
}
