using Newtonsoft.Json;
using OpenSpace.Loader;
using OpenSpace.Visual.PS2Optimized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Basically a submesh
    /// </summary>
    public class GeometricObjectElementTriangles : IGeometricObjectElement {
        [JsonIgnore] public GeometricObject geo;
        public Pointer offset;

        [JsonIgnore]
        public string name;
        public Pointer off_material;
        public GameMaterial gameMaterial;
		public VisualMaterial visualMaterialOG;
		public VisualMaterial visualMaterial;
        public bool backfaceCulling;
        public ushort num_triangles;
        public ushort num_uvs;
        public ushort num_uvMaps;
        public Pointer off_triangles;
        public Pointer off_mapping_uvs;
        public Pointer off_normals;
        public Pointer off_uvs;
        public Pointer off_vertex_indices;
        public ushort num_vertex_indices;
        public ushort OPT_num_mapping_entries;
        public Pointer OPT_off_mapping_vertices;
        public Pointer OPT_off_mapping_uvs;
        public ushort OPT_num_triangleStrip;
        public ushort OPT_num_disconnectedTriangles;
        public Pointer OPT_off_triangleStrip;
        public Pointer OPT_off_disconnectedTriangles;
		public Pointer off_mapping_lightmap;
		public ushort num_mapping_lightmap;
		public ushort parallelBox;
		public byte isVisibleInPortal;

        public int[] OPT_mapping_vertices = null;
        public int[][] OPT_mapping_uvs = null;
        public Vector2[] uvs = null;
        public int[] OPT_triangleStrip = null;
        public int[] OPT_disconnectedTriangles = null;
        public int[][] mapping_uvs = null;
        public int[] triangles = null;
		public int[] mapping_lightmap = null;
        public Vector3[] normals = null;

		// Revolution
		public Color[] vertexColors = null;
		public int lightmap_index = -1;

		// R3 PS2
		public Pointer off_sdc_mapping = null;
		public ushort[] sdc_mapping;
		public PS2OptimizedSDCStructureElement sdc = null;

        private SkinnedMeshRenderer OPT_s_mr = null;
        private SkinnedMeshRenderer s_mr = null;
        private Renderer OPT_mr = null;
        private Renderer mr = null;
        private Mesh OPT_unityMesh = null;
        private Mesh unityMesh = null;
		private Texture2D lightmap = null;
		private Vector2[] lightmapUVs = null;


        private GameObject gao = null;

		public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);// Create object and read triangle data
                    gao.layer = LayerMask.NameToLayer("Visual");
                    CreateUnityMesh();
                }
                return gao;
            }
        }

        public GeometricObjectElementTriangles(Pointer offset, GeometricObject geo) {
            this.geo = geo;
            this.offset = offset;
        }

		private void CreateUnityMeshFromSDC() {
			/*if (sdc.geo.Type == 6) {
				// Just fill in things based on mapping
				return;
			}*/
			BoneWeight[] new_boneWeights = null;
			if (OPT_unityMesh != null) {
				OPT_unityMesh = CopyMesh(OPT_unityMesh);
			} else {
				bool backfaceCulling = false;
				int triangle_size = 3 * (int)(backfaceCulling ? 1 : 2);
				if (sdc.geo.Type == 4 || sdc.geo.Type == 5 || sdc.geo.Type == 6) {
					int[] triangles = new int[triangle_size * sdc.geo.num_triangles[sdc.index]];
					OPT_unityMesh = new Mesh();
					Vector3[] vertices = sdc.vertices.Select(v => new Vector3(v.x, v.z, v.y)).ToArray();
					Array.Resize(ref vertices, (int)sdc.num_vertices_actual);
					OPT_unityMesh.vertices = vertices;
					new_boneWeights = (geo.bones == null || sdc_mapping == null) ? null : new BoneWeight[vertices.Length];
					normals = null;
					int currentTriInStrip = 0;
					int triangleIndex = 0;
					for (int v = 2; (v < sdc.num_vertices_actual && triangleIndex < triangles.Length); v++) {
						//if (sdc.vertices[v].Equals(sdc.vertices[v - 1])) continue;
						if (triangleIndex >= triangles.Length) MapLoader.Loader.print(
							offset 
							+ " - " + sdc.geo.Offset
							+ " - " + sdc.offset 
							+ " - " + sdc.geo.Type
							+ " - " + sdc.geo.num_triangles[sdc.index]
							+ " - " + num_triangles
							+ " - " + triangleIndex
							+ " - " + v);
						if (sdc.vertices[v].w == 1f) {
							if ((currentTriInStrip) % 2 == 0) {
								triangles[triangleIndex + 0] = v - 2;
								triangles[triangleIndex + 1] = v - 1;
								triangles[triangleIndex + 2] = v - 0;
								if (!backfaceCulling) {
									triangles[triangleIndex + 3] = v - 1;
									triangles[triangleIndex + 4] = v - 2;
									triangles[triangleIndex + 5] = v - 0;
								}
							} else {
								triangles[triangleIndex + 0] = v - 1;
								triangles[triangleIndex + 1] = v - 2;
								triangles[triangleIndex + 2] = v - 0;
								if (!backfaceCulling) {
									triangles[triangleIndex + 3] = v - 2;
									triangles[triangleIndex + 4] = v - 1;
									triangles[triangleIndex + 5] = v - 0;
								}
							}
							triangleIndex += triangle_size;
							currentTriInStrip++;
						} else {
							currentTriInStrip = 0;
						}
					}

					uint num_textures = Math.Max(1, visualMaterial.num_textures_in_material);
					for (int t = 0; t < num_textures; t++) {
						List<Vector3> uv = Enumerable.Range(0, vertices.Length).Select(v => sdc.GetUV(v, t, applyBlendWeight: sdc.geo.Type != 6)).ToList();
						OPT_unityMesh.SetUVs(t, uv.ToArray());
					}
					/*Vector3[] normals = Enumerable.Range(0, vertices.Length).Select(i => sdc.GetNormal(i)).ToArray();
					OPT_unityMesh.normals = normals;*/
					/*for (int i = 0; i < normals.Length; i++) {
						GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
						g.transform.position = vertices[i] + normals[i];
						g.transform.localScale = Vector3.one * 0.2f;
					}*/
					if (sdc.geo.Type != 6) {
						Vector4[] colors = Enumerable.Range(0, vertices.Length).Select(i => sdc.GetColor(i)).ToArray();
						OPT_unityMesh.SetUVs((int)num_textures, colors);
					} else {
						Vector3[] calculatedNormals = new Vector3[geo.num_vertices];
						if (sdc.normals == null && geo.normals == null) {
							// Calculate normals here
							Mesh tempMesh = new Mesh();
							Vector3[] new_vertices = new Vector3[num_triangles * 3];
							int triangles_index = 0;
							int[] unityTriangles = new int[num_triangles * 3];
							for (int j = 0; j < num_triangles; j++, triangles_index += 3) {
								int i0 = this.triangles[(j * 3) + 0], m0 = (j * 3) + 0; // Old index, mapped index
								int i1 = this.triangles[(j * 3) + 1], m1 = (j * 3) + 1;
								int i2 = this.triangles[(j * 3) + 2], m2 = (j * 3) + 2;
								new_vertices[m0] = geo.vertices[i0];
								new_vertices[m1] = geo.vertices[i1];
								new_vertices[m2] = geo.vertices[i2];
								unityTriangles[triangles_index + 0] = m0;
								unityTriangles[triangles_index + 1] = m2;
								unityTriangles[triangles_index + 2] = m1;
							}
							tempMesh.vertices = new_vertices;
							tempMesh.triangles = unityTriangles;
							tempMesh.RecalculateNormals();
							triangles_index = 0;
							for (int j = 0; j < num_triangles; j++, triangles_index += 3) {
								int i0 = this.triangles[(j * 3) + 0], m0 = (j * 3) + 0; // Old index, mapped index
								int i1 = this.triangles[(j * 3) + 1], m1 = (j * 3) + 1;
								int i2 = this.triangles[(j * 3) + 2], m2 = (j * 3) + 2;
								calculatedNormals[i0] = tempMesh.normals[m0];
								calculatedNormals[i1] = tempMesh.normals[m1];
								calculatedNormals[i2] = tempMesh.normals[m2];
							}

						}
						normals = new Vector3[vertices.Length];
						// Also set bone weights
						ushort mapping_i = 0;
						for (int i = 0; i < geo.vertices.Length; i++) {
							ushort length = sdc_mapping[mapping_i];
							mapping_i++;
							if (mapping_i >= sdc_mapping.Length) break;
							for (int j = 0; j < length; j++) {
								ushort vertIndexInSdc = (ushort)(sdc_mapping[mapping_i] & 0x7FFF);
								if (vertIndexInSdc < vertices.Length) {
									if (new_boneWeights != null) {
										new_boneWeights[vertIndexInSdc] = geo.bones.weights[i];
									}
									if (geo.normals != null) {
										normals[vertIndexInSdc] = geo.normals[i];
									} else {
										normals[vertIndexInSdc] = calculatedNormals[i];
									}
									//normals[vertIndexInSdc] = geo.normals[i];
								}
								mapping_i++;
								if (mapping_i >= sdc_mapping.Length) break;
							}
						}
					}
					/*m.triangles = Enumerable.Range(0, sdcEl.vertices.Length).ToArray();
					Debug.LogWarning(sdcEl.offset + " - " + sdc.Type + " - " + (m.triangles.Length / 3) + " - " + (m.triangles.Length % 3) + " - " + sdc.num_triangles[sdcIndex]);
					*/
					OPT_unityMesh.triangles = triangles;

					if (sdc.normals != null) {
						OPT_unityMesh.normals = Enumerable.Range(0, vertices.Length).Select(v => sdc.GetNormal(v)).ToArray();
					} else if (sdc.geo.Type == 6) {
						OPT_unityMesh.normals = normals;
					} else {
						OPT_unityMesh.RecalculateNormals();
					}
					//m.uv = sdcEl.uvUnoptimized.Select(uv => new Vector2(uv.u, uv.v)).ToArray();
					//m.uv = 
					//OPT_unityMesh.RecalculateNormals();
				} else {
					OPT_unityMesh = new Mesh();
					Vector3[] vertices = sdc.vertices.Select(v => new Vector3(v.x, v.z, v.y)).ToArray();
					OPT_unityMesh.vertices = vertices;
					OPT_unityMesh.triangles = Enumerable.Range(0, vertices.Length).ToArray();
					uint num_textures = Math.Max(1, visualMaterial.num_textures_in_material);
					for (int t = 0; t < num_textures; t++) {
						List<Vector3> uv = Enumerable.Range(0, vertices.Length).Select(v => sdc.GetUV(v, t, applyBlendWeight: true)).ToList();
						OPT_unityMesh.SetUVs(t, uv.ToArray());
					}
					Vector4[] colors = Enumerable.Range(0, vertices.Length).Select(i => sdc.GetColor(i)).ToArray();
					OPT_unityMesh.SetUVs((int)num_textures, colors);
					OPT_unityMesh.RecalculateNormals();
					//m.uv = 
					//OPT_unityMesh.RecalculateNormals();
				}

				if (new_boneWeights != null) {
					OPT_unityMesh.boneWeights = new_boneWeights;
					OPT_unityMesh.bindposes = geo.bones.bindPoses;
				}
			}
			GameObject OPT_gao = (OPT_mr == null ? gao : new GameObject("[Optimized] " + name));
			if (OPT_gao != gao) {
				OPT_gao.transform.SetParent(gao.transform);
			} else {
				gao.name = "[Optimized] " + gao.name;
				gao.name += " - " + sdc.offset;
			}
			if (geo.bones != null) {
				OPT_mr = OPT_gao.AddComponent<SkinnedMeshRenderer>();
				OPT_s_mr = (SkinnedMeshRenderer)OPT_mr;
				OPT_s_mr.bones = geo.bones.bones;
				OPT_s_mr.rootBone = geo.bones.bones[0];
				OPT_s_mr.sharedMesh = CopyMesh(OPT_unityMesh);

				BoxCollider bc = OPT_gao.AddComponent<BoxCollider>();
				bc.center = OPT_s_mr.bounds.center;
				bc.size = OPT_s_mr.bounds.size;
			} else {
				MeshFilter mf = OPT_gao.AddComponent<MeshFilter>();
				mf.sharedMesh = OPT_unityMesh;
				OPT_mr = OPT_gao.AddComponent<MeshRenderer>();

				try {
					MeshCollider mc = OPT_gao.AddComponent<MeshCollider>();
					mc.isTrigger = false;
					//mc.cookingOptions = MeshColliderCookingOptions.None;
					//mc.sharedMesh = OPT_unityMesh;
				} catch (Exception) { }
			}
		}

        private void CreateUnityMesh() {
            /*if (mesh.bones != null) {
                for (int j = 0; j < mesh.bones.num_bones; j++) {
                    Transform b = mesh.bones.bones[j];
                    b.transform.SetParent(gao.transform);
                    mesh.bones.bindPoses[j] = mesh.bones.bones[j].worldToLocalMatrix * gao.transform.localToWorldMatrix;
                }
            }*/
            VisualMaterial.Hint materialHints = geo.lookAtMode != 0 ? VisualMaterial.Hint.Billboard : VisualMaterial.Hint.None;
            //VisualMaterial.Hint materialHints = VisualMaterial.Hint.None;
            uint num_textures = 0;
            if (visualMaterial != null) {
                num_textures = visualMaterial.num_textures;
			}
			uint triangle_size = 3 * (uint)(backfaceCulling ? 1 : 2);

			if (sdc != null) {
				CreateUnityMeshFromSDC();
			}


			// Create mesh from unoptimized data
			if ((sdc == null) && num_triangles > 0) {
				Vector3[] new_vertices = new Vector3[num_triangles * 3];
				Vector3[] new_normals = new Vector3[num_triangles * 3];
				Vector3[][] new_uvs = new Vector3[num_textures][];
				BoneWeight[] new_boneWeights = geo.bones != null ? new BoneWeight[num_triangles * 3] : null;
				for (int um = 0; um < num_textures; um++) {
					new_uvs[um] = new Vector3[num_triangles * 3];
				}
				int[] unityTriangles = new int[num_triangles * triangle_size];
				for (int um = 0; um < num_textures; um++) {
					for (int j = 0; j < num_triangles * 3; j++) {
						uint uvMap = (uint)visualMaterial.textures[um].uvFunction % num_uvMaps;
						if (uvs != null) {
							new_uvs[um][j] = uvs[mapping_uvs[uvMap][j] % uvs.Length]; // modulo because R2 iOS has some corrupt uvs
							if (MapLoader.Loader.blockyMode && normals != null) new_uvs[um][j] = uvs[mapping_uvs[uvMap][j - (j % 3)]];
						}
						new_uvs[um][j].z = 1;
						/*int i0 = reader.ReadInt16(), m0 = (j * 3) + 0; // Old index, mapped index
                        int i1 = reader.ReadInt16(), m1 = (j * 3) + 1;
                        int i2 = reader.ReadInt16(), m2 = (j * 3) + 2;
                        new_uvs_spe[um][m0] = uvs[i0];
                        new_uvs_spe[um][m1] = uvs[i1];
                        new_uvs_spe[um][m2] = uvs[i2];*/
					}
				}
				//print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
				uint triangles_index = 0;
				for (int j = 0; j < num_triangles; j++, triangles_index += triangle_size) {
					int i0 = triangles[(j * 3) + 0], m0 = (j * 3) + 0; // Old index, mapped index
					int i1 = triangles[(j * 3) + 1], m1 = (j * 3) + 1;
					int i2 = triangles[(j * 3) + 2], m2 = (j * 3) + 2;
					if (i1 > geo.vertices.Length) MapLoader.Loader.print(geo.vertices.Length);
					new_vertices[m0] = geo.vertices[i0];
					new_vertices[m1] = geo.vertices[i1];
					new_vertices[m2] = geo.vertices[i2];

					if (geo.normals != null) {
						new_normals[m0] = geo.normals[i0];
						new_normals[m1] = geo.normals[i1];
						new_normals[m2] = geo.normals[i2];
					}
					if (MapLoader.Loader.blockyMode && normals != null) {
						new_normals[m0] = normals[j];
						new_normals[m1] = normals[j];
						new_normals[m2] = normals[j];
					}
					if (new_boneWeights != null) {
						new_boneWeights[m0] = geo.bones.weights[i0];
						new_boneWeights[m1] = geo.bones.weights[i1];
						new_boneWeights[m2] = geo.bones.weights[i2];
					}
					if (geo.blendWeights != null) {
						for (int um = 0; um < num_textures; um++) {
							if (geo.blendWeights[visualMaterial.textures[um].blendIndex] != null) {
								if (um == 0) materialHints |= VisualMaterial.Hint.Transparent;
								new_uvs[um][m0].z = geo.blendWeights[visualMaterial.textures[um].blendIndex][i0];
								new_uvs[um][m1].z = geo.blendWeights[visualMaterial.textures[um].blendIndex][i1];
								new_uvs[um][m2].z = geo.blendWeights[visualMaterial.textures[um].blendIndex][i2];
							}
						}
					}
					unityTriangles[triangles_index + 0] = m0;
					unityTriangles[triangles_index + 1] = m2;
					unityTriangles[triangles_index + 2] = m1;
					if (!backfaceCulling) {
						unityTriangles[triangles_index + 3] = unityTriangles[triangles_index + 0];
						unityTriangles[triangles_index + 4] = unityTriangles[triangles_index + 2];
						unityTriangles[triangles_index + 5] = unityTriangles[triangles_index + 1];
					}
				}
				if (unityMesh == null) {
					unityMesh = new Mesh();
					unityMesh.vertices = new_vertices;
					unityMesh.normals = new_normals;
					unityMesh.triangles = unityTriangles;
					if (new_boneWeights != null) {
						unityMesh.boneWeights = new_boneWeights;
						unityMesh.bindposes = geo.bones.bindPoses;
					}
					for (int i = 0; i < num_textures; i++) {
						unityMesh.SetUVs(i, new_uvs[i].ToList());
					}
				} else {
					unityMesh = CopyMesh(unityMesh);
				}
				//mesh.SetUVs(0, new_uvs_spe.ToList());
				/*mesh.uv = new_uvs_spe;*/
				if (new_boneWeights != null) {
					mr = gao.AddComponent<SkinnedMeshRenderer>();
					s_mr = (SkinnedMeshRenderer)mr;
					s_mr.bones = geo.bones.bones;
					s_mr.rootBone = geo.bones.bones[0];
					s_mr.sharedMesh = CopyMesh(unityMesh);

					BoxCollider bc = gao.AddComponent<BoxCollider>();
					bc.center = s_mr.bounds.center;
					bc.size = s_mr.bounds.size;
				} else {
					MeshFilter mf = gao.AddComponent<MeshFilter>();
					mr = gao.AddComponent<MeshRenderer>();
					mf.sharedMesh = unityMesh;
					try {
						MeshCollider mc = gao.AddComponent<MeshCollider>();
						mc.isTrigger = false;
						//mc.cookingOptions = MeshColliderCookingOptions.None;
						//mc.sharedMesh = unityMesh;
					} catch (Exception) { }
				}
				//}
			}


			// Create mesh from optimized data
			long OPT_num_triangles_total = ((OPT_num_triangleStrip > 2 ? OPT_num_triangleStrip - 2 : 0) + OPT_num_disconnectedTriangles) * (backfaceCulling ? 1 : 2);
            if (sdc == null && OPT_num_triangles_total > 0 && num_triangles <= 0) {
				uint triangles_index = 0;
				Vector3[] new_vertices = new Vector3[OPT_num_mapping_entries];
                Vector3[] new_normals = new Vector3[OPT_num_mapping_entries];
                Vector4[][] new_uvs = new Vector4[num_textures + (vertexColors != null ? 1 : 0)][]; // Thanks to Unity we can only store the blend weights as a third component of the UVs
                BoneWeight[] new_boneWeights = geo.bones != null ? new BoneWeight[OPT_num_mapping_entries] : null;
                for (int um = 0; um < num_textures; um++) {
                    new_uvs[um] = new Vector4[OPT_num_mapping_entries];
                }
				if (vertexColors != null) {
					new_uvs[num_textures] = new Vector4[OPT_num_mapping_entries];
					for (int i = 0; i < OPT_num_mapping_entries; i++) {
						new_uvs[num_textures][i] = new Vector4(vertexColors[i].r, vertexColors[i].g, vertexColors[i].b, vertexColors[i].a);
					}
				}
                for (int j = 0; j < OPT_num_mapping_entries; j++) {
                    new_vertices[j] = geo.vertices[OPT_mapping_vertices[j]];
                    if(geo.normals != null) new_normals[j] = geo.normals[OPT_mapping_vertices[j]];
                    if (new_boneWeights != null) new_boneWeights[j] = geo.bones.weights[OPT_mapping_vertices[j]];
                    for (int um = 0; um < num_textures; um++) {
                        uint uvMap = (uint)visualMaterial.textures[um].uvFunction % num_uvMaps;
                        //MapLoader.Loader.print(visualMaterial.textures[um].uvFunction + " - " + num_uvMaps);
                        new_uvs[um][j] = uvs[OPT_mapping_uvs[uvMap][j]];
                        if (geo.blendWeights != null && geo.blendWeights[visualMaterial.textures[um].blendIndex] != null) {
                            if (um == 0) materialHints |= VisualMaterial.Hint.Transparent;
                            new_uvs[um][j].z = geo.blendWeights[visualMaterial.textures[um].blendIndex][OPT_mapping_vertices[j]];
                        } else {
                            new_uvs[um][j].z = 1;
                        }
                    }
                }
                int[] unityTriangles = new int[OPT_num_triangles_total * triangle_size];
                if (OPT_num_triangleStrip > 2) {
                    for (int j = 0; j < OPT_num_triangleStrip - 2; j++, triangles_index += triangle_size) {
                        if (j % 2 == 0) {
                            unityTriangles[triangles_index + 0] = OPT_triangleStrip[j + 0];
                            unityTriangles[triangles_index + 1] = OPT_triangleStrip[j + 2];
                            unityTriangles[triangles_index + 2] = OPT_triangleStrip[j + 1];
                        } else {
                            unityTriangles[triangles_index + 0] = OPT_triangleStrip[j + 0];
                            unityTriangles[triangles_index + 1] = OPT_triangleStrip[j + 1];
                            unityTriangles[triangles_index + 2] = OPT_triangleStrip[j + 2];
                        }
                        if (!backfaceCulling) {
                            unityTriangles[triangles_index + 3] = unityTriangles[triangles_index + 0];
                            unityTriangles[triangles_index + 4] = unityTriangles[triangles_index + 2];
                            unityTriangles[triangles_index + 5] = unityTriangles[triangles_index + 1];
                        }
                    }
                }
                if (OPT_num_triangleStrip < 2) OPT_num_triangleStrip = 0;
                if (OPT_num_disconnectedTriangles > 0) {
                    //print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
                    for (int j = 0; j < OPT_num_disconnectedTriangles; j++, triangles_index += triangle_size) {
                        unityTriangles[triangles_index + 0] = OPT_disconnectedTriangles[(j * 3) + 0];
                        unityTriangles[triangles_index + 2] = OPT_disconnectedTriangles[(j * 3) + 1];
                        unityTriangles[triangles_index + 1] = OPT_disconnectedTriangles[(j * 3) + 2];
                        if (!backfaceCulling) {
                            unityTriangles[triangles_index + 3] = unityTriangles[triangles_index + 0];
                            unityTriangles[triangles_index + 4] = unityTriangles[triangles_index + 2];
                            unityTriangles[triangles_index + 5] = unityTriangles[triangles_index + 1];
                        }
                    }
                }
				if (OPT_unityMesh == null) {
					OPT_unityMesh = new Mesh();
					OPT_unityMesh.vertices = new_vertices;
					if (geo.normals != null) OPT_unityMesh.normals = new_normals;
					OPT_unityMesh.triangles = unityTriangles;
					if (new_boneWeights != null) {
						OPT_unityMesh.boneWeights = new_boneWeights;
						OPT_unityMesh.bindposes = geo.bones.bindPoses;
					}
					for (int i = 0; i < new_uvs.Length; i++) {
						OPT_unityMesh.SetUVs(i, new_uvs[i].ToList());
					}
				} else {
					OPT_unityMesh = CopyMesh(OPT_unityMesh);
				}
				GameObject OPT_gao = (OPT_mr == null ? gao : new GameObject("[Optimized] " + name));
				if (OPT_gao != gao) {
					OPT_gao.transform.SetParent(gao.transform);
				} else {
					gao.name = "[Optimized] " + gao.name;
				}
				if (new_boneWeights != null) {
                    OPT_mr = OPT_gao.AddComponent<SkinnedMeshRenderer>();
                    OPT_s_mr = (SkinnedMeshRenderer)OPT_mr;
                    OPT_s_mr.bones = geo.bones.bones;
                    OPT_s_mr.rootBone = geo.bones.bones[0];
                    OPT_s_mr.sharedMesh = CopyMesh(OPT_unityMesh);
					
					BoxCollider bc = OPT_gao.AddComponent<BoxCollider>();
					bc.center = OPT_s_mr.bounds.center;
					bc.size = OPT_s_mr.bounds.size;
				} else {
                    MeshFilter mf = OPT_gao.AddComponent<MeshFilter>();
                    mf.sharedMesh = OPT_unityMesh;
                    OPT_mr = OPT_gao.AddComponent<MeshRenderer>();

					try {
						MeshCollider mc = OPT_gao.AddComponent<MeshCollider>();
						mc.isTrigger = false;
						//mc.cookingOptions = MeshColliderCookingOptions.None;
						//mc.sharedMesh = OPT_unityMesh;
					} catch (Exception) { }
				}
            }
            if (visualMaterial != null) {
                //gao.name += " " + visualMaterial.offset + " - " + (visualMaterial.textures.Count > 0 ? visualMaterial.textures[0].offset.ToString() : "NULL" );
                Material unityMat = visualMaterial.GetMaterial(materialHints);
				if (((sdc != null && sdc.geo.Type != 6) || vertexColors != null) && unityMat != null) unityMat.SetVector("_Tex2Params", new Vector4(60, 0, 0, 0));
                bool receiveShadows = (visualMaterial.properties & VisualMaterial.property_receiveShadows) != 0;
                bool scroll = visualMaterial.ScrollingEnabled;
                /*if (num_uvMaps > 1) {
                    unityMat.SetFloat("_UVSec", 1f);
                } else if (scroll) {
                    for (int i = num_uvMaps; i < visualMaterial.textures.Count; i++) {
                        if (visualMaterial.textures[i].ScrollingEnabled) {
                            unityMat.SetFloat("_UVSec", 1f);
                            break;
                        }
                    }
                }*/
                //if (r3mat.Material.GetColor("_EmissionColor") != Color.black) print("Mesh with emission: " + name);
                if (OPT_mr != null) {
                    OPT_mr.sharedMaterial = unityMat;
                    //mr_main.UpdateGIMaterials();
                    if (!receiveShadows) OPT_mr.receiveShadows = false;
                    if (visualMaterial.animTextures.Count > 0) {
                        MultiTextureMaterial mtmat = OPT_mr.gameObject.AddComponent<MultiTextureMaterial>();
                        mtmat.visMat = visualMaterial;
                        mtmat.mat = OPT_mr.material;
                    }
                    /*if (scroll) {
                        ScrollingTexture scrollComponent = mr_main.gameObject.AddComponent<ScrollingTexture>();
                        scrollComponent.visMat = visualMaterial;
                        scrollComponent.mat = mr_main.material;
                    }*/
                }
                if (mr != null) {
                    mr.sharedMaterial = unityMat;
                    //mr_spe.UpdateGIMaterials();
                    if (!receiveShadows) mr.receiveShadows = false;
                    if (visualMaterial.animTextures.Count > 0) {
                        MultiTextureMaterial mtmat = mr.gameObject.AddComponent<MultiTextureMaterial>();
                        mtmat.visMat = visualMaterial;
                        mtmat.mat = mr.material;
					}
                    /*if (scroll) {
                        ScrollingTexture scrollComponent = mr_spe.gameObject.AddComponent<ScrollingTexture>();
                        scrollComponent.visMat = visualMaterial;
                        scrollComponent.mat = mr_spe.material;
                    }*/
                }
            }
        }
		public void MorphVertices(GeometricObjectElementTriangles el, float lerp) {
			// Use UpdateMeshVertices if possible! Only use this for special cases
			if (OPT_unityMesh != null) {
				Vector3[] new_vertices = OPT_unityMesh.vertices;
				for (int j = 0; j < OPT_num_mapping_entries; j++) {
					Vector3 from = geo.vertices[OPT_mapping_vertices[j]];
					Vector3 to = el.geo.vertices[el.OPT_mapping_vertices[j]];
					new_vertices[j] = Vector3.LerpUnclamped(from, to, lerp);
				}
				OPT_unityMesh.vertices = new_vertices;
			}
			if (unityMesh != null) {
				Vector3[] new_vertices = unityMesh.vertices;
				for (int j = 0; j < num_triangles; j++) {
					int i0 = triangles[(j * 3) + 0], o0 = el.triangles[(j * 3) + 0], m0 = (j * 3) + 0; // Old index, mapped index
					int i1 = triangles[(j * 3) + 1], o1 = el.triangles[(j * 3) + 1], m1 = (j * 3) + 1;
					int i2 = triangles[(j * 3) + 2], o2 = el.triangles[(j * 3) + 2], m2 = (j * 3) + 2;
					new_vertices[m0] = Vector3.LerpUnclamped(geo.vertices[i0], el.geo.vertices[o0], lerp);
					new_vertices[m1] = Vector3.LerpUnclamped(geo.vertices[i1], el.geo.vertices[o1], lerp);
					new_vertices[m2] = Vector3.LerpUnclamped(geo.vertices[i2], el.geo.vertices[o2], lerp);
				}
				unityMesh.vertices = new_vertices;
			}
		}

		public void UpdateMeshVertices(Vector3[] vertices) {
			if (OPT_unityMesh != null) {
				Vector3[] new_vertices = OPT_unityMesh.vertices;
				if (sdc != null && sdc_mapping != null) {
					// R3 PS2
					ushort mapping_i = 0;
					for (int i = 0; i < geo.vertices.Length; i++) {
						ushort length = sdc_mapping[mapping_i];
						mapping_i++;
						if (mapping_i >= sdc_mapping.Length) break;
						for (int j = 0; j < length; j++) {
							ushort vertIndexInSdc = (ushort)(sdc_mapping[mapping_i] & 0x7FFF);
							if (vertIndexInSdc < vertices.Length) {
								new_vertices[vertIndexInSdc] = vertices[i];
							}
							mapping_i++;
							if (mapping_i >= sdc_mapping.Length) break;
						}
					}
				} else {
					// Other games / platforms
					for (int j = 0; j < OPT_num_mapping_entries; j++) {
						new_vertices[j] = vertices[OPT_mapping_vertices[j]];
					}
				}
				OPT_unityMesh.vertices = new_vertices;
			}
			if (unityMesh != null) {
				Vector3[] new_vertices = unityMesh.vertices;
				for (int j = 0; j < num_triangles; j++) {
					int i0 = triangles[(j * 3) + 0], m0 = (j * 3) + 0; // Old index, mapped index
					int i1 = triangles[(j * 3) + 1], m1 = (j * 3) + 1;
					int i2 = triangles[(j * 3) + 2], m2 = (j * 3) + 2;
					new_vertices[m0] = vertices[i0];
					new_vertices[m1] = vertices[i1];
					new_vertices[m2] = vertices[i2];
				}
				unityMesh.vertices = new_vertices;
			}
		}
		public void ResetVertices() {
			UpdateMeshVertices(geo.vertices);
		}

        public static GeometricObjectElementTriangles Read(Reader reader, Pointer offset, GeometricObject geo) {
            MapLoader l = MapLoader.Loader;
            GeometricObjectElementTriangles sm = new GeometricObjectElementTriangles(offset, geo);
            sm.name = "Submesh @ pos " + offset;
			//l.print(sm.name);
            sm.backfaceCulling = !l.forceDisplayBackfaces;
            sm.off_material = Pointer.Read(reader);
			if (Settings.s.game == Settings.Game.LargoWinch) {
				//sm.visualMaterial = VisualMaterial.FromOffset(sm.off_material);
				sm.visualMaterial = VisualMaterial.FromOffsetOrRead(sm.off_material, reader);
			} else if (Settings.s.engineVersion == Settings.EngineVersion.R3 || Settings.s.game == Settings.Game.R2Revolution) {
                sm.visualMaterial = VisualMaterial.FromOffset(sm.off_material);
            } else {
                sm.gameMaterial = GameMaterial.FromOffsetOrRead(sm.off_material, reader);
                sm.visualMaterial = sm.gameMaterial.visualMaterial;
            }
			sm.visualMaterialOG = sm.visualMaterial;
            /*if (sm.visualMaterial != null && sm.visualMaterial.textures.Count > 0 && sm.visualMaterial.textures[0].off_texture != null) {
                sm.name += " - VisMatTex:" + sm.visualMaterial.textures[0].offset + " - TexInfo:" + sm.visualMaterial.textures[0].off_texture;
            }*/
            if (sm.visualMaterial != null) {
                sm.backfaceCulling = ((sm.visualMaterial.flags & VisualMaterial.flags_backfaceCulling) != 0) && !l.forceDisplayBackfaces;
            }
            sm.num_triangles = reader.ReadUInt16();
			if (Settings.s.game == Settings.Game.R2Revolution) {
				sm.lightmap_index = reader.ReadInt16();
				sm.off_triangles = Pointer.Read(reader);
			} else {
				sm.num_uvs = reader.ReadUInt16();
				if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
					sm.num_uvMaps = reader.ReadUInt16();
					sm.lightmap_index = reader.ReadInt16();
				}
				sm.off_triangles = Pointer.Read(reader); // 1 entry = 3 shorts. Max: num_vertices
				if (Settings.s.mode == Settings.Mode.Rayman3GC) reader.ReadUInt32();
				sm.off_mapping_uvs = Pointer.Read(reader); // 1 entry = 3 shorts. Max: num_weights
				sm.off_normals = Pointer.Read(reader); // 1 entry = 3 floats
				sm.off_uvs = Pointer.Read(reader); // 1 entry = 2 floats
				if (Settings.s.game == Settings.Game.LargoWinch) {
					sm.off_mapping_lightmap = Pointer.Read(reader);
					sm.num_mapping_lightmap = reader.ReadUInt16();
					reader.ReadUInt16();
				} else if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
					if (Settings.s.platform != Settings.Platform.PS2) {
						reader.ReadUInt32();
						reader.ReadUInt32();
					} else {
						// PS2
						reader.ReadUInt32();
						sm.off_sdc_mapping = Pointer.Read(reader);
						Pointer.DoAt(ref reader, sm.off_sdc_mapping, () => {
							uint length = reader.ReadUInt32();
							sm.sdc_mapping = new ushort[length];
							for (int i = 0; i < length; i++) {
								sm.sdc_mapping[i] = reader.ReadUInt16();
							}
						});
					}
				} else if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
					reader.ReadUInt32();
				}
				if (Settings.s.game != Settings.Game.TTSE) {
					sm.off_vertex_indices = Pointer.Read(reader);
					sm.num_vertex_indices = reader.ReadUInt16();
					sm.parallelBox = reader.ReadUInt16();
					reader.ReadUInt32();
				}
			}
            if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
				if (Settings.s.game != Settings.Game.Dinosaur
					&& Settings.s.game != Settings.Game.LargoWinch
					&& Settings.s.mode != Settings.Mode.RaymanArenaGCDemo
					&& Settings.s.platform != Settings.Platform.PS2) {
					sm.isVisibleInPortal = reader.ReadByte();
					reader.ReadByte();
					sm.OPT_num_mapping_entries = reader.ReadUInt16(); // num_shorts
					sm.OPT_off_mapping_vertices = Pointer.Read(reader); // shorts_offset1 (1st array of size num_shorts, max_num_vertices)
					sm.OPT_off_mapping_uvs = Pointer.Read(reader); // shorts_offset2 (2nd array of size num_shorts, max: num_weights)
					sm.OPT_num_triangleStrip = reader.ReadUInt16(); // num_shorts2
					sm.OPT_num_disconnectedTriangles = reader.ReadUInt16();
					sm.OPT_off_triangleStrip = Pointer.Read(reader); // shorts2_offset (array of size num_shorts2)
					sm.OPT_off_disconnectedTriangles = Pointer.Read(reader);
					if (Settings.s.hasNames) sm.name += reader.ReadString(0x34);
				} else if(Settings.s.platform == Settings.Platform.PS2) {
					reader.ReadUInt32();
					sm.isVisibleInPortal = reader.ReadByte();
					reader.ReadByte();
					reader.ReadUInt16();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					sm.OPT_num_mapping_entries = 0;
					sm.OPT_off_mapping_vertices = null;
					sm.OPT_off_mapping_uvs = null;
					sm.OPT_num_triangleStrip = 0;
					sm.OPT_num_disconnectedTriangles = 0;
					sm.OPT_off_triangleStrip = null;
					sm.OPT_off_disconnectedTriangles = null;
				} else {
					sm.OPT_num_mapping_entries = 0;
					sm.OPT_off_mapping_vertices = null;
					sm.OPT_off_mapping_uvs = null;
					sm.OPT_num_triangleStrip = 0;
					sm.OPT_num_disconnectedTriangles = 0;
					sm.OPT_off_triangleStrip = null;
					sm.OPT_off_disconnectedTriangles = null;
					sm.isVisibleInPortal = 1;
					if (Settings.s.mode == Settings.Mode.RaymanArenaGCDemo) {
						sm.isVisibleInPortal = reader.ReadByte();
						reader.ReadByte();
						sm.OPT_num_mapping_entries = reader.ReadUInt16(); // num_shorts
					}
				}
            } else {
                // Defaults for Rayman 2, no optimized mesh feature
                sm.num_uvMaps = 1;
                sm.OPT_num_mapping_entries = 0;
                sm.OPT_off_mapping_vertices = null;
                sm.OPT_off_mapping_uvs = null;
                sm.OPT_num_triangleStrip = 0;
                sm.OPT_num_disconnectedTriangles = 0;
                sm.OPT_off_triangleStrip = null;
                sm.OPT_off_disconnectedTriangles = null;
				sm.isVisibleInPortal = 1;
            }

            // Read mapping tables
            sm.OPT_mapping_uvs = new int[sm.num_uvMaps][];
            if (sm.OPT_num_mapping_entries > 0) {
				Pointer.DoAt(ref reader, sm.OPT_off_mapping_vertices, () => {
					//print("Mapping offset: " + String.Format("0x{0:X}", fs.Position));
					sm.OPT_mapping_vertices = new int[sm.OPT_num_mapping_entries];
					for (int j = 0; j < sm.OPT_num_mapping_entries; j++) {
						sm.OPT_mapping_vertices[j] = reader.ReadInt16();
					}
				});
				Pointer.DoAt(ref reader, sm.OPT_off_mapping_uvs, () => {
					for (int j = 0; j < sm.num_uvMaps; j++) {
						sm.OPT_mapping_uvs[j] = new int[sm.OPT_num_mapping_entries];
					}
					for (int j = 0; j < sm.OPT_num_mapping_entries; j++) {
						for (int um = 0; um < sm.num_uvMaps; um++) {
							sm.OPT_mapping_uvs[um][j] = reader.ReadInt16();
						}
					}
				});
            }
            if (sm.num_triangles > 0) {
				Pointer.DoAt(ref reader, sm.off_mapping_uvs, () => {
					sm.mapping_uvs = new int[sm.num_uvMaps][];
					for (int j = 0; j < sm.num_uvMaps; j++) {
						sm.mapping_uvs[j] = new int[sm.num_triangles * 3];
					}
					// Why is uv maps here the outer loop instead of inner like the other thing?
					for (int um = 0; um < sm.num_uvMaps; um++) {
						for (int j = 0; j < sm.num_triangles * 3; j++) {
							sm.mapping_uvs[um][j] = reader.ReadInt16();
						}
					}
				});
            }

			// Read UVs
			Pointer.DoAt(ref reader, sm.off_uvs, () => {
				sm.uvs = new Vector2[sm.num_uvs];
				for (int j = 0; j < sm.num_uvs; j++) {
					sm.uvs[j] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
				}
			});
			// Read triangle data
			Pointer.DoAt(ref reader, sm.OPT_off_triangleStrip, () => {
				sm.OPT_triangleStrip = new int[sm.OPT_num_triangleStrip];
				for (int j = 0; j < sm.OPT_num_triangleStrip; j++) {
					sm.OPT_triangleStrip[j] = reader.ReadInt16();
				}
			});
			Pointer.DoAt(ref reader, sm.OPT_off_disconnectedTriangles, () => {
				sm.OPT_disconnectedTriangles = new int[sm.OPT_num_disconnectedTriangles * 3];
				//print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
				for (int j = 0; j < sm.OPT_num_disconnectedTriangles; j++) {
					sm.OPT_disconnectedTriangles[(j * 3) + 0] = reader.ReadInt16();
					sm.OPT_disconnectedTriangles[(j * 3) + 1] = reader.ReadInt16();
					sm.OPT_disconnectedTriangles[(j * 3) + 2] = reader.ReadInt16();
				}
			});
            if (sm.num_triangles > 0) {
				Pointer.DoAt(ref reader, sm.off_triangles, () => {
					sm.triangles = new int[sm.num_triangles * 3];
					//print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
					for (int j = 0; j < sm.num_triangles; j++) {
						sm.triangles[(j * 3) + 0] = reader.ReadInt16();
						sm.triangles[(j * 3) + 1] = reader.ReadInt16();
						sm.triangles[(j * 3) + 2] = reader.ReadInt16();
					}
				});
                if (sm.off_normals != null) {
					Pointer.DoAt(ref reader, sm.off_normals, () => {
						sm.normals = new Vector3[sm.num_triangles];
						for (int j = 0; j < sm.num_triangles; j++) {
							float x = reader.ReadSingle();
							float z = reader.ReadSingle();
							float y = reader.ReadSingle();
							sm.normals[j] = new Vector3(x, y, z);
						}
					});
                }
            }
			if (Settings.s.game == Settings.Game.LargoWinch && sm.lightmap_index != -1) {
				LWLoader lwl = MapLoader.Loader as LWLoader;
				if (lwl.lms != null && sm.lightmap_index >= 0 && sm.lightmap_index < lwl.lms.Count) {
					/*if (sm.lightmap_index < l.off_lightmapUV.Length - 1) {
						int amount = ((int)l.off_lightmapUV[sm.lightmap_index + 1].offset - (int)l.off_lightmapUV[sm.lightmap_index].offset);
						amount = amount / 8;
						l.print(offset + " - UVs: " + amount + " - " + sm.mesh.num_vertices + " - " + sm.num_mapping_entries + " - " + sm.num_uvs + " - " + sm.num_disconnected_triangles_spe + " - " + sm.num_mapping_lightmap);
					}*/
					Vector2[] lightmapUVs = new Vector2[sm.num_mapping_lightmap];
					Pointer.DoAt(ref reader, sm.off_mapping_lightmap, () => {
						sm.mapping_lightmap = new int[sm.num_mapping_lightmap];
						for (int i = 0; i < sm.num_mapping_lightmap; i++) {
							sm.mapping_lightmap[i] = reader.ReadInt16();
						}
					});
					Pointer.DoAt(ref reader, l.off_lightmapUV[sm.lightmap_index], () => {
						for (int j = 0; j < lightmapUVs.Length; j++) {
							lightmapUVs[j] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
						}
					});
					sm.AddLightmap(lwl.GetLightmap(sm.lightmap_index), lightmapUVs);
				}
			}
            return sm;
        }

		public void AddLightmap(Texture2D lightmap, Vector2[] lightmapUVs) {
			this.lightmap = lightmap;
			this.lightmapUVs = lightmapUVs;


			// Bad hack
			Array.Resize(ref uvs, num_uvs + lightmapUVs.Length);
			if (OPT_mapping_uvs != null) {
				Array.Resize(ref OPT_mapping_uvs, num_uvMaps + 1);
				OPT_mapping_uvs[OPT_mapping_uvs.Length - 1] = Enumerable.Range(num_uvs, lightmapUVs.Length).ToArray();
			}
			if (Settings.s.game == Settings.Game.LargoWinch) {
				if (mapping_uvs != null && mapping_lightmap != null) {
					Array.Resize(ref mapping_uvs, num_uvMaps + 1);
					mapping_uvs[mapping_uvs.Length - 1] = new int[num_triangles * 3];
					for (int i = 0; i < num_triangles * 3; i++) {
						int search = triangles[i];
						int lightmapUV = Array.IndexOf(mapping_lightmap, search);
						if (lightmapUV != -1) {
							mapping_uvs[mapping_uvs.Length - 1][i] = num_uvs + lightmapUV;
						} else {
							MapLoader.Loader.print("not found");
							mapping_uvs[mapping_uvs.Length - 1][i] = mapping_uvs[mapping_uvs.Length - 2][i];
						}
						//mapping_uvs_spe[mapping_uvs_spe.Length - 1][i] = num_uvs + disconnected_triangles_spe[i];
					}

					//Enumerable.Range(num_uvs, lightmapUVs.Length).ToArray();
				}
			}
			Array.Copy(lightmapUVs, 0, uvs, num_uvs, lightmapUVs.Length);
			/*for (int j = 0; j < lightmapUVs.Length; j++) {
				uvs[num_uvs + j] = lightmapuv;
			}*/
			num_uvs += (ushort)lightmapUVs.Length;
			num_uvMaps++;

			if (visualMaterial != null) {
				visualMaterial = visualMaterial.Clone();
				visualMaterial.num_textures += 1;
				visualMaterial.textures.Add(new VisualMaterialTexture() {
					texture = new TextureInfo(null) {
						width = (ushort)lightmap.width,
						height = (ushort)lightmap.height,
						Texture = lightmap
					},
					textureOp = 50,
					uvFunction = 1
				});
			}
		}

        public void ReinitBindPoses() {
            if (OPT_s_mr != null) {
                Mesh newmesh = CopyMesh(OPT_unityMesh);
                newmesh.bindposes = geo.bones.bindPoses;
                OPT_s_mr.sharedMesh = newmesh;
            }
            if (s_mr != null) {
                Mesh newmesh = CopyMesh(unityMesh);
                newmesh.bindposes = geo.bones.bindPoses;
                s_mr.sharedMesh = newmesh;
            }
        }

        // Call after clone
        public void Reset() {
            gao = null;
            OPT_s_mr = null;
            s_mr = null;
			OPT_mr = null;
			mr = null;
            OPT_unityMesh = null;
            unityMesh = null;
			if (geo.bones != null) {
				OPT_unityMesh = null;
				unityMesh = null;
			}
        }

        public IGeometricObjectElement Clone(GeometricObject geo) {
            GeometricObjectElementTriangles sm = (GeometricObjectElementTriangles)MemberwiseClone();
            sm.geo = geo;
            sm.Reset();
            return sm;
        }

        private Mesh CopyMesh(Mesh mesh) {
            uint num_textures = visualMaterial != null ? visualMaterial.num_textures : 0;
            Mesh newmesh = new Mesh();
            newmesh.vertices = mesh.vertices;
            newmesh.triangles = mesh.triangles;
            for (int i = 0; i < num_textures; i++) {
                List<Vector3> uvsTemp = new List<Vector3>();
                mesh.GetUVs(i, uvsTemp);
                newmesh.SetUVs(i, uvsTemp);
            }
            newmesh.normals = mesh.normals;
            newmesh.colors = mesh.colors;
            newmesh.tangents = mesh.tangents;
            newmesh.boneWeights = mesh.boneWeights;
            newmesh.bindposes = mesh.bindposes;
            return newmesh;
        }
    }
}
