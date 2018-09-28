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
        public PhysicalObject po;
        public Pointer offset;
        
        public Pointer off_vertices;
        public Pointer off_normals;
        public Pointer off_blendWeights;
        public Pointer off_materials;
        public Pointer off_subblock_types;
        public Pointer off_subblocks;
        public ushort num_vertices;
        public ushort num_subblocks;
        public string name;
        public Vector3[] vertices = null;
        public Vector3[] normals = null;
        public float[][] blendWeights = null;
        public ushort[] subblock_types = null;
        public IGeometricElement[] subblocks = null;
        public DeformSet bones = null;
        
        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) InitGameObject();
                return gao;
            }
        }

        public MeshObject(PhysicalObject po, Pointer offset) {
            this.po = po;
            this.offset = offset;
        }

        public void InitGameObject() {
            gao = new GameObject(name);
            gao.tag = "Visual";
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

        public static MeshObject Read(Reader reader, PhysicalObject po, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            MeshObject m = new MeshObject(po, offset);
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) m.num_vertices = (ushort)reader.ReadUInt32();
            m.off_vertices = Pointer.Read(reader);
            m.off_normals = Pointer.Read(reader);
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                m.off_materials = Pointer.Read(reader);
            } else {
                m.off_blendWeights = Pointer.Read(reader);
            }
            if (l.mode != MapLoader.Mode.RaymanArenaGC) {
                reader.ReadInt32();
            }
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) m.num_subblocks = (ushort)reader.ReadUInt32();
            m.off_subblock_types = Pointer.Read(reader);
            m.off_subblocks = Pointer.Read(reader);
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                if (Settings.s.engineVersion == Settings.EngineVersion.R2) {
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
                if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
                    reader.ReadInt32();
                    reader.ReadInt16();
                }
            } else {
                reader.ReadInt32();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
            }
            m.name = "Mesh @ " + offset;
            if (Settings.s.hasNames) m.name = reader.ReadString(0x32);
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
            m.InitGameObject();
            return m;
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
