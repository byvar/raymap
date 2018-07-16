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
        public MeshObject mesh;
        public Pointer offset;

        public string name;
        public Pointer off_material;
        public VisualMaterial r3mat;
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

        private SkinnedMeshRenderer s_mr_main = null;
        private SkinnedMeshRenderer s_mr_spe = null;
        private Mesh mesh_main = null;
        private Mesh mesh_spe = null;


        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);// Create object and read triangle data
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

            Renderer mr_main = null, mr_spe = null;
            long num_triangles_main = ((num_connected_vertices > 2 ? num_connected_vertices - 2 : 0) + num_disconnected_triangles) * (backfaceCulling ? 1 : 2);
            uint triangle_size = 3 * (uint)(backfaceCulling ? 1 : 2);
            uint triangles_index = 0;
            if (num_triangles_main > 0) {
                Vector3[] new_vertices = new Vector3[num_mapping_entries];
                Vector3[] new_normals = new Vector3[num_mapping_entries];
                Vector3[][] new_uvs = new Vector3[num_uvMaps][]; // Thanks to Unity we can only store the blend weights as a third component of the UVs
                BoneWeight[] new_boneWeights = mesh.bones != null ? new BoneWeight[num_mapping_entries] : null;
                for (int um = 0; um < num_uvMaps; um++) {
                    new_uvs[um] = new Vector3[num_mapping_entries];
                }
                for (int j = 0; j < num_mapping_entries; j++) {
                    new_vertices[j] = mesh.vertices[mapping_vertices[j]];
                    new_normals[j] = mesh.normals[mapping_vertices[j]];
                    if (new_boneWeights != null) new_boneWeights[j] = mesh.bones.weights[mapping_vertices[j]];
                    for (int um = 0; um < num_uvMaps; um++) {
                        new_uvs[um][j] = uvs[mapping_uvs[um][j]];
                        if (mesh.blendWeights != null) {
                            new_uvs[um][j].z = mesh.blendWeights[mapping_vertices[j]];
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
                mesh_main = new Mesh();
                mesh_main.vertices = new_vertices;
                mesh_main.normals = new_normals;
                mesh_main.triangles = triangles;
                if (new_boneWeights != null) {
                    mesh_main.boneWeights = new_boneWeights;
                    mesh_main.bindposes = mesh.bones.bindPoses;
                }
                for (int i = 0; i < num_uvMaps; i++) {
                    mesh_main.SetUVs(i, new_uvs[i].ToList());
                }
                if (new_boneWeights != null) {
                    mr_main = gao.AddComponent<SkinnedMeshRenderer>();
                    s_mr_main = (SkinnedMeshRenderer)mr_main;
                    s_mr_main.bones = mesh.bones.bones;
                    s_mr_main.rootBone = mesh.bones.bones[0];
                    s_mr_main.sharedMesh = CopyMesh(mesh_main);
                } else {
                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    mf.mesh = mesh_main;
                    mr_main = gao.AddComponent<MeshRenderer>();
                }
            }
            if (num_disconnected_triangles_spe > 0) {
                Vector3[] new_vertices_spe = new Vector3[num_disconnected_triangles_spe * 3];
                Vector3[] new_normals_spe = new Vector3[num_disconnected_triangles_spe * 3];
                Vector3[][] new_uvs_spe = new Vector3[num_uvMaps][];
                BoneWeight[] new_boneWeights_spe = mesh.bones != null ? new BoneWeight[num_disconnected_triangles_spe * 3] : null;
                for (int um = 0; um < num_uvMaps; um++) {
                    new_uvs_spe[um] = new Vector3[num_disconnected_triangles_spe * 3];
                }
                int[] triangles_spe = new int[num_disconnected_triangles_spe * triangle_size];
                triangles_index = 0;
                for (int um = 0; um < num_uvMaps; um++) {
                    for (int j = 0; j < num_disconnected_triangles_spe * 3; j++) {
                        new_uvs_spe[um][j] = uvs[mapping_uvs_spe[um][j]];
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
                    new_vertices_spe[m0] = mesh.vertices[i0];
                    new_vertices_spe[m1] = mesh.vertices[i1];
                    new_vertices_spe[m2] = mesh.vertices[i2];
                    new_normals_spe[m0] = mesh.normals[i0];
                    new_normals_spe[m1] = mesh.normals[i1];
                    new_normals_spe[m2] = mesh.normals[i2];
                    if (new_boneWeights_spe != null) {
                        new_boneWeights_spe[m0] = mesh.bones.weights[i0];
                        new_boneWeights_spe[m1] = mesh.bones.weights[i1];
                        new_boneWeights_spe[m2] = mesh.bones.weights[i2];
                    }
                    if (mesh.blendWeights != null) {
                        for (int um = 0; um < num_uvMaps; um++) {
                            new_uvs_spe[um][m0].z = mesh.blendWeights[i0];
                            new_uvs_spe[um][m1].z = mesh.blendWeights[i1];
                            new_uvs_spe[um][m2].z = mesh.blendWeights[i2];
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
                    mesh_spe = new Mesh();
                    mesh_spe.vertices = new_vertices_spe;
                    mesh_spe.normals = new_normals_spe;
                    mesh_spe.triangles = triangles_spe;
                    if (new_boneWeights_spe != null) {
                        mesh_spe.boneWeights = new_boneWeights_spe;
                        mesh_spe.bindposes = mesh.bones.bindPoses;
                    }
                    for (int i = 0; i < num_uvMaps; i++) {
                        mesh_spe.SetUVs(i, new_uvs_spe[i].ToList());
                    }
                    //mesh.SetUVs(0, new_uvs_spe.ToList());
                    /*mesh.uv = new_uvs_spe;*/
                    if (new_boneWeights_spe != null) {
                        mr_spe = gao_spe.AddComponent<SkinnedMeshRenderer>();
                        s_mr_spe = (SkinnedMeshRenderer)mr_spe;
                        s_mr_spe.bones = mesh.bones.bones;
                        s_mr_spe.rootBone = mesh.bones.bones[0];
                        s_mr_spe.sharedMesh = CopyMesh(mesh_spe);
                    } else {
                        MeshFilter mf = gao_spe.AddComponent<MeshFilter>();
                        mf.mesh = mesh_spe;
                        mr_spe = gao_spe.AddComponent<MeshRenderer>();
                    }
                //}
            }
            if (r3mat != null) {
                gao.name += " " + r3mat.offset;
                Material unityMat = r3mat.Material;
                bool receiveShadows = (r3mat.properties & VisualMaterial.property_receiveShadows) != 0;
                if (num_uvMaps > 1) unityMat.SetFloat("_UVSec", 50f);
                //if (r3mat.Material.GetColor("_EmissionColor") != Color.black) print("Mesh with emission: " + name);
                if (mr_main != null) {
                    mr_main.material = unityMat;
                    //mr_main.UpdateGIMaterials();
                    if (!receiveShadows) mr_main.receiveShadows = false;
                    if (r3mat.animTextures.Count > 0) {
                        MultiTextureMaterial mtmat = mr_main.gameObject.AddComponent<MultiTextureMaterial>();
                        mtmat.r3mat = r3mat;
                        mtmat.mat = mr_main.material;
                    }
                    if (r3mat.scrollingEnabled) {
                        ScrollingTexture scrollComponent = mr_main.gameObject.AddComponent<ScrollingTexture>();
                        scrollComponent.r3mat = r3mat;
                        scrollComponent.mat = mr_main.material;
                    }
                }
                if (mr_spe != null) {
                    mr_spe.material = unityMat;
                    //mr_spe.UpdateGIMaterials();
                    if (!receiveShadows) mr_spe.receiveShadows = false;
                    if (r3mat.animTextures.Count > 0) {
                        MultiTextureMaterial mtmat = mr_spe.gameObject.AddComponent<MultiTextureMaterial>();
                        mtmat.r3mat = r3mat;
                        mtmat.mat = mr_spe.material;
                    }
                    if (r3mat.scrollingEnabled) {
                        ScrollingTexture scrollComponent = mr_spe.gameObject.AddComponent<ScrollingTexture>();
                        scrollComponent.r3mat = r3mat;
                        scrollComponent.mat = mr_spe.material;
                    }
                }
            }
        }

        public static MeshElement Read(EndianBinaryReader reader, Pointer offset, MeshObject m) {
            MapLoader l = MapLoader.Loader;
            MeshElement sm = new MeshElement(offset, m);
            sm.name = "Submesh @ pos " + offset;
            sm.backfaceCulling = !l.forceDisplayBackfaces;
            sm.off_material = Pointer.Read(reader);
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                sm.r3mat = VisualMaterial.FromOffset(sm.off_material);
            } else {
                Pointer off_current = Pointer.Goto(ref reader, sm.off_material);
                sm.off_material = Pointer.Read(reader);
                if (sm.off_material != null) {
                    Pointer.Goto(ref reader, sm.off_material);
                    sm.r3mat = VisualMaterial.FromOffset(sm.off_material, createIfNull: true);
                } else sm.r3mat = null;
                Pointer.Goto(ref reader, off_current);
            }
            if (sm.r3mat != null) {
                sm.backfaceCulling = ((sm.r3mat.flags & VisualMaterial.flags_backfaceCulling) != 0) && !l.forceDisplayBackfaces;
            }
            sm.num_disconnected_triangles_spe = reader.ReadUInt16();
            sm.num_uvs = reader.ReadUInt16();
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                sm.num_uvMaps = reader.ReadUInt16();
                reader.ReadUInt16();
            }
            sm.off_disconnected_triangles_spe = Pointer.Read(reader); // 1 entry = 3 shorts. Max: num_vertices
            if (l.mode == MapLoader.Mode.Rayman3GC) reader.ReadUInt32();
            sm.off_mapping_uvs_spe = Pointer.Read(reader); // 1 entry = 3 shorts. Max: num_weights
            sm.off_weights_spe = Pointer.Read(reader); // 1 entry = 3 floats
            sm.off_uvs = Pointer.Read(reader); // 1 entry = 2 floats
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            sm.off_vertex_indices = Pointer.Read(reader);
            sm.num_vertex_indices = reader.ReadUInt16();
            reader.ReadInt16();
            reader.ReadUInt32();
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                reader.ReadUInt16();
                sm.num_mapping_entries = reader.ReadUInt16(); // num_shorts
                sm.off_mapping_vertices = Pointer.Read(reader); // shorts_offset1 (1st array of size num_shorts, max_num_vertices)
                sm.off_mapping_uvs = Pointer.Read(reader); // shorts_offset2 (2nd array of size num_shorts, max: num_weights)
                sm.num_connected_vertices = reader.ReadUInt16(); // num_shorts2
                sm.num_disconnected_triangles = reader.ReadUInt16();
                sm.off_connected_vertices = Pointer.Read(reader); // shorts2_offset (array of size num_shorts2)
                sm.off_disconnected_triangles = Pointer.Read(reader);
                if (l.mode == MapLoader.Mode.Rayman3GC) sm.name = new string(reader.ReadChars(0x34)).TrimEnd('\0');
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
            Pointer.Goto(ref reader, sm.off_uvs);
            sm.uvs = new Vector2[sm.num_uvs];
            for (int j = 0; j < sm.num_uvs; j++) {
                sm.uvs[j] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            }
            // Read triangle data
            Pointer.Goto(ref reader, sm.off_connected_vertices);
            //print("Creating triangles from connected vertices at " + String.Format("0x{0:X}", fs.Position));
            sm.connected_vertices = new int[sm.num_connected_vertices];
            for (int j = 0; j < sm.num_connected_vertices; j++) {
                sm.connected_vertices[j] = reader.ReadInt16();
            }
            Pointer.Goto(ref reader, sm.off_disconnected_triangles);
            sm.disconnected_triangles = new int[sm.num_disconnected_triangles * 3];
            //print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
            for (int j = 0; j < sm.num_disconnected_triangles; j++) {
                sm.disconnected_triangles[(j * 3) + 0] = reader.ReadInt16();
                sm.disconnected_triangles[(j * 3) + 1] = reader.ReadInt16();
                sm.disconnected_triangles[(j * 3) + 2] = reader.ReadInt16();
            }
            if (sm.num_disconnected_triangles_spe > 0) {
                Pointer.Goto(ref reader, sm.off_disconnected_triangles_spe);
                sm.disconnected_triangles_spe = new int[sm.num_disconnected_triangles_spe * 3];
                //print("Loading disconnected triangles at " + String.Format("0x{0:X}", fs.Position));
                for (int j = 0; j < sm.num_disconnected_triangles_spe; j++) {
                    sm.disconnected_triangles_spe[(j * 3) + 0] = reader.ReadInt16();
                    sm.disconnected_triangles_spe[(j * 3) + 1] = reader.ReadInt16();
                    sm.disconnected_triangles_spe[(j * 3) + 2] = reader.ReadInt16();
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
            mesh_main = null;
            mesh_spe = null;
        }

        public IGeometricElement Clone(MeshObject mesh) {
            MeshElement sm = (MeshElement)MemberwiseClone();
            sm.mesh = mesh;
            sm.Reset();
            return sm;
        }

        private Mesh CopyMesh(Mesh mesh) {
            Mesh newmesh = new Mesh();
            newmesh.vertices = mesh.vertices;
            newmesh.triangles = mesh.triangles;
            for (int i = 0; i < num_uvMaps; i++) {
                List<Vector2> uvsTemp = new List<Vector2>();
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
