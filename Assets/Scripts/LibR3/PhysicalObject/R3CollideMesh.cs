using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// Mesh data (both static and dynamic)
    /// </summary>
    public class R3CollideMesh {
        public R3PhysicalObject po;
        public R3Pointer offset;

        public GameObject gao = null;

        public R3Pointer off_modelstart;
        public ushort num_vertices;
        public ushort num_subblocks;
        public R3Pointer off_vertices;
        public R3Pointer off_subblock_types;
        public R3Pointer off_subblocks;

        public Vector3[] vertices = null;
        public ushort[] subblock_types = null;
        public IR3CollideGeometricElement[] subblocks = null;


        public R3CollideMesh(R3PhysicalObject po, R3Pointer offset) {
            this.po = po;
            this.offset = offset;
        }

        public static R3CollideMesh Read(EndianBinaryReader reader, R3PhysicalObject po, R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            R3CollideMesh m = new R3CollideMesh(po, offset);
            m.num_vertices = reader.ReadUInt16();
            m.num_subblocks = reader.ReadUInt16();
            reader.ReadUInt32();
            m.off_vertices = R3Pointer.Read(reader);
            m.off_subblock_types = R3Pointer.Read(reader);
            m.off_subblocks = R3Pointer.Read(reader);
            R3Pointer.Read(reader);
            reader.ReadInt32();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            
            // Vertices
            R3Pointer off_current = R3Pointer.Goto(ref reader, m.off_vertices);
            m.vertices = new Vector3[m.num_vertices];
            for (int i = 0; i < m.num_vertices; i++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                m.vertices[i] = new Vector3(x, y, z);
            }
            // Read subblock types & initialize arrays
            R3Pointer.Goto(ref reader, m.off_subblock_types);
            m.subblock_types = new ushort[m.num_subblocks];
            m.subblocks = new IR3CollideGeometricElement[m.num_subblocks];
            for (uint i = 0; i < m.num_subblocks; i++) {
                m.subblock_types[i] = reader.ReadUInt16();
            }
            m.gao = new GameObject("Collide Set @ " + offset);
            m.gao.tag = "Collide";
            for (uint i = 0; i < m.num_subblocks; i++) {
                R3Pointer.Goto(ref reader, m.off_subblocks + (i * 4));
                R3Pointer block_offset = R3Pointer.Read(reader);
                R3Pointer.Goto(ref reader, block_offset);
                switch (m.subblock_types[i]) {
                    case 1: // Collide submesh
                        m.subblocks[i] = R3CollideSubMesh.Read(reader, block_offset, m);
                        //material_i++;
                        break;
                    default:
                        m.subblocks[i] = null;
                        l.print("Unknown collide geometric element type " + m.subblock_types[i] + " at offset " + block_offset);
                        break;
                }
            }
            for (uint i = 0; i < m.num_subblocks; i++) {
                if (m.subblocks[i] != null) {
                    if (m.subblocks[i] is R3CollideSubMesh) {
                        GameObject child = ((R3CollideSubMesh)m.subblocks[i]).Gao;
                        child.transform.SetParent(m.gao.transform);
                        child.transform.localPosition = Vector3.zero;
                    }
                }
            }
            m.gao.SetActive(false); // Invisible by default
            return m;
        }

        public R3CollideMesh Clone() {
            R3CollideMesh m = (R3CollideMesh)MemberwiseClone();
            m.gao = new GameObject("Collide Set");
            m.gao.tag = "Collide";
            m.subblocks = new IR3CollideGeometricElement[num_subblocks];
            for (uint i = 0; i < m.num_subblocks; i++) {
                if (subblocks[i] != null) {
                    m.subblocks[i] = subblocks[i].Clone(m);
                }
            }
            for (uint i = 0; i < m.num_subblocks; i++) {
                if (m.subblocks[i] != null) {
                    if (m.subblocks[i] is R3CollideSubMesh) {
                        GameObject child = ((R3CollideSubMesh)m.subblocks[i]).Gao;
                        child.transform.SetParent(m.gao.transform);
                        child.transform.localPosition = Vector3.zero;
                    }
                }
            }
            m.gao.SetActive(false); // Invisible by default
            return m;
        }
    }
}
