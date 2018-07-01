using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Collide {
    public class CollideMeshElement : ICollideGeometricElement {
        public CollideMeshObject mesh;
        public Pointer offset;

        public Pointer off_material;
        public Pointer off_triangles; // num_triangles * 3 * 0x2
        public Pointer off_normals; // num_triangles * 3 * 0x4. 1 normal per face, kinda logical for collision I guess
        public ushort num_triangles;
        public Pointer off_mapping;
        public Pointer off_unk;
        public Pointer off_unk2;
        public ushort num_mapping_entries;
        
        public int[] vertex_indices = null;
        public int[] mapping = null;
        public Vector3[] normals = null;


        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("Collide Submesh @ " + offset);// Create object and read triangle data
                    CreateUnityMesh();
                }
                return gao;
            }
        }

        public CollideMeshElement(Pointer offset, CollideMeshObject mesh) {
            this.mesh = mesh;
            this.offset = offset;
        }

        private void CreateUnityMesh() {
            if(num_triangles > 0) {
                Vector3[] new_vertices = new Vector3[num_triangles * 3];
                Vector3[] new_normals = new Vector3[num_triangles * 3];

                for (int j = 0; j < num_triangles * 3; j++) {
                    new_vertices[j] = mesh.vertices[vertex_indices[j]];
                    new_normals[j] = normals[j/3];
                }
                int[] triangles = new int[num_triangles * 3];
                for (int j = 0; j < num_triangles; j++) {
                    triangles[(j * 3) + 0] = (j * 3) + 0;
                    triangles[(j * 3) + 1] = (j * 3) + 2;
                    triangles[(j * 3) + 2] = (j * 3) + 1;
                }
                Mesh meshUnity = new Mesh();
                meshUnity.vertices = new_vertices;
                meshUnity.normals = new_normals;
                meshUnity.triangles = triangles;
                MeshFilter mf = gao.AddComponent<MeshFilter>();
                mf.mesh = meshUnity;
                MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                mr.material = MapLoader.Loader.baseMaterial;
            }
        }

        public static CollideMeshElement Read(EndianBinaryReader reader, Pointer offset, CollideMeshObject m) {
            MapLoader l = MapLoader.Loader;
            CollideMeshElement sm = new CollideMeshElement(offset, m);
            sm.off_material = Pointer.Read(reader);
            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                sm.num_triangles = reader.ReadUInt16();
                reader.ReadUInt16();
                sm.off_triangles = Pointer.Read(reader);
                Pointer.Read(reader);
                sm.off_normals = Pointer.Read(reader);
                Pointer.Read(reader);
            } else {
                sm.off_triangles = Pointer.Read(reader);
                sm.off_normals = Pointer.Read(reader);
                sm.num_triangles = reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt32();
                sm.off_mapping = Pointer.Read(reader);
                sm.off_unk = Pointer.Read(reader);
                sm.off_unk2 = Pointer.Read(reader);
                sm.num_mapping_entries = reader.ReadUInt16();
                reader.ReadUInt16();
            }
            
            Pointer.Goto(ref reader, sm.off_triangles);
            sm.vertex_indices = new int[sm.num_triangles * 3];
            for (int j = 0; j < sm.num_triangles; j++) {
                sm.vertex_indices[(j * 3) + 0] = reader.ReadInt16();
                sm.vertex_indices[(j * 3) + 1] = reader.ReadInt16();
                sm.vertex_indices[(j * 3) + 2] = reader.ReadInt16();
            }
            Pointer.Goto(ref reader, sm.off_normals);
            sm.normals = new Vector3[sm.num_triangles];
            for (int j = 0; j < sm.num_triangles; j++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                sm.normals[j] = new Vector3(x, y, z);
            }
            /*R3Pointer.Goto(ref reader, sm.off_mapping);
            sm.mapping = new int[sm.num_triangles * 3];
            for (int j = 0; j < sm.num_triangles; j++) {
                sm.mapping[(j * 3) + 0] = reader.ReadInt16();
                sm.mapping[(j * 3) + 1] = reader.ReadInt16();
                sm.mapping[(j * 3) + 2] = reader.ReadInt16();
            }
            R3Pointer.Goto(ref reader, sm.off_unk);
            sm.normals = new Vector3[sm.num_mapping_entries];
            for (int j = 0; j < sm.num_mapping_entries; j++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                sm.normals[j] = new Vector3(x, y, z);
            }*/
            return sm;
        }

        // Call after clone
        public void Reset() {
            gao = null;
        }

        public ICollideGeometricElement Clone(CollideMeshObject mesh) {
            CollideMeshElement sm = (CollideMeshElement)MemberwiseClone();
            sm.mesh = mesh;
            sm.Reset();
            return sm;
        }
    }
}
