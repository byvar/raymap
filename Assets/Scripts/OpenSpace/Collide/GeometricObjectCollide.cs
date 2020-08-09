using Newtonsoft.Json;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Collide {
    /// <summary>
    /// Mesh data (both static and dynamic)
    /// </summary>
    public class GeometricObjectCollide {
        public PhysicalObject po;
        public Pointer offset;
        public CollideType type;
		
        public Pointer off_modelstart;
        public ushort num_vertices;
        public ushort num_elements;
        public Pointer off_vertices;
        public Pointer off_normals = null;
        public Pointer off_element_types;
        public Pointer off_elements;
        public Pointer off_parallelBoxes;
        public ushort num_parallelBoxes;
        public Vector3 sphereCenter = Vector3.zero;
        public float sphereRadius = 0;

        public Vector3[] vertices = null;
        public Vector3[] normals = null;
        public ushort[] element_types = null;
        public IGeometricObjectElementCollide[] elements = null;

        public bool isBoundingVolume = false;

        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) InitGameObject();
                return gao;
            }
        }
        private void InitGameObject() {
            gao = new GameObject("Collide Set " + (type != CollideType.None ? type + " " : "") + "@ " + offset);
            gao.tag = "Collide";
            gao.layer = LayerMask.NameToLayer("Collide");

            for (uint i = 0; i < num_elements; i++) {
                if (elements[i] != null) {
                    /*System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
                    w.Start();*/
                    GameObject child = elements[i].Gao;
                    child.transform.SetParent(gao.transform);
                    child.transform.localPosition = Vector3.zero;


                    /*MapLoader.Loader.print("COL: " + elements[i].GetType() + " - " + w.ElapsedMilliseconds);
                    w.Stop();*/
                    /*if (m.subblocks[i] is CollideMeshElement) {
                        GameObject child = ((CollideMeshElement)m.subblocks[i]).Gao;
                        child.transform.SetParent(m.gao.transform);
                        child.transform.localPosition = Vector3.zero;
                    } else if (m.subblocks[i] is CollideSpheresElement) {
                        GameObject child = ((CollideSpheresElement)m.subblocks[i]).Gao;
                        child.transform.SetParent(m.gao.transform);
                        child.transform.localPosition = Vector3.zero;
                    } else if (m.subblocks[i] is CollideAlignedBoxesElement) {
                        GameObject child = ((CollideAlignedBoxesElement)m.subblocks[i]).Gao;
                        child.transform.SetParent(m.gao.transform);
                        child.transform.localPosition = Vector3.zero;
                    }*/
                }
            }
            SetVisualsActive(false); // Invisible by default
                                     //m.gao.SetActive(false); // Invisible by default
        }

        public GeometricObjectCollide(Pointer offset, CollideType type = CollideType.None) {
            this.offset = offset;
            this.type = type;
        }

        public void SetVisualsActive(bool active) {
			if (gao == null) return;
            Renderer[] renderers = gao.GetComponentsInChildren<Renderer>(includeInactive: true);
            foreach (Renderer ren in renderers) {
                ren.enabled = active;
            }
            /*if (subblocks != null) {
                foreach (ICollideGeometricElement subblock in subblocks) {
                    GameObject child = subblock.Gao;
                    if (child != null) {
                        Renderer mainRen = child.GetComponent<Renderer>();
                    }
                    //subblock.Gao
                }
            }*/
        }

        public bool ContainsPoint(Vector3 pos) {
            for (int i = 0; i < elements.Length; i++) {
                if (elements[i] == null) continue;
                switch (elements[i]) {
                    case GeometricObjectElementCollideAlignedBoxes box:
                        foreach (var b in box.boxes) {
                            Vector3 boxMin = vertices[b.minPoint];
                            Vector3 boxMax = vertices[b.maxPoint];
                            bool contains = pos.x >= boxMin.x && pos.x <= boxMax.x
                                && pos.y >= boxMin.y && pos.y <= boxMax.y
                                && pos.z >= boxMin.z && pos.z <= boxMax.z;
                            if (contains) return true;
                        }
                        break;
                    case GeometricObjectElementCollideSpheres sphere:
                        foreach (var s in sphere.spheres) {
                            Vector3 center = vertices[s.centerPoint];
                            bool contains = Vector3.Distance(pos, center) <= s.radius;
                            if (contains) return true;
                        }
                        break;
                    case GeometricObjectElementCollideTriangles tris:
                        break;
                }
            }
            return false;
        }

        public BoundingVolume BoundingBox {
            get {
                float xMin = vertices[0].x, xMax = vertices[0].x;
                float yMin = vertices[0].y, yMax = vertices[0].y;
                float zMin = vertices[0].z, zMax = vertices[0].z;
                void CheckVertex(Vector3 v) {
                    if (v.x < xMin) xMin = v.x;
                    if (v.y < yMin) yMin = v.y;
                    if (v.z < zMin) zMin = v.z;
                    if (v.x > xMax) xMax = v.x;
                    if (v.y > yMax) yMax = v.y;
                    if (v.z > zMax) zMax = v.z;
                }
                for (int i = 0; i < vertices.Length; i++) {
                    CheckVertex(vertices[i]);
                }
                for (int i = 0; i < elements.Length; i++) {
                    if (elements[i] == null) continue;
                    switch (elements[i]) {
                        case GeometricObjectElementCollideSpheres sphere:
                            foreach (var s in sphere.spheres) {
                                Vector3 center = vertices[s.centerPoint];
                                CheckVertex(center + Vector3.one * s.radius);
                                CheckVertex(center - Vector3.one * s.radius);
                            }
                            break;
                    }
                }
                Vector3 boxMin = new Vector3(xMin, yMin, zMin);
                Vector3 boxMax = new Vector3(xMax, yMax, zMax);
                return new BoundingVolume(null) {
                    type = BoundingVolume.Type.Box,
                    boxMin = boxMin,
                    boxMax = boxMax,
                    boxSize = boxMax - boxMin,
                    boxCenter = boxMin + (boxMax - boxMin) / 2,
                };
            }
        }

        public static GeometricObjectCollide Read(Reader reader, Pointer offset, CollideType type = CollideType.None, bool isBoundingVolume = false) {
            MapLoader l = MapLoader.Loader;
			//l.print("CollideMesh " + offset);
            GeometricObjectCollide m = new GeometricObjectCollide(offset, type);
            m.isBoundingVolume = isBoundingVolume;
            if (Settings.s.engineVersion == Settings.EngineVersion.R3 || Settings.s.game == Settings.Game.R2Revolution) {
                m.num_vertices = reader.ReadUInt16();
                m.num_elements = reader.ReadUInt16();
				if (Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.game != Settings.Game.LargoWinch) {
					reader.ReadUInt32();
				}
            }
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) m.num_vertices = (ushort)reader.ReadUInt32();
            m.off_vertices = Pointer.Read(reader);
            if (Settings.s.engineVersion < Settings.EngineVersion.R3 && Settings.s.game != Settings.Game.R2Revolution) {
                m.off_normals = Pointer.Read(reader);
                Pointer.Read(reader);
                reader.ReadInt32();
            }
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) m.num_elements = (ushort)reader.ReadUInt32();
            m.off_element_types = Pointer.Read(reader);
            m.off_elements = Pointer.Read(reader);
            if (Settings.s.game != Settings.Game.R2Revolution && Settings.s.game != Settings.Game.LargoWinch) {
                Pointer.Read(reader);
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    if (Settings.s.engineVersion == Settings.EngineVersion.R2) {
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        m.num_vertices = reader.ReadUInt16();
                        m.num_elements = reader.ReadUInt16();
                    }
                    if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) {
                        reader.ReadInt32();
                        reader.ReadInt32();
                    }
                    reader.ReadUInt16();
                    m.num_parallelBoxes = reader.ReadUInt16();
                } else {
                    //l.print((Pointer.Current(reader).FileOffset - offset.FileOffset));
                    m.off_parallelBoxes = Pointer.Read(reader);
                }
            } else {
                reader.ReadUInt32();
            }
            if (Settings.s.engineVersion != Settings.EngineVersion.Montreal) {
                m.sphereRadius = reader.ReadSingle(); // bounding volume radius
                float sphereX = reader.ReadSingle(); // x
                float sphereZ = reader.ReadSingle(); // z
                float sphereY = reader.ReadSingle(); // y
                m.sphereCenter = new Vector3(sphereX, sphereY, sphereZ);

                if (Settings.s.engineVersion == Settings.EngineVersion.R2) reader.ReadUInt32();
            }
            
            // Vertices
            Pointer off_current = Pointer.Goto(ref reader, m.off_vertices);
            m.vertices = new Vector3[m.num_vertices];
            for (int i = 0; i < m.num_vertices; i++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                m.vertices[i] = new Vector3(x, y, z);
            }

            // Normals
            if (m.off_normals != null) {
                off_current = Pointer.Goto(ref reader, m.off_normals);
                m.normals = new Vector3[m.num_vertices];
                for (int i = 0; i < m.num_vertices; i++) {
                    float x = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    m.normals[i] = new Vector3(x, y, z);
                }
            }
            // Read subblock types & initialize arrays
            Pointer.Goto(ref reader, m.off_element_types);
            m.element_types = new ushort[m.num_elements];
            m.elements = new IGeometricObjectElementCollide[m.num_elements];
            for (uint i = 0; i < m.num_elements; i++) {
                m.element_types[i] = reader.ReadUInt16();
            }

            for (uint i = 0; i < m.num_elements; i++) {
                Pointer.Goto(ref reader, m.off_elements + (i * 4));
                Pointer block_offset = Pointer.Read(reader);
                Pointer.Goto(ref reader, block_offset);
                switch (m.element_types[i]) {
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
                    case 1: // Collide submesh
                        m.elements[i] = GeometricObjectElementCollideTriangles.Read(reader, block_offset, m);
                        //material_i++;
                        break;
                    case 7:
                        m.elements[i] = GeometricObjectElementCollideSpheres.Read(reader, block_offset, m);
                        break;
                    case 8:
                        m.elements[i] = GeometricObjectElementCollideAlignedBoxes.Read(reader, block_offset, m);
                        break;
                    default:
                        m.elements[i] = null;
                        l.print("Unknown collide geometric element type " + m.element_types[i] + " at offset " + block_offset + " (Object: " + offset + ")");
                        break;
                }
            }
            return m;
        }

        public GeometricObjectCollide Clone() {
            GeometricObjectCollide m = (GeometricObjectCollide)MemberwiseClone();
            for (uint i = 0; i < m.num_elements; i++) {
                if (elements[i] != null) {
                    m.elements[i] = elements[i].Clone(m);
                }
            }
            return m;
        }
    }
}
