using OpenSpace.Loader;
using OpenSpace.Object;
using OpenSpace.Visual.Deform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Mesh data (both static and dynamic)
    /// </summary>
    public class MeshObject : IGeometricObject {
        public Pointer offset;
        
        public Pointer off_vertices;
        public Pointer off_normals;
        public Pointer off_blendWeights;
        public Pointer off_materials;
        public Pointer off_subblock_types;
        public Pointer off_subblocks;
		public Pointer off_mapping; // Revolution only
        public uint lookAtMode;
        public ushort num_vertices;
        public ushort num_subblocks;
        public string name;
        public Vector3[] vertices = null;
        public Vector3[] normals = null;
        public float[][] blendWeights = null;
        public ushort[] subblock_types = null;
		public int[][] mapping = null;
        public IGeometricElement[] subblocks = null;
        public DeformSet bones = null;
        
        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) InitGameObject();
                return gao;
            }
        }

        public MeshObject(Pointer offset) {
            this.offset = offset;
        }

        public void InitGameObject() {
            gao = new GameObject(name);
            gao.tag = "Visual";
            gao.layer = LayerMask.NameToLayer("Visual");
            if (bones != null) {
                GameObject child = bones.Gao;
                child.transform.SetParent(gao.transform);
                child.transform.localPosition = Vector3.zero;
            }
            for (uint i = 0; i < num_subblocks; i++) {
                if (subblocks[i] != null) {
                    GameObject child = subblocks[i].Gao;
                    child.transform.SetParent(gao.transform);
                    child.transform.localPosition = Vector3.zero;
                }
            }
            if (lookAtMode != 0) {
                BillboardBehaviour billboard = gao.AddComponent<BillboardBehaviour>();
                billboard.mode = (BillboardBehaviour.LookAtMode)lookAtMode;
            }
        }

        public void ReinitBindposes() {
            if (bones != null) {
                for (uint i = 0; i < num_subblocks; i++) {
                    if (subblocks[i] != null) {
                        if (subblocks[i] is MeshElement) {
                            ((MeshElement)subblocks[i]).ReinitBindPoses();
                        }
                    }
                }
            }
        }

        public static MeshObject Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            MeshObject m = new MeshObject(offset);
			if (Settings.s.game == Settings.Game.R2Revolution) {
				m.off_subblock_types = Pointer.Read(reader);
				m.off_subblocks = Pointer.Read(reader);
				uint flags = reader.ReadUInt32();
				reader.ReadSingle();
				reader.ReadSingle();
				reader.ReadSingle();
				reader.ReadSingle();
				m.off_mapping = Pointer.Read(reader);
				m.num_vertices = reader.ReadUInt16();
				m.num_subblocks = reader.ReadUInt16();
				m.off_vertices = Pointer.Read(reader);
				m.off_normals = Pointer.Read(reader);
				m.lookAtMode = flags & 3;
			} else {
				if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) m.num_vertices = (ushort)reader.ReadUInt32();
				m.off_vertices = Pointer.Read(reader);
				m.off_normals = Pointer.Read(reader);
				if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
					m.off_materials = Pointer.Read(reader);
				} else {
					m.off_blendWeights = Pointer.Read(reader);
				}
				if (Settings.s.mode != Settings.Mode.RaymanArenaGC) {
					reader.ReadInt32();
				}
				if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) m.num_subblocks = (ushort)reader.ReadUInt32();
				m.off_subblock_types = Pointer.Read(reader);
				m.off_subblocks = Pointer.Read(reader);
				reader.ReadInt32();
				reader.ReadInt32();
				if (Settings.s.engineVersion == Settings.EngineVersion.R2) {
					reader.ReadInt32();
					reader.ReadInt32();
				}
				if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
					m.lookAtMode = reader.ReadUInt32();
					//if (m.lookAtMode != 0) l.print(m.lookAtMode);
					m.num_vertices = reader.ReadUInt16();
					m.num_subblocks = reader.ReadUInt16();
					reader.ReadInt32();
					reader.ReadSingle();
					reader.ReadSingle();
					reader.ReadSingle();
					reader.ReadSingle();
					reader.ReadInt32();
					if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
						reader.ReadInt32();
						reader.ReadInt16();
					}
				} else {
					reader.ReadInt32();
					reader.ReadInt32();
					reader.ReadSingle();
					reader.ReadSingle();
					reader.ReadSingle();
					reader.ReadSingle();
				}
			}
            m.name = "Mesh @ " + offset;
            if (Settings.s.hasNames) m.name = reader.ReadString(0x32);
            // Vertices
			Pointer.DoAt(ref reader, m.off_vertices, () => {
				m.vertices = new Vector3[m.num_vertices];
				for (int i = 0; i < m.num_vertices; i++) {
					float x = reader.ReadSingle();
					float z = reader.ReadSingle();
					float y = reader.ReadSingle();
					m.vertices[i] = new Vector3(x, y, z);
				}
			});
			// Normals
			Pointer.DoAt(ref reader, m.off_normals, () => {
				m.normals = new Vector3[m.num_vertices];
				for (int i = 0; i < m.num_vertices; i++) {
					float x = reader.ReadSingle();
					float z = reader.ReadSingle();
					float y = reader.ReadSingle();
					m.normals[i] = new Vector3(x, y, z);
				}
			});
            Pointer.DoAt(ref reader, m.off_blendWeights, () => {
                m.blendWeights = new float[4][];
                /*reader.ReadUInt32(); // 0
                R3Pointer off_blendWeightsStart = R3Pointer.Read(reader);
                R3Pointer.Goto(ref reader, off_blendWeightsStart);*/
                for (int i = 0; i < 4; i++) {
                    Pointer off_blendWeights = Pointer.Read(reader);
                    Pointer.DoAt(ref reader, off_blendWeights, () => {
                        m.blendWeights[i] = new float[m.num_vertices];
                        for (int j = 0; j < m.num_vertices; j++) {
                            m.blendWeights[i][j] = reader.ReadSingle();
                        }
                    });
                }
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            });
			Pointer.DoAt(ref reader, m.off_mapping, () => {
				// Revolution only
				reader.ReadUInt32();
				Pointer.Read(reader);
				Pointer off_mappingBlocks = Pointer.Read(reader);
				Pointer.Read(reader);
				Pointer.Read(reader);
				ushort num_mappingBlocks = reader.ReadUInt16();
				reader.ReadUInt16();
				Pointer.DoAt(ref reader, off_mappingBlocks, () => {
					m.mapping = new int[num_mappingBlocks][];
					for (int i = 0; i < num_mappingBlocks; i++) {
						Pointer off_mapping = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_mapping, () => {
							m.mapping[i] = new int[m.num_vertices];
							for (int j = 0; j < m.num_vertices; j++) {
								m.mapping[i][j] = reader.ReadUInt16();
								if (m.mapping[i][j] >= m.num_vertices) l.print(m.offset);
							}
						});
					}
				});
			});
            // Read subblock types & initialize arrays
            Pointer.Goto(ref reader, m.off_subblock_types);
            m.subblock_types = new ushort[m.num_subblocks];
            m.subblocks = new IGeometricElement[m.num_subblocks];
            for (uint i = 0; i < m.num_subblocks; i++) {
                m.subblock_types[i] = reader.ReadUInt16();
            }
			// Process blocks
			for (uint i = 0; i < m.num_subblocks; i++) {
                Pointer.Goto(ref reader, m.off_subblocks + (i * 4));
                Pointer block_offset = Pointer.Read(reader);
                Pointer.Goto(ref reader, block_offset);
                switch (m.subblock_types[i]) {
                    case 1: // Material
                        m.subblocks[i] = MeshElement.Read(reader, block_offset, m);
                        break;
                    case 3: // Sprite
                        m.subblocks[i] = SpriteElement.Read(reader, block_offset, m);
                        break;
                    case 13:
                        m.bones = DeformSet.Read(reader, block_offset, m);
                        m.subblocks[i] = m.bones;
                        break;
                    default:
                        m.subblocks[i] = null;
                        /*1 = indexedtriangles
                        2 = facemap
                        3 = sprite
                        4 = TMesh
                        5 = points
                        6 = lines
                        7 = spheres
                        8 = alignedboxes
                        9 = cones
                        13 = deformationsetinfo*/
                        l.print("Unknown geometric element type " + m.subblock_types[i] + " at offset " + block_offset);
                        break;
                }
			}
			ReadMeshFromATO(reader, m);
			m.InitGameObject();
            return m;
        }

		private static void ReadMeshFromATO(Reader reader, MeshObject m) {
			// Revolution only: Before creating the gameobject, read the actual model data from the ATO
			if (Settings.s.game == Settings.Game.R2Revolution) {
				MapLoader l = MapLoader.Loader;
				List<MeshObject> meshObjects = new List<MeshObject>();
				for (uint i = 0; i < m.num_subblocks; i++) {
					if (m.subblock_types[i] == 1) {
						R2PS2Loader ps2l = (R2PS2Loader)l;
						meshObjects.Add(ps2l.ato.meshes[ps2l.ato.meshes.Length - 1 - ps2l.meshesRead - meshObjects.Count]);
					}
				}
				if (meshObjects.Count > 0) {
					int currentSubblock = 0;
					int curNumVertices = 0;
					bool tryMapping = true;
					if (m.vertices == null) {
						m.num_vertices = (ushort)meshObjects.Sum(mesh => mesh.num_vertices);
						m.vertices = new Vector3[m.num_vertices];
						m.normals = new Vector3[m.num_vertices];
						tryMapping = false;
					}
					for (int i = 0; i < meshObjects.Count; i++) {
						MeshObject mo = meshObjects[i];
						while (currentSubblock < m.num_subblocks && m.subblock_types[currentSubblock] != 1) {
							currentSubblock++;
						}
						MeshElement me = (MeshElement)m.subblocks[currentSubblock];
						MeshElement moe = ((MeshElement)mo.subblocks[0]);
						if (!tryMapping) {
							Array.Copy(mo.vertices, 0, m.vertices, curNumVertices, mo.num_vertices);
							if (mo.normals != null) {
								Array.Copy(mo.normals, 0, m.normals, curNumVertices, mo.num_vertices);
							}
							me.mapping_vertices = Enumerable.Range(curNumVertices, mo.num_vertices).ToArray();
							curNumVertices += mo.num_vertices;
						} else {
							me.mapping_vertices = new int[moe.num_mapping_entries];
							for (int j = 0; j < mo.vertices.Length; j++) {
								me.mapping_vertices[j] = Array.IndexOf(m.vertices, mo.vertices[j]);
								if (me.mapping_vertices[j] == -1 || me.mapping_vertices[j] != Array.IndexOf(m.vertices, mo.vertices[j])) {
									Debug.LogError("Failed matching vertices between Renderware and OpenSpace");
								}
							}
						}
						me.disconnected_triangles = moe.disconnected_triangles;
						me.num_disconnected_triangles = moe.num_disconnected_triangles;
						me.disconnected_triangles_spe = null;
						me.num_disconnected_triangles_spe = 0;
						me.num_uvMaps = moe.num_uvMaps;
						me.num_uvs = moe.num_uvs;
						me.uvs = moe.uvs;
						me.mapping_uvs = moe.mapping_uvs;
						me.num_mapping_entries = moe.num_mapping_entries;
						me.vertexColors = moe.vertexColors;
						currentSubblock++;
						if (me.lightmap_index != -1) {
							R2PS2Loader ps2l = ((R2PS2Loader)l);
							string id_r = me.lightmap_index.ToString("D3") + "." + 0;
							//l.print(id_r + " - " + me.num_disconnected_triangles + " - " + m.num_vertices + " - " + mo.num_vertices);
							Texture2D lm = ps2l.GetLightmap(id_r);
							if (me.visualMaterial != null) {
								if (lm != null) {
									string id_g = me.lightmap_index.ToString("D3") + "." + 1;
									string id_b = me.lightmap_index.ToString("D3") + "." + 2;
									Texture2D lm_g = ps2l.GetLightmap(id_g);
									Texture2D lm_b = ps2l.GetLightmap(id_b);
									if (lm_g != null && lm_b != null) {
										for (int j = 0; j < lm.width; j++) {
											for (int k = 0; k < lm.height; k++) {
												Color r = lm.GetPixel(j, k);
												Color g = lm.GetPixel(j, k);
												Color b = lm.GetPixel(j, k);
												lm.SetPixel(j, k, new Color(r.r, g.g, b.b, 1f));
											}
										}
										lm.Apply();
									}
								} else {
									lm = new Texture2D(1, 1);
									lm.SetPixel(0, 0, Color.white);
									lm.wrapMode = TextureWrapMode.Clamp;
									lm.filterMode = FilterMode.Bilinear;
									lm.Apply();
								}
								me.visualMaterial = me.visualMaterial.Clone();
								me.visualMaterial.num_textures += 1;
								me.visualMaterial.textures.Add(new VisualMaterialTexture() {
									texture = new TextureInfo(null) {
										width = (ushort)lm.width,
										height = (ushort)lm.height,
										Texture = lm
									},
									textureOp = 50,
									uvFunction = 1
								});
								Pointer.DoAt(ref reader, ps2l.off_lightmapUV[me.lightmap_index], () => {
									Array.Resize(ref me.uvs, me.num_uvs + mo.num_vertices);
									Array.Resize(ref me.mapping_uvs, me.num_uvMaps + 1);
									me.mapping_uvs[me.mapping_uvs.Length - 1] = Enumerable.Range(me.num_uvs, mo.num_vertices).ToArray();
									for (int j = 0; j < mo.num_vertices; j++) {
										me.uvs[me.num_uvs + j] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
									}
									me.num_uvs += mo.num_vertices;
									me.num_uvMaps++;
								});
							}
						}
					}
					((R2PS2Loader)l).meshesRead += (uint)meshObjects.Count;
				}
			}
		}

        // Call after clone
        public void Reset() {
            gao = null;
        }

        public IGeometricObject Clone() {
            MeshObject m = (MeshObject)MemberwiseClone();
            m.Reset();
            m.subblocks = new IGeometricElement[num_subblocks];
            for (uint i = 0; i < m.num_subblocks; i++) {
                if (subblocks[i] != null) {
                    m.subblocks[i] = subblocks[i].Clone(m);
                    if (m.subblocks[i] is DeformSet) m.bones = (DeformSet)m.subblocks[i];
                }
            }
            return m;
        }
    }
}
