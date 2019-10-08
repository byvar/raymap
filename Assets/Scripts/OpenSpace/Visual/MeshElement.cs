using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Basically a submesh
    /// </summary>
    public class MeshElement : IGeometricElement {
        [JsonIgnore] public MeshObject mesh;
        public Pointer offset;

        [JsonIgnore]
        public string name;
        public Pointer off_material;
        public GameMaterial gameMaterial;
        public VisualMaterial visualMaterial;
        public bool backfaceCulling;
        public ushort num_disconnected_triangles_spe;
        public ushort num_uvs;
        public ushort num_uvMaps;
        public Pointer off_disconnected_triangles_spe;
        public Pointer off_mapping_uvs_spe;
        public Pointer off_weights_spe;
        public Pointer off_uvs;
        public Pointer off_vertex_indices;
        public ushort num_vertex_indices;
        public ushort num_mapping_entries;
        public Pointer off_mapping_vertices;
        public Pointer off_mapping_uvs;
        public ushort num_connected_vertices;
        public ushort num_disconnected_triangles;
        public Pointer off_connected_vertices;
        public Pointer off_disconnected_triangles;
        public int[] mapping_vertices = null;
        public int[][] mapping_uvs = null;
        public Vector2[] uvs = null;
        public int[] connected_vertices = null;
        public int[] disconnected_triangles = null;
        public int[][] mapping_uvs_spe = null;
        public int[] disconnected_triangles_spe = null;
        public Vector3[] normals_spe = null;

		// Revolution
		public Color[] vertexColors = null;
		public int lightmap_index = -1;

        private SkinnedMeshRenderer s_mr_main = null;
        private SkinnedMeshRenderer s_mr_spe = null;
        private Renderer mr_main = null;
        private Renderer mr_spe = null;
        private Mesh mesh_main = null;
        private Mesh mesh_spe = null;


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

        public MeshElement(Pointer offset, MeshObject mesh) {
            this.mesh = mesh;
            this.offset = offset;
        }

        private void CreateUnityMesh() {
            /*if (mesh.bones != null) {
                for (int j = 0; j < mesh.bones.num_bones; j++) {
                    Transform b = mesh.bones.bones[j];
                    b.transform.SetParent(gao.transform);
                    mesh.bones.bindPoses[j] = mesh.bones.bones[j].worldToLocalMatrix * gao.transform.localToWorldMatrix;
                }
            }*/
            VisualMaterial.Hint materialHints = mesh.lookAtMode != 0 ? VisualMaterial.Hint.Billboard : VisualMaterial.Hint.None;
            //VisualMaterial.Hint materialHints = VisualMaterial.Hint.None;
            uint num_textures = 0;
            if (visualMaterial != null) {
                num_textures = visualMaterial.num_textures;
            }
			
            long num_triangles_main = ((num_connected_vertices > 2 ? num_connected_vertices - 2 : 0) + num_disconnected_triangles) * (backfaceCulling ? 1 : 2);
            uint triangle_size = 3 * (uint)(backfaceCulling ? 1 : 2);
            uint triangles_index = 0;
            if (num_triangles_main > 0) {
                Vector3[] new_vertices = new Vector3[num_mapping_entries];
                Vector3[] new_normals = new Vector3[num_mapping_entries];
                Vector4[][] new_uvs = new Vector4[num_textures + (vertexColors != null ? 1 : 0)][]; // Thanks to Unity we can only store the blend weights as a third component of the UVs
                BoneWeight[] new_boneWeights = mesh.bones != null ? new BoneWeight[num_mapping_entries] : null;
                for (int um = 0; um < num_textures; um++) {
                    new_uvs[um] = new Vector4[num_mapping_entries];
                }
				if (vertexColors != null) {
					new_uvs[num_textures] = new Vector4[num_mapping_entries];
					for (int i = 0; i < num_mapping_entries; i++) {
						new_uvs[num_textures][i] = new Vector4(vertexColors[i].r, vertexColors[i].g, vertexColors[i].b, vertexColors[i].a);
					}
				}
                for (int j = 0; j < num_mapping_entries; j++) {
                    new_vertices[j] = mesh.vertices[mapping_vertices[j]];
                    if(mesh.normals != null) new_normals[j] = mesh.normals[mapping_vertices[j]];
                    if (new_boneWeights != null) new_boneWeights[j] = mesh.bones.weights[mapping_vertices[j]];
                    for (int um = 0; um < num_textures; um++) {
                        uint uvMap = (uint)visualMaterial.textures[um].uvFunction % num_uvMaps;
                        //MapLoader.Loader.print(visualMaterial.textures[um].uvFunction + " - " + num_uvMaps);
                        new_uvs[um][j] = uvs[mapping_uvs[uvMap][j]];
                        if (mesh.blendWeights != null && mesh.blendWeights[visualMaterial.textures[um].blendIndex] != null) {
                            if (um == 0) materialHints |= VisualMaterial.Hint.Transparent;
                            new_uvs[um][j].z = mesh.blendWeights[visualMaterial.textures[um].blendIndex][mapping_vertices[j]];
                        } else {
                            new_uvs[um][j].z = 1;
                        }
                    }
                }
                int[] triangles = new int[num_triangles_main * triangle_size];
                if (num_connected_vertices > 2) {
                    for (int j = 0; j < num_connected_vertices - 2; j++, triangles_index += triangle_size) {
                        if (j % 2 == 0) {
                            triangles[triangles_index + 0] = connected_vertices[j + 0];
                            triangles[triangles_index + 1] = connected_vertices[j + 2];
                            triangles[triangles_index + 2] = connected_vertices[j + 1];
                        } else {
                            triangles[triangles_index + 0] = connected_vertices[j + 0];
                            triangles[triangles_index + 1] = connected_vertices[j + 1];
                            triangles[triangles_index + 2] = connected_vertices[j + 2];
                        }
                        if (!backfaceCulling) {
                            triangles[triangles_index + 3] = triangles[triangles_index + 0];
                            triangles[triangles_index + 4] = triangles[triangles_index + 2];
                            triangles[triangles_index + 5] = triangles[triangles_index + 1];
                        }
                    }
                }
                if (num_connected_vertices < 2) num_connected_vertices = 0;
                if (num_disconnected_triangles > 0) {
                    //print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
                    for (int j = 0; j < num_disconnected_triangles; j++, triangles_index += triangle_size) {
                        triangles[triangles_index + 0] = disconnected_triangles[(j * 3) + 0];
                        triangles[triangles_index + 2] = disconnected_triangles[(j * 3) + 1];
                        triangles[triangles_index + 1] = disconnected_triangles[(j * 3) + 2];
                        if (!backfaceCulling) {
                            triangles[triangles_index + 3] = triangles[triangles_index + 0];
                            triangles[triangles_index + 4] = triangles[triangles_index + 2];
                            triangles[triangles_index + 5] = triangles[triangles_index + 1];
                        }
                    }
                }
				if (mesh_main == null) {
					mesh_main = new Mesh();
					mesh_main.vertices = new_vertices;
					if (mesh.normals != null) mesh_main.normals = new_normals;
					mesh_main.triangles = triangles;
					if (new_boneWeights != null) {
						mesh_main.boneWeights = new_boneWeights;
						mesh_main.bindposes = mesh.bones.bindPoses;
					}
					for (int i = 0; i < new_uvs.Length; i++) {
						mesh_main.SetUVs(i, new_uvs[i].ToList());
					}
				} else {
					mesh_main = CopyMesh(mesh_main);
				}
				if (new_boneWeights != null) {
                    mr_main = gao.AddComponent<SkinnedMeshRenderer>();
                    s_mr_main = (SkinnedMeshRenderer)mr_main;
                    s_mr_main.bones = mesh.bones.bones;
                    s_mr_main.rootBone = mesh.bones.bones[0];
                    s_mr_main.sharedMesh = CopyMesh(mesh_main);
					
					BoxCollider bc = gao.AddComponent<BoxCollider>();
					bc.center = s_mr_main.bounds.center;
					bc.size = s_mr_main.bounds.size;
				} else {
                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh_main;
                    mr_main = gao.AddComponent<MeshRenderer>();
					MeshCollider mc = gao.AddComponent<MeshCollider>();
					mc.isTrigger = false;
					mc.sharedMesh = mesh_main;
				}
            }
            if (num_disconnected_triangles_spe > 0) {
                Vector3[] new_vertices_spe = new Vector3[num_disconnected_triangles_spe * 3];
                Vector3[] new_normals_spe = new Vector3[num_disconnected_triangles_spe * 3];
                Vector3[][] new_uvs_spe = new Vector3[num_textures][];
                BoneWeight[] new_boneWeights_spe = mesh.bones != null ? new BoneWeight[num_disconnected_triangles_spe * 3] : null;
                for (int um = 0; um < num_textures; um++) {
                    new_uvs_spe[um] = new Vector3[num_disconnected_triangles_spe * 3];
                }
                int[] triangles_spe = new int[num_disconnected_triangles_spe * triangle_size];
                triangles_index = 0;
                for (int um = 0; um < num_textures; um++) {
                    for (int j = 0; j < num_disconnected_triangles_spe * 3; j++) {
                        uint uvMap = (uint)visualMaterial.textures[um].uvFunction % num_uvMaps;
						if (uvs != null) {
							new_uvs_spe[um][j] = uvs[mapping_uvs_spe[uvMap][j]];
							if (MapLoader.Loader.blockyMode && normals_spe != null) new_uvs_spe[um][j] = uvs[mapping_uvs_spe[uvMap][j - (j % 3)]];
						}
                        new_uvs_spe[um][j].z = 1;
                        /*int i0 = reader.ReadInt16(), m0 = (j * 3) + 0; // Old index, mapped index
                        int i1 = reader.ReadInt16(), m1 = (j * 3) + 1;
                        int i2 = reader.ReadInt16(), m2 = (j * 3) + 2;
                        new_uvs_spe[um][m0] = uvs[i0];
                        new_uvs_spe[um][m1] = uvs[i1];
                        new_uvs_spe[um][m2] = uvs[i2];*/
                    }
                }
                //print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
                for (int j = 0; j < num_disconnected_triangles_spe; j++, triangles_index += triangle_size) {
                    int i0 = disconnected_triangles_spe[(j * 3) + 0], m0 = (j * 3) + 0; // Old index, mapped index
                    int i1 = disconnected_triangles_spe[(j * 3) + 1], m1 = (j * 3) + 1;
                    int i2 = disconnected_triangles_spe[(j * 3) + 2], m2 = (j * 3) + 2;
					if (i1 > mesh.vertices.Length) MapLoader.Loader.print(mesh.vertices.Length);
                    new_vertices_spe[m0] = mesh.vertices[i0];
                    new_vertices_spe[m1] = mesh.vertices[i1];
                    new_vertices_spe[m2] = mesh.vertices[i2];

					if (mesh.normals != null) {
						new_normals_spe[m0] = mesh.normals[i0];
						new_normals_spe[m1] = mesh.normals[i1];
						new_normals_spe[m2] = mesh.normals[i2];
					}
                    if (MapLoader.Loader.blockyMode && normals_spe != null) {
                        new_normals_spe[m0] = normals_spe[j];
                        new_normals_spe[m1] = normals_spe[j];
                        new_normals_spe[m2] = normals_spe[j];
                    }
                    if (new_boneWeights_spe != null) {
                        new_boneWeights_spe[m0] = mesh.bones.weights[i0];
                        new_boneWeights_spe[m1] = mesh.bones.weights[i1];
                        new_boneWeights_spe[m2] = mesh.bones.weights[i2];
                    }
                    if (mesh.blendWeights != null) {
                        for (int um = 0; um < num_textures; um++) {
                            if (mesh.blendWeights[visualMaterial.textures[um].blendIndex] != null) {
                                if (um == 0) materialHints |= VisualMaterial.Hint.Transparent;
                                new_uvs_spe[um][m0].z = mesh.blendWeights[visualMaterial.textures[um].blendIndex][i0];
                                new_uvs_spe[um][m1].z = mesh.blendWeights[visualMaterial.textures[um].blendIndex][i1];
                                new_uvs_spe[um][m2].z = mesh.blendWeights[visualMaterial.textures[um].blendIndex][i2];
                            }
                        }
                    }
                    triangles_spe[triangles_index + 0] = m0;
                    triangles_spe[triangles_index + 1] = m2;
                    triangles_spe[triangles_index + 2] = m1;
                    if (!backfaceCulling) {
                        triangles_spe[triangles_index + 3] = triangles_spe[triangles_index + 0];
                        triangles_spe[triangles_index + 4] = triangles_spe[triangles_index + 2];
                        triangles_spe[triangles_index + 5] = triangles_spe[triangles_index + 1];
                    }
                }
                //if (mr_main == null) {
                GameObject gao_spe = (mr_main == null ? gao : new GameObject("[SPE] " + name));
                if (gao_spe != gao) {
                    gao_spe.transform.SetParent(gao.transform);
                } else {
                    gao.name = "[SPE] " + gao.name;
                }
				if (mesh_spe == null) {
					mesh_spe = new Mesh();
					mesh_spe.vertices = new_vertices_spe;
					mesh_spe.normals = new_normals_spe;
					mesh_spe.triangles = triangles_spe;
					if (new_boneWeights_spe != null) {
						mesh_spe.boneWeights = new_boneWeights_spe;
						mesh_spe.bindposes = mesh.bones.bindPoses;
					}
					for (int i = 0; i < num_textures; i++) {
						mesh_spe.SetUVs(i, new_uvs_spe[i].ToList());
					}
				} else {
					mesh_spe = CopyMesh(mesh_spe);
				}
				//mesh.SetUVs(0, new_uvs_spe.ToList());
				/*mesh.uv = new_uvs_spe;*/
				if (new_boneWeights_spe != null) {
                    mr_spe = gao_spe.AddComponent<SkinnedMeshRenderer>();
                    s_mr_spe = (SkinnedMeshRenderer)mr_spe;
                    s_mr_spe.bones = mesh.bones.bones;
                    s_mr_spe.rootBone = mesh.bones.bones[0];
                    s_mr_spe.sharedMesh = CopyMesh(mesh_spe);
					
					BoxCollider bc = gao_spe.AddComponent<BoxCollider>();
					bc.center = s_mr_spe.bounds.center;
					bc.size = s_mr_spe.bounds.size;
				} else {
                    MeshFilter mf = gao_spe.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh_spe;
                    mr_spe = gao_spe.AddComponent<MeshRenderer>();
					MeshCollider mc = gao_spe.AddComponent<MeshCollider>();
					mc.isTrigger = false;
					mc.sharedMesh = mesh_spe;
				}
                //}
            }
            if (visualMaterial != null) {
                gao.name += " " + visualMaterial.offset + " - " + (visualMaterial.textures.Count > 0 ? visualMaterial.textures[0].offset.ToString() : "NULL" );
                Material unityMat = visualMaterial.GetMaterial(materialHints);
				if (vertexColors != null & unityMat != null) unityMat.SetVector("_Tex2Params", new Vector4(60, 0, 0, 0));
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
                if (mr_main != null) {
                    mr_main.sharedMaterial = unityMat;
                    //mr_main.UpdateGIMaterials();
                    if (!receiveShadows) mr_main.receiveShadows = false;
                    if (visualMaterial.animTextures.Count > 0) {
                        MultiTextureMaterial mtmat = mr_main.gameObject.AddComponent<MultiTextureMaterial>();
                        mtmat.visMat = visualMaterial;
                        mtmat.mat = mr_main.sharedMaterial;
                    }
                    /*if (scroll) {
                        ScrollingTexture scrollComponent = mr_main.gameObject.AddComponent<ScrollingTexture>();
                        scrollComponent.visMat = visualMaterial;
                        scrollComponent.mat = mr_main.material;
                    }*/
                }
                if (mr_spe != null) {
                    mr_spe.sharedMaterial = unityMat;
                    //mr_spe.UpdateGIMaterials();
                    if (!receiveShadows) mr_spe.receiveShadows = false;
                    if (visualMaterial.animTextures.Count > 0) {
                        MultiTextureMaterial mtmat = mr_spe.gameObject.AddComponent<MultiTextureMaterial>();
                        mtmat.visMat = visualMaterial;
                        mtmat.mat = mr_spe.sharedMaterial;
                    }
                    /*if (scroll) {
                        ScrollingTexture scrollComponent = mr_spe.gameObject.AddComponent<ScrollingTexture>();
                        scrollComponent.visMat = visualMaterial;
                        scrollComponent.mat = mr_spe.material;
                    }*/
                }
            }
        }
		public void MorphVertices(MeshElement el, float lerp) {
			// Use UpdateMeshVertices if possible! Only use this for special cases
			if (mesh_main != null) {
				Vector3[] new_vertices = mesh_main.vertices;
				for (int j = 0; j < num_mapping_entries; j++) {
					Vector3 from = mesh.vertices[mapping_vertices[j]];
					Vector3 to = el.mesh.vertices[el.mapping_vertices[j]];
					new_vertices[j] = Vector3.LerpUnclamped(from, to, lerp);
				}
				mesh_main.vertices = new_vertices;
			}
			if (mesh_spe != null) {
				Vector3[] new_vertices_spe = mesh_spe.vertices;
				for (int j = 0; j < num_disconnected_triangles_spe; j++) {
					int i0 = disconnected_triangles_spe[(j * 3) + 0], o0 = el.disconnected_triangles_spe[(j * 3) + 0], m0 = (j * 3) + 0; // Old index, mapped index
					int i1 = disconnected_triangles_spe[(j * 3) + 1], o1 = el.disconnected_triangles_spe[(j * 3) + 1], m1 = (j * 3) + 1;
					int i2 = disconnected_triangles_spe[(j * 3) + 2], o2 = el.disconnected_triangles_spe[(j * 3) + 2], m2 = (j * 3) + 2;
					new_vertices_spe[m0] = Vector3.LerpUnclamped(mesh.vertices[i0], el.mesh.vertices[o0], lerp);
					new_vertices_spe[m1] = Vector3.LerpUnclamped(mesh.vertices[i1], el.mesh.vertices[o1], lerp);
					new_vertices_spe[m2] = Vector3.LerpUnclamped(mesh.vertices[i2], el.mesh.vertices[o2], lerp);
				}
				mesh_spe.vertices = new_vertices_spe;
			}
		}

		public void UpdateMeshVertices(Vector3[] vertices) {
			if (mesh_main != null) {
				Vector3[] new_vertices = mesh_main.vertices;
				for (int j = 0; j < num_mapping_entries; j++) {
					new_vertices[j] = vertices[mapping_vertices[j]];
				}
				mesh_main.vertices = new_vertices;
			}
			if (mesh_spe != null) {
				Vector3[] new_vertices_spe = mesh_spe.vertices;
				for (int j = 0; j < num_disconnected_triangles_spe; j++) {
					int i0 = disconnected_triangles_spe[(j * 3) + 0], m0 = (j * 3) + 0; // Old index, mapped index
					int i1 = disconnected_triangles_spe[(j * 3) + 1], m1 = (j * 3) + 1;
					int i2 = disconnected_triangles_spe[(j * 3) + 2], m2 = (j * 3) + 2;
					new_vertices_spe[m0] = vertices[i0];
					new_vertices_spe[m1] = vertices[i1];
					new_vertices_spe[m2] = vertices[i2];
				}
				mesh_spe.vertices = new_vertices_spe;
			}
		}
		public void ResetVertices() {
			UpdateMeshVertices(mesh.vertices);
		}

        public static MeshElement Read(Reader reader, Pointer offset, MeshObject m) {
            MapLoader l = MapLoader.Loader;
            MeshElement sm = new MeshElement(offset, m);
            sm.name = "Submesh @ pos " + offset;
			l.print(sm.name);
            sm.backfaceCulling = !l.forceDisplayBackfaces;
            sm.off_material = Pointer.Read(reader);
            if (Settings.s.engineVersion == Settings.EngineVersion.R3 || Settings.s.game == Settings.Game.R2Revolution) {
                sm.visualMaterial = VisualMaterial.FromOffset(sm.off_material);
            } else {
                sm.gameMaterial = GameMaterial.FromOffsetOrRead(sm.off_material, reader);
                sm.visualMaterial = sm.gameMaterial.visualMaterial;
            }
            /*if (sm.visualMaterial != null && sm.visualMaterial.textures.Count > 0 && sm.visualMaterial.textures[0].off_texture != null) {
                sm.name += " - VisMatTex:" + sm.visualMaterial.textures[0].offset + " - TexInfo:" + sm.visualMaterial.textures[0].off_texture;
            }*/
            if (sm.visualMaterial != null) {
                sm.backfaceCulling = ((sm.visualMaterial.flags & VisualMaterial.flags_backfaceCulling) != 0) && !l.forceDisplayBackfaces;
            }
            sm.num_disconnected_triangles_spe = reader.ReadUInt16();
			if (Settings.s.game == Settings.Game.R2Revolution) {
				sm.lightmap_index = reader.ReadInt16();
				sm.off_disconnected_triangles_spe = Pointer.Read(reader);
			} else {
				sm.num_uvs = reader.ReadUInt16();
				if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
					sm.num_uvMaps = reader.ReadUInt16();
					reader.ReadUInt16();
				}
				sm.off_disconnected_triangles_spe = Pointer.Read(reader); // 1 entry = 3 shorts. Max: num_vertices
				if (Settings.s.mode == Settings.Mode.Rayman3GC) reader.ReadUInt32();
				sm.off_mapping_uvs_spe = Pointer.Read(reader); // 1 entry = 3 shorts. Max: num_weights
				sm.off_weights_spe = Pointer.Read(reader); // 1 entry = 3 floats
				sm.off_uvs = Pointer.Read(reader); // 1 entry = 2 floats
				if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
					reader.ReadUInt32();
					reader.ReadUInt32();
				} else if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
					reader.ReadUInt32();
				}
				if (Settings.s.game != Settings.Game.TTSE) {
					sm.off_vertex_indices = Pointer.Read(reader);
					sm.num_vertex_indices = reader.ReadUInt16();
					reader.ReadInt16();
					reader.ReadUInt32();
				}
			}
            if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
				if (Settings.s.game != Settings.Game.Dinosaur) {
					reader.ReadUInt16();
					sm.num_mapping_entries = reader.ReadUInt16(); // num_shorts
					sm.off_mapping_vertices = Pointer.Read(reader); // shorts_offset1 (1st array of size num_shorts, max_num_vertices)
					sm.off_mapping_uvs = Pointer.Read(reader); // shorts_offset2 (2nd array of size num_shorts, max: num_weights)
					sm.num_connected_vertices = reader.ReadUInt16(); // num_shorts2
					sm.num_disconnected_triangles = reader.ReadUInt16();
					sm.off_connected_vertices = Pointer.Read(reader); // shorts2_offset (array of size num_shorts2)
					sm.off_disconnected_triangles = Pointer.Read(reader);
					if (Settings.s.hasNames) sm.name += reader.ReadString(0x34);
				} else {
					sm.num_mapping_entries = 0;
					sm.off_mapping_vertices = null;
					sm.off_mapping_uvs = null;
					sm.num_connected_vertices = 0;
					sm.num_disconnected_triangles = 0;
					sm.off_connected_vertices = null;
					sm.off_disconnected_triangles = null;
				}
            } else {
                // Defaults for Rayman 2
                sm.num_uvMaps = 1;
                sm.num_mapping_entries = 0;
                sm.off_mapping_vertices = null;
                sm.off_mapping_uvs = null;
                sm.num_connected_vertices = 0;
                sm.num_disconnected_triangles = 0;
                sm.off_connected_vertices = null;
                sm.off_disconnected_triangles = null;
            }

            // Read mapping tables
            sm.mapping_uvs = new int[sm.num_uvMaps][];
            if (sm.num_mapping_entries > 0) {
                Pointer.Goto(ref reader, sm.off_mapping_vertices);
                //print("Mapping offset: " + String.Format("0x{0:X}", fs.Position));
                sm.mapping_vertices = new int[sm.num_mapping_entries];
                for (int j = 0; j < sm.num_mapping_entries; j++) {
                    sm.mapping_vertices[j] = reader.ReadInt16();
                }
                Pointer.Goto(ref reader, sm.off_mapping_uvs);
                for (int j = 0; j < sm.num_uvMaps; j++) {
                    sm.mapping_uvs[j] = new int[sm.num_mapping_entries];
                }
                for (int j = 0; j < sm.num_mapping_entries; j++) {
                    for (int um = 0; um < sm.num_uvMaps; um++) {
                        sm.mapping_uvs[um][j] = reader.ReadInt16();
                    }
                }
            }
            if (sm.num_disconnected_triangles_spe > 0) {
                Pointer.Goto(ref reader, sm.off_mapping_uvs_spe);
                sm.mapping_uvs_spe = new int[sm.num_uvMaps][];
                for (int j = 0; j < sm.num_uvMaps; j++) {
                    sm.mapping_uvs_spe[j] = new int[sm.num_disconnected_triangles_spe * 3];
                }
                // Why is uv maps here the outer loop instead of inner like the other thing?
                for (int um = 0; um < sm.num_uvMaps; um++) {
                    for (int j = 0; j < sm.num_disconnected_triangles_spe * 3; j++) {
                        sm.mapping_uvs_spe[um][j] = reader.ReadInt16();
                    }
                }
            }

			// Read UVs
			Pointer.DoAt(ref reader, sm.off_uvs, () => {
				sm.uvs = new Vector2[sm.num_uvs];
				for (int j = 0; j < sm.num_uvs; j++) {
					sm.uvs[j] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
				}
			});
			// Read triangle data
			Pointer.DoAt(ref reader, sm.off_connected_vertices, () => {
				sm.connected_vertices = new int[sm.num_connected_vertices];
				for (int j = 0; j < sm.num_connected_vertices; j++) {
					sm.connected_vertices[j] = reader.ReadInt16();
				}
			});
			Pointer.DoAt(ref reader, sm.off_disconnected_triangles, () => {
				sm.disconnected_triangles = new int[sm.num_disconnected_triangles * 3];
				//print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
				for (int j = 0; j < sm.num_disconnected_triangles; j++) {
					sm.disconnected_triangles[(j * 3) + 0] = reader.ReadInt16();
					sm.disconnected_triangles[(j * 3) + 1] = reader.ReadInt16();
					sm.disconnected_triangles[(j * 3) + 2] = reader.ReadInt16();
				}
			});
            if (sm.num_disconnected_triangles_spe > 0) {
                Pointer.Goto(ref reader, sm.off_disconnected_triangles_spe);
                sm.disconnected_triangles_spe = new int[sm.num_disconnected_triangles_spe * 3];
                //print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
                for (int j = 0; j < sm.num_disconnected_triangles_spe; j++) {
                    sm.disconnected_triangles_spe[(j * 3) + 0] = reader.ReadInt16();
                    sm.disconnected_triangles_spe[(j * 3) + 1] = reader.ReadInt16();
                    sm.disconnected_triangles_spe[(j * 3) + 2] = reader.ReadInt16();
                }
                if (sm.off_weights_spe != null) {
                    Pointer.Goto(ref reader, sm.off_weights_spe);
                    sm.normals_spe = new Vector3[sm.num_disconnected_triangles_spe];
                    for (int j = 0; j < sm.num_disconnected_triangles_spe; j++) {
                        float x = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        sm.normals_spe[j] = new Vector3(x, y, z);
                    }
                }
            }
            return sm;
        }

        public void ReinitBindPoses() {
            if (s_mr_main != null) {
                Mesh newmesh = CopyMesh(mesh_main);
                newmesh.bindposes = mesh.bones.bindPoses;
                s_mr_main.sharedMesh = newmesh;
            }
            if (s_mr_spe != null) {
                Mesh newmesh = CopyMesh(mesh_spe);
                newmesh.bindposes = mesh.bones.bindPoses;
                s_mr_spe.sharedMesh = newmesh;
            }
        }

        // Call after clone
        public void Reset() {
            gao = null;
            s_mr_main = null;
            s_mr_spe = null;
			mr_main = null;
			mr_spe = null;
            mesh_main = null;
            mesh_spe = null;
			if (mesh.bones != null) {
				mesh_main = null;
				mesh_spe = null;
			}
        }

        public IGeometricElement Clone(MeshObject mesh) {
            MeshElement sm = (MeshElement)MemberwiseClone();
            sm.mesh = mesh;
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
