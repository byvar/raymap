using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3PhysicalObject : IEquatable<R3PhysicalObject> {
        public R3Pointer offset;
        public R3Pointer off_visualSet;
        public R3Pointer off_collideSet;
        public R3Pointer off_visualBoundingVolume;
        public R3Pointer off_collideBoundingVolume;
        public List<R3VisualSetLOD> visualSet;
        public R3CollideMesh collideMesh;
        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("[PO]");
                }
                return gao;
            }
        }

        public R3PhysicalObject(R3Pointer offset) {
            this.offset = offset;
            visualSet = new List<R3VisualSetLOD>();
        }
        public override bool Equals(System.Object obj) {
            return obj is R3PhysicalObject && this == (R3PhysicalObject)obj;
        }
        public override int GetHashCode() {
            return offset.GetHashCode();
        }

        public bool Equals(R3PhysicalObject other) {
            return this == (R3PhysicalObject)other;
        }

        public static bool operator ==(R3PhysicalObject x, R3PhysicalObject y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.offset == y.offset;
        }
        public static bool operator !=(R3PhysicalObject x, R3PhysicalObject y) {
            return !(x == y);
        }

        public static R3PhysicalObject Read(EndianBinaryReader reader, R3Pointer offset) {
            R3PhysicalObject po = new R3PhysicalObject(offset);

            // Header
            po.off_visualSet = R3Pointer.Read(reader);
            po.off_collideSet = R3Pointer.Read(reader);
            po.off_visualBoundingVolume = R3Pointer.Read(reader);
            po.off_collideBoundingVolume = R3Pointer.Read(reader);

            // Parse visual set
            if (po.off_visualSet != null) {
                R3Pointer.Goto(ref reader, po.off_visualSet);
                reader.ReadUInt32(); // 0
                ushort numberOfLOD = reader.ReadUInt16();
                ushort type = reader.ReadUInt16();
                for (uint i = 0; i < numberOfLOD; i++) {
                    // if distance > the float at this offset, game engine uses next LOD if there is one
                    R3VisualSetLOD lod = new R3VisualSetLOD();
                    R3Pointer off_LODDistance = R3Pointer.Read(reader);
                    lod.off_data = R3Pointer.Read(reader);
                    reader.ReadUInt32(); // always 0?
                    reader.ReadUInt32(); // always 0?
                    if (off_LODDistance != null) {
                        R3Pointer off_current = R3Pointer.Goto(ref reader, off_LODDistance);
                        lod.LODdistance = reader.ReadSingle();
                        R3Pointer.Goto(ref reader, off_current);
                    }
                    if (lod.off_data != null) {
                        R3Pointer off_current = R3Pointer.Goto(ref reader, lod.off_data);
                        switch (type) {
                            case 0:
                                lod.obj = R3Mesh.Read(reader, po, lod.off_data);
                                R3Mesh m = ((R3Mesh)lod.obj);
                                if (m.name != "Mesh") po.Gao.name = "[PO] " + m.name;
                                m.gao.transform.parent = po.Gao.transform;
                                break;
                            case 1:
                                lod.obj = R3Unknown.Read(reader, po, lod.off_data);
                                break;
                            default:
                                R3Loader.Loader.print("unknown type " + type + " at offset: " + offset);
                                break;
                        }
                        R3Pointer.Goto(ref reader, off_current);
                    }
                    po.visualSet.Add(lod);
                }
            }

            // Parse collide set
            if (po.off_collideSet != null) {
                R3Pointer.Goto(ref reader, po.off_collideSet);
                uint u1 = reader.ReadUInt32(); // 0
                uint u2 = reader.ReadUInt32(); // 0
                uint u3 = reader.ReadUInt32(); // 0
                R3Pointer off_mesh = R3Pointer.Read(reader);
                if (off_mesh != null) {
                    R3Pointer.Goto(ref reader, off_mesh);
                    po.collideMesh = R3CollideMesh.Read(reader, po, off_mesh);
                    po.collideMesh.gao.transform.parent = po.Gao.transform;
                }
                //R3Loader.Loader.print("Collide set: " + po.off_collideSet + " - vol: " + po.off_visualBoundingVolume);
            }
            R3Loader.Loader.physicalObjects.Add(po);
            return po;
        }

        // Call after clone
        public void Reset() {
            gao = null;
        }

        public R3PhysicalObject Clone() {
            R3PhysicalObject po = (R3PhysicalObject)MemberwiseClone();
            po.visualSet = new List<R3VisualSetLOD>();
            po.Reset();
            for (int i = 0; i < visualSet.Count; i++) {
                R3VisualSetLOD l = new R3VisualSetLOD();
                l.LODdistance = visualSet[i].LODdistance;
                l.off_data = visualSet[i].off_data;
                l.obj = visualSet[i].obj.Clone();
                po.visualSet.Add(l);
                if (l.obj is R3Mesh) {
                    R3Mesh m = ((R3Mesh)l.obj);
                    if (m.name != "Mesh") po.Gao.name = "[PO] " + m.name;
                    m.gao.transform.parent = po.Gao.transform;
                }
            }
            if (collideMesh != null) {
                po.collideMesh = collideMesh.Clone();
                po.collideMesh.gao.transform.parent = po.Gao.transform;
            }
            R3Loader.Loader.physicalObjects.Add(po);
            return po;
        }
    }
}
