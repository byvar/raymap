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
        public PhysicalObject po;
        public Pointer offset;

        public GameObject gao = null;

        public Pointer off_modelstart;
        public Pointer off_vertices;
        public Pointer off_normals;
        public Pointer off_blendWeights;
        public Pointer off_subblock_types;
        public Pointer off_subblocks;
        public ushort num_vertices;
        public ushort num_subblocks;
        public string name;
        public Vector3[] vertices = null;
        public Vector3[] normals = null;
        public float[] blendWeights = null;
        public ushort[] subblock_types = null;
        public IGeometricElement[] subblocks = null;
        public DeformSet bones = null;


        public MeshObject(PhysicalObject po, Pointer offset) {
            this.po = po;
            this.offset = offset;
        }

        public void InitGameObjects() {
            for (uint i = 0; i < num_subblocks; i++) {
                if (subblocks[i] != null) {
                    if (subblocks[i] is MeshElement) {
                        GameObject child = ((MeshElement)subblocks[i]).Gao;
                        child.transform.SetParent(gao.transform);
                        child.transform.localPosition = Vector3.zero;
                    } else if (subblocks[i] is DeformSet) {
                        DeformSet ds = ((DeformSet)subblocks[i]);
                        for (int j = 0; j < ds.num_bones; j++) {
                            Transform b = ds.bones[j];
                            b.transform.SetParent(gao.transform);
                        }
                    } else if (subblocks[i] is SpriteElement) {
                        GameObject child = ((SpriteElement)subblocks[i]).Gao;
                        child.transform.SetParent(gao.transform);
                        child.transform.localPosition = Vector3.zero;
                    }
                }
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

        public static MeshObject Read(EndianBinaryReader reader, PhysicalObject po, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            MeshObject m = new MeshObject(po, offset);
            Pointer off_modelstart = Pointer.Read(reader);
            m.off_modelstart = off_modelstart;
            Pointer.Goto(ref reader, off_modelstart);
            m.off_vertices = Pointer.Read(reader);
            m.off_normals = Pointer.Read(reader);
            m.off_blendWeights = Pointer.Read(reader);
            if (l.mode != MapLoader.Mode.RaymanArenaGC) {
                reader.ReadInt32();
            }
            m.off_subblock_types = Pointer.Read(reader);
            m.off_subblocks = Pointer.Read(reader);
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                reader.ReadInt32();
                reader.ReadInt32();
            }
            m.num_vertices = reader.ReadUInt16();
            m.num_subblocks = reader.ReadUInt16();
            reader.ReadInt32();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadInt32();
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                reader.ReadInt32();
                reader.ReadInt16();
            }
            m.name = "Mesh";
            if (l.mode == MapLoader.Mode.Rayman3GC) m.name = new string(reader.ReadChars(0x32)).TrimEnd('\0');
            // Vertices
            Pointer off_current = Pointer.Goto(ref reader, m.off_vertices);
            //print("Loading vertices at " + String.Format("0x{0:X}", fs.Position) + " | Amount: " + num_vertices);
            m.vertices = new Vector3[m.num_vertices];
            for (int i = 0; i < m.num_vertices; i++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                m.vertices[i] = new Vector3(x, y, z);
            }
            // Normals
            Pointer.Goto(ref reader, m.off_normals);
            m.normals = new Vector3[m.num_vertices];
            for (int i = 0; i < m.num_vertices; i++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                m.normals[i] = new Vector3(x, y, z);
            }
            if (m.off_blendWeights != null) {
                Pointer.Goto(ref reader, m.off_blendWeights);
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
            Pointer.Goto(ref reader, m.off_subblock_types);
            m.subblock_types = new ushort[m.num_subblocks];
            m.subblocks = new IGeometricElement[m.num_subblocks];
            for (uint i = 0; i < m.num_subblocks; i++) {
                m.subblock_types[i] = reader.ReadUInt16();
            }
            m.gao = new GameObject(m.name);
            m.gao.tag = "Visual";
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
            m.InitGameObjects();
            return m;
        }

        public IGeometricObject Clone() {
            MeshObject m = (MeshObject)MemberwiseClone();
            m.gao = new GameObject(m.name);
            m.gao.tag = "Visual";
            m.subblocks = new IGeometricElement[num_subblocks];
            for (uint i = 0; i < m.num_subblocks; i++) {
                if (subblocks[i] != null) {
                    m.subblocks[i] = subblocks[i].Clone(m);
                    if (m.subblocks[i] is DeformSet) m.bones = (DeformSet)m.subblocks[i];
                }
            }
            m.InitGameObjects();
            return m;
        }
    }
}
