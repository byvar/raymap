using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// Mesh data (both static and dynamic)
    /// </summary>
    public class R3Mesh : R3PhysicalObject {
        public GameObject gao = null;
        public List<R3Unknown> listUnknown;

        public R3Pointer off_modelstart;
        public R3Pointer off_vertices;
        public R3Pointer off_normals;
        public R3Pointer off_blendWeights;
        public R3Pointer off_subblock_types;
        public R3Pointer off_subblocks;
        public uint num_vertices;
        public uint num_subblocks;
        public string name;
        public Vector3[] vertices = null;
        public Vector3[] normals = null;
        public float[] blendWeights = null;
        public uint[] subblock_types = null;
        public IR3GeometricElement[] subblocks = null;
        public R3DeformSet bones = null;


        public R3Mesh(R3Pointer off_header, R3Pointer off_data) : base(off_header, off_data) {
            listUnknown = new List<R3Unknown>();
        }

        public static R3Mesh Read(EndianBinaryReader reader, R3Pointer off_header, R3Pointer off_data) {
            R3Loader l = R3Loader.Loader;
            R3Mesh m = new R3Mesh(off_header, off_data);
            R3Pointer off_modelstart = R3Pointer.Read(reader);
            m.off_modelstart = off_modelstart;
            R3Pointer.Goto(ref reader, off_modelstart);
            m.off_vertices = R3Pointer.Read(reader);
            m.off_normals = R3Pointer.Read(reader);
            m.off_blendWeights = R3Pointer.Read(reader);
            if (l.mode == R3Loader.Mode.Rayman3PC || l.mode == R3Loader.Mode.RaymanArenaPC || l.mode == R3Loader.Mode.Rayman3GC) {
                reader.ReadInt32();
            }
            m.off_subblock_types = R3Pointer.Read(reader);
            m.off_subblocks = R3Pointer.Read(reader);
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            m.num_vertices = reader.ReadUInt16();
            m.num_subblocks = reader.ReadUInt16();
            reader.ReadInt32();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt16();
            m.name = "Mesh";
            if (l.mode == R3Loader.Mode.Rayman3GC) m.name = new string(reader.ReadChars(0x32));
            // Vertices
            R3Pointer off_current = R3Pointer.Goto(ref reader, m.off_vertices);
            //print("Loading vertices at " + String.Format("0x{0:X}", fs.Position) + " | Amount: " + num_vertices);
            m.vertices = new Vector3[m.num_vertices];
            for (int i = 0; i < m.num_vertices; i++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                m.vertices[i] = new Vector3(x, y, z);
            }
            // Normals
            R3Pointer.Goto(ref reader, m.off_normals);
            m.normals = new Vector3[m.num_vertices];
            for (int i = 0; i < m.num_vertices; i++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                m.normals[i] = new Vector3(x, y, z);
            }
            if (m.off_blendWeights != null) {
                R3Pointer.Goto(ref reader, m.off_blendWeights);
                /*reader.ReadUInt32(); // 0
                R3Pointer off_blendWeightsStart = R3Pointer.Read(reader);
                R3Pointer.Goto(ref reader, off_blendWeightsStart);*/
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                m.blendWeights = new float[m.num_vertices];
                for (int i = 0; i < m.num_vertices; i++) {
                    m.blendWeights[i] = reader.ReadSingle();
                }
            }
            // Read subblock types & initialize arrays
            R3Pointer.Goto(ref reader, m.off_subblock_types);
            m.subblock_types = new uint[m.num_subblocks];
            m.subblocks = new IR3GeometricElement[m.num_subblocks];
            for (uint i = 0; i < m.num_subblocks; i++) {
                m.subblock_types[i] = reader.ReadUInt16();
            }
            //R3Material[] materials = new R3Material[num_materials];
            m.gao = new GameObject(m.name);
            // Process blocks
            //uint material_i = 0;
            //print("Num: " + num_subblocks + " | Off: " + off_subblocks + " | Name: " + name);
            for (uint i = 0; i < m.num_subblocks; i++) {
                R3Pointer.Goto(ref reader, m.off_subblocks + (i * 4));
                R3Pointer block_offset = R3Pointer.Read(reader);
                R3Pointer.Goto(ref reader, block_offset);
                switch (m.subblock_types[i]) {
                    case 1: // Material
                        m.subblocks[i] = R3SubMesh.Read(reader, block_offset, m);
                        //material_i++;
                        break;
                    case 13:
                        m.bones = R3DeformSet.Read(reader, block_offset, m);
                        m.subblocks[i] = m.bones;
                        break;
                    default:
                        m.subblocks[i] = null;
                        l.print("Unknown geometric element type " + m.subblock_types[i] + " at offset " + block_offset);
                        break;
                }
            }
            for (uint i = 0; i < m.num_subblocks; i++) {
                if (m.subblocks[i] != null) {
                    if (m.subblocks[i] is R3SubMesh) {
                        GameObject child = ((R3SubMesh)m.subblocks[i]).Gao;
                        child.transform.SetParent(m.gao.transform);
                        child.transform.localPosition = Vector3.zero;
                    } else if (m.subblocks[i] is R3DeformSet) {
                        R3DeformSet ds = ((R3DeformSet)m.subblocks[i]);
                        for (int j = 0; j < ds.num_bones; j++) {
                            Transform b = ds.bones[j];
                            b.transform.SetParent(m.gao.transform);
                        }
                        //child.transform.SetParent(m.gao.transform);
                        //child.transform.localPosition = Vector3.zero;
                    }
                }
            }
            return m;
        }
    }
}
