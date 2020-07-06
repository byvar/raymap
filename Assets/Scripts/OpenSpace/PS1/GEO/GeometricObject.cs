using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class GeometricObject : OpenSpaceStruct {
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

		// VIP & JB
		public ushort num_bones;
		public ushort num_boneWeights;
		public ushort num_boneUnk;
		public ushort num_unk4;
		public Pointer off_bones;
		public Pointer off_boneWeights;
		public Pointer off_boneUnk;
		public Pointer off_unk4;
		public uint off_bones_;
		public uint off_boneWeights_;
		public uint off_boneUnk_;
		public uint off_unk4_;

		// Parsed
		public Vertex[] vertices;
		public PolygonList[] triangleLists;
		public DeformBone[] bones;
		public DeformVertexWeights[] boneWeights;
		public DeformVertexUnknown[] boneUnk;

		protected override void ReadInternal(Reader reader) {
			//Load.print(Offset);
			uint_00 = reader.ReadUInt32();
			num_vertices = reader.ReadUInt16();
			num_triangleLists = reader.ReadUInt16();
			short_08 = reader.ReadInt16();
			short_0A = reader.ReadInt16();
			short_0C = reader.ReadInt16();
			ushort_0E = reader.ReadUInt16();
			off_vertices = Pointer.Read(reader);
			off_triangleLists = Pointer.Read(reader);
			if (Settings.s.game == Settings.Game.VIP || Settings.s.game == Settings.Game.JungleBook) {
				num_bones = reader.ReadUInt16();
				num_boneWeights = reader.ReadUInt16();
				num_boneUnk = reader.ReadUInt16();
				num_unk4 = reader.ReadUInt16();
				if (num_bones > 0) {
					off_bones = Pointer.Read(reader);
				} else {
					off_bones_ = reader.ReadUInt32();
				}
				if (num_boneWeights > 0) {
					off_boneWeights = Pointer.Read(reader);
				} else {
					off_boneWeights_ = reader.ReadUInt32();
				}
				if (num_boneUnk > 0) {
					off_boneUnk = Pointer.Read(reader);
				} else {
					off_boneUnk_ = reader.ReadUInt32();
				}
				if (num_unk4 > 0) {
					off_unk4 = Pointer.Read(reader);
				} else {
					off_unk4_ = reader.ReadUInt32();
				}
			} else {
				short_18 = reader.ReadInt16();
				short_1A = reader.ReadInt16();
				currentScrollValue = reader.ReadInt16();
				short_1E = reader.ReadInt16();
			}

			vertices = Load.ReadArray<Vertex>(num_vertices, reader, off_vertices);
			triangleLists = Load.ReadArray<PolygonList>(num_triangleLists, reader, off_triangleLists);
			bones = Load.ReadArray<DeformBone>(num_bones, reader, off_bones);
			boneWeights = Load.ReadArray<DeformVertexWeights>(num_boneWeights, reader, off_boneWeights);
			boneUnk = Load.ReadArray<DeformVertexUnknown>(num_boneUnk, reader, off_boneUnk);
			//CreateGAO();
		}

		public GameObject GetGameObject(out GameObject[] boneGaos, GeometricObject morphObject = null, float morphProgress = 0f) {
			GameObject parentGao = new GameObject(Offset.ToString());

			// Bones
			boneGaos = null;
			if (bones != null && bones.Length > 0) {
				GameObject rootBone = new GameObject("Root bone");
				boneGaos = new GameObject[] { rootBone };
				boneGaos[0].transform.SetParent(parentGao.transform);
				boneGaos[0].transform.localPosition = Vector3.zero;
				boneGaos[0].transform.localRotation = Quaternion.identity;
				boneGaos[0].transform.localScale = Vector3.one;
				boneGaos = boneGaos.Concat(bones.Select(b => b.GetGameObject(parentGao))).ToArray();
			}

			// Morph
			Vector3[] mainVertices = vertices.Select(s => new Vector3(
					s.x / 256f,
					s.z / 256f,
					s.y / 256f)).ToArray();
			Color[] mainColors = vertices.Select(s => new Color(
					s.r / (float)0x80,
					s.g / (float)0x80,
					s.b / (float)0x80,
					1f)).ToArray();
			if (morphProgress > 0f && morphObject != null && morphObject.vertices.Length == vertices.Length) {
				Vector3[] morphVertices = morphObject.vertices.Select(s => new Vector3(
					s.x / 256f,
					s.z / 256f,
					s.y / 256f)).ToArray();
				Color[] morphColors = morphObject.vertices.Select(s => new Color(
					s.r / (float)0x80,
					s.g / (float)0x80,
					s.b / (float)0x80,
					1f)).ToArray();
				for (int i = 0; i < vertices.Length; i++) {
					mainVertices[i] = Vector3.Lerp(mainVertices[i], morphVertices[i], morphProgress);
					mainColors[i] = Color.Lerp(mainColors[i], morphColors[i], morphProgress);
				}
			}

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
				gao.layer = LayerMask.NameToLayer("Visual");
				gao.transform.localPosition = Vector3.zero;
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				gao.AddComponent<ExportableModel>();
				MeshRenderer mr = null;
				SkinnedMeshRenderer smr = null;
				Matrix4x4[] bindPoses = null;
				if (bones == null || bones.Length <= 0) {
					mr = gao.AddComponent<MeshRenderer>();
				} else {
					smr = gao.AddComponent<SkinnedMeshRenderer>();
					//smr = (SkinnedMeshRenderer)mr;
					smr.bones = boneGaos.Select(bo => bo.transform).ToArray();
					bindPoses = new Matrix4x4[smr.bones.Length];
					for (int bi = 0; bi < smr.bones.Length; bi++) {
						bindPoses[bi] = smr.bones[bi].worldToLocalMatrix * parentGao.transform.localToWorldMatrix;
					}
					smr.rootBone = smr.bones[0];
				}
				
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
							spr_gao.transform.localPosition = mainVertices[s.v0];
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
				//Vertex[] v = vertIndices.Select(vi => vertices[vi]).ToArray();
				BoneWeight[] w = null;
				if (bones != null && bones.Length > 0 && boneWeights != null) {
					w = new BoneWeight[vertIndices.Count];
					for (int vi = 0; vi < w.Length; vi++) {
						DeformVertexWeights dvw = boneWeights.FirstOrDefault(bw => bw.ind_vertex == vertIndices[vi]);
						if (dvw != null) {
							w[vi] = dvw.UnityWeight;
						} else {
							w[vi] = new BoneWeight() { boneIndex0 = 0, weight0 = 1f };
						}
					}
				}
				Mesh m = new Mesh();
				m.vertices = vertIndices.Select(vi => mainVertices[vi]).ToArray();
				m.colors = vertIndices.Select(vi => mainColors[vi]).ToArray();
				m.SetUVs(0, uvs.Select(s => new Vector4(s.x, s.y, alpha, 0f)).ToList());
				m.triangles = triIndices.ToArray();
				m.RecalculateNormals();
				if (w != null) {
					m.boneWeights = w;
					m.bindposes = bindPoses;
				}
				mf.mesh = m;

				if (mr != null) {
					mr.material = vm.CreateMaterial();
				} else if (smr != null) {
					smr.material = vm.CreateMaterial();
					smr.sharedMesh = m;
				}
				try {
					MeshCollider mc = gao.AddComponent<MeshCollider>();
					mc.isTrigger = false;
					//mc.cookingOptions = MeshColliderCookingOptions.None;
					//mc.sharedMesh = mesh;
				} catch (Exception) { }
			}
			// Untextured (some skyboxes, etc)
			if(untextured.Count > 0) {
				GameObject gao = new GameObject(Offset.ToString() + " - Untextured");
				gao.transform.SetParent(parentGao.transform);
				gao.transform.localPosition = Vector3.zero;
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				gao.AddComponent<ExportableModel>();
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
				//Vertex[] v = vertIndices.Select(vi => vertices[vi]).ToArray();
				Mesh m = new Mesh();
				m.vertices = vertIndices.Select(vi => mainVertices[vi]).ToArray();
				m.colors = vertIndices.Select(vi => mainColors[vi]).ToArray();
				m.SetUVs(0, vertIndices.Select(s => new Vector4(0.5f, 0.5f, 1f, 1f)).ToList());
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

				try {
					MeshCollider mc = gao.AddComponent<MeshCollider>();
					mc.isTrigger = false;
					//mc.cookingOptions = MeshColliderCookingOptions.None;
					//mc.sharedMesh = mesh;
				} catch (Exception) { }
			}

			return parentGao;
		}
	}
}
