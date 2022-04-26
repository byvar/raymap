using BinarySerializer.Unity;
using Raymap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class GEO_GeometricObjectExtensions {


		public static GameObject GetGameObject(this GEO_GeometricObject geo, out GameObject[] boneGaos, GEO_GeometricObject morphObject = null, float morphProgress = 0f) {
			GameObject parentGao = new GameObject(geo.Offset.ToString());
			parentGao.AddBinarySerializableData(geo);

			// Bones
			boneGaos = null;
			if (geo.Bones != null && geo.Bones.Length > 0) {
				GameObject rootBone = new GameObject("Root bone");
				boneGaos = new GameObject[] { rootBone };
				boneGaos[0].transform.SetParent(parentGao.transform);
				boneGaos[0].transform.localPosition = Vector3.zero;
				boneGaos[0].transform.localRotation = Quaternion.identity;
				boneGaos[0].transform.localScale = Vector3.one;
				boneGaos = boneGaos.Concat(geo.Bones.Select(b => b.GetGameObject(parentGao))).ToArray();
			}

			// Morph
			Vector3[] mainVertices = geo.Vertices.Select(s => s.Position.GetUnityVector(convertAxes: true)).ToArray();
			Color[] mainColors = geo.Vertices.Select(s => new Color(
					s.Color.R / (float)0x80,
					s.Color.G / (float)0x80,
					s.Color.B / (float)0x80,
					1f)).ToArray();
			if (morphProgress > 0f && morphObject != null && morphObject.Vertices.Length == geo.Vertices.Length) {
				Vector3[] morphVertices = morphObject.Vertices.Select(s => s.Position.GetUnityVector(convertAxes: true)).ToArray();
				Color[] morphColors = morphObject.Vertices.Select(s => new Color(
					s.Color.R / (float)0x80,
					s.Color.G / (float)0x80,
					s.Color.B / (float)0x80,
					1f)).ToArray();
				for (int i = 0; i < geo.Vertices.Length; i++) {
					mainVertices[i] = Vector3.Lerp(mainVertices[i], morphVertices[i], morphProgress);
					mainColors[i] = Color.Lerp(mainColors[i], morphColors[i], morphProgress);
				}
			}

			// First pass
			Dictionary<GLI_VisualMaterial, List<GEO_IPS1Polygon>> textured = new Dictionary<GLI_VisualMaterial, List<GEO_IPS1Polygon>>();
			List<GEO_IPS1Polygon> untextured = new List<GEO_IPS1Polygon>();
			for (int i = 0; i < geo.PolygonLists.Length; i++) {
				GEO_PolygonList polys = geo.PolygonLists[i];
				if (polys.Polygons != null) {
					foreach (GEO_IPS1Polygon p in polys.Polygons) {
						if (p is GEO_QuadLOD && (p as GEO_QuadLOD).Quads?.Length > 0) {
							GEO_Quad[] quads = (p as GEO_QuadLOD).Quads;
							foreach (GEO_Quad q in quads) {
								GLI_VisualMaterial b = q.Material;
								if (b == null) {
									untextured.Add(q);
								} else {
									if (!textured.ContainsKey(b)) textured[b] = new List<GEO_IPS1Polygon>();
									textured[b].Add(q);
								}
							}
						} else {
							GLI_VisualMaterial b = p.Material;
							if (b == null) {
								untextured.Add(p);
							} else {
								if (!textured.ContainsKey(b)) textured[b] = new List<GEO_IPS1Polygon>();
								textured[b].Add(p);
							}
						}
					}
				}
			}

			// Second pass
			GLI_VisualMaterial[] textures = textured.Keys.ToArray();
			for (int i = 0; i < textures.Length; i++) {
				GLI_VisualMaterial vm = textures[i];
				GLI_Texture b = vm.Texture;

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

				GEO_IPS1Polygon pf = textured[vm].FirstOrDefault();
				GameObject gao = new GameObject(geo.Offset.ToString()
					+ " - " + i
					+ " - " + pf?.Offset
					+ " - " + pf?.GetType()
					+ " - " + string.Format("{0:X2}", vm.MaterialFlags)
					+ "|" + string.Format("{0:X2}", vm.Scroll)
					+ " - " + vm.BlendMode);
				gao.transform.SetParent(parentGao.transform);
				gao.layer = LayerMask.NameToLayer("Visual");
				gao.transform.localPosition = Vector3.zero;

				List<int> vertIndices = new List<int>();
				List<int> triIndices = new List<int>();
				List<Vector2> uvs = new List<Vector2>();
				foreach (GEO_IPS1Polygon p in textured[vm]) {
					int currentCount = vertIndices.Count;
					switch (p) {
						case GEO_Triangle t:
							vertIndices.Add(t.V0);
							vertIndices.Add(t.V1);
							vertIndices.Add(t.V2);

							uvs.Add(b.CalculateUV(t.X0, t.Y0).GetUnityVector());
							uvs.Add(b.CalculateUV(t.X1, t.Y1).GetUnityVector());
							uvs.Add(b.CalculateUV(t.X2, t.Y2).GetUnityVector());
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
						case GEO_Quad q:
							vertIndices.Add(q.V0);
							vertIndices.Add(q.V1);
							vertIndices.Add(q.V2);
							vertIndices.Add(q.V3);

							uvs.Add(b.CalculateUV(q.X0, q.Y0).GetUnityVector());
							uvs.Add(b.CalculateUV(q.X1, q.Y1).GetUnityVector());
							uvs.Add(b.CalculateUV(q.X2, q.Y2).GetUnityVector());
							uvs.Add(b.CalculateUV(q.X3, q.Y3).GetUnityVector());

							triIndices.Add(currentCount);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 2);

							triIndices.Add(currentCount + 2);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 3);
							break;
						case GEO_Sprite s:

							GameObject spr_gao = new GameObject("Sprite");
							spr_gao.transform.SetParent(gao.transform);
							spr_gao.transform.localPosition = mainVertices[s.V0];
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

							scale_x = ((float)s.Height / geo.Context.GetLevel().CoordinateFactor) / 2.0f;
							scale_y = ((float)s.Width / geo.Context.GetLevel().CoordinateFactor) / 2.0f;

							BoxCollider bc = spr_gao.AddComponent<BoxCollider>();
							bc.size = new Vector3(0, scale_y * 2, scale_x * 2);

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
				if (geo.Bones != null && geo.Bones.Length > 0 && geo.BoneWeights != null) {
					w = new BoneWeight[vertIndices.Count];
					for (int vi = 0; vi < w.Length; vi++) {
						GEO_DeformationVertexWeightSet dvw = geo.BoneWeights.FirstOrDefault(bw => bw.VertexIndex == vertIndices[vi]);
						if (dvw != null) {
							w[vi] = dvw.GetUnityWeight();
						} else {
							w[vi] = new BoneWeight() { boneIndex0 = 0, weight0 = 1f };
						}
					}
				}
				if (vertIndices.Any()) {
					MeshFilter mf = gao.AddComponent<MeshFilter>();
					gao.AddComponent<ExportableModel>();
					MeshRenderer mr = null;
					SkinnedMeshRenderer smr = null;
					Matrix4x4[] bindPoses = null;
					if (geo.Bones == null || geo.Bones.Length <= 0) {
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
			}

			// Untextured (some skyboxes, etc)
			if (untextured.Count > 0) {
				GameObject gao = new GameObject(geo.Offset.ToString() + " - Untextured");
				gao.transform.SetParent(parentGao.transform);
				gao.layer = LayerMask.NameToLayer("Visual");
				gao.transform.localPosition = Vector3.zero;
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				gao.AddComponent<ExportableModel>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				List<int> vertIndices = new List<int>();
				List<int> triIndices = new List<int>();

				foreach (GEO_IPS1Polygon p in untextured) {
					int currentCount = vertIndices.Count;
					switch (p) {
						case GEO_TriangleNoTexture t:
							vertIndices.Add(t.V0);
							vertIndices.Add(t.V1);
							vertIndices.Add(t.V2);

							triIndices.Add(currentCount);
							triIndices.Add(currentCount + 1);
							triIndices.Add(currentCount + 2);
							break;
						case GEO_QuadNoTexture q:
							vertIndices.Add(q.V0);
							vertIndices.Add(q.V1);
							vertIndices.Add(q.V2);
							vertIndices.Add(q.V3);

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
				baseMaterial = ((Unity_Environment_CPA)geo.Context.GetUnityEnvironment()).MaterialVisualOpaque;
				//}
				Material mat = new Material(baseMaterial);
				mat.SetInt("_NumTextures", 1);
				string textureName = "_Tex0";
				Texture2D tex = TextureHelpers.GrayTexture();
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
