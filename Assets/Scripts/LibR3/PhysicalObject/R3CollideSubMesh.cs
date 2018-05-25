using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3CollideSubMesh : IR3CollideGeometricElement {
        public R3CollideMesh mesh;
        public R3Pointer offset;

        public R3Pointer off_material;
        public R3Pointer off_triangles; // num_triangles * 3 * 0x2
        public R3Pointer off_normals; // num_triangles * 3 * 0x4. 1 normal per face, kinda logical for collision I guess
        public ushort num_triangles;
        public R3Pointer off_mapping;
        public R3Pointer off_unk;
        public R3Pointer off_unk2;
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

        public R3CollideSubMesh(R3Pointer offset, R3CollideMesh mesh) {
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
                mr.material = R3Loader.Loader.baseMaterial;
            }
        }

        public static R3CollideSubMesh Read(EndianBinaryReader reader, R3Pointer offset, R3CollideMesh m) {
            R3Loader l = R3Loader.Loader;
            R3CollideSubMesh sm = new R3CollideSubMesh(offset, m);
            sm.off_material = R3Pointer.Read(reader);
            sm.off_triangles = R3Pointer.Read(reader);
            sm.off_normals = R3Pointer.Read(reader);
            sm.num_triangles = reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt32();
            sm.off_mapping = R3Pointer.Read(reader);
            sm.off_unk = R3Pointer.Read(reader);
            sm.off_unk2 = R3Pointer.Read(reader);
            sm.num_mapping_entries = reader.ReadUInt16();
            reader.ReadUInt16();
            
            R3Pointer.Goto(ref reader, sm.off_triangles);
            sm.vertex_indices = new int[sm.num_triangles * 3];
            for (int j = 0; j < sm.num_triangles; j++) {
                sm.vertex_indices[(j * 3) + 0] = reader.ReadInt16();
                sm.vertex_indices[(j * 3) + 1] = reader.ReadInt16();
                sm.vertex_indices[(j * 3) + 2] = reader.ReadInt16();
            }
            R3Pointer.Goto(ref reader, sm.off_normals);
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

        public IR3CollideGeometricElement Clone(R3CollideMesh mesh) {
            R3CollideSubMesh sm = (R3CollideSubMesh)MemberwiseClone();
            sm.mesh = mesh;
            sm.Reset();
            return sm;
        }
    }
}
