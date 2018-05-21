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
                //R3Loader.Loader.print("Collide set: " + po.off_collideSet + " - vol: " + po.off_visualBoundingVolume);

                /* Example:
                00000000
                00000000
                00000000
                00258BCC off mesh
                ------ this is off mesh. size of col geo obj = 0x2c ------
                000C     num vertices
                0001     num subblocks
                0000
                0000
                00258BF8 points after 4 floats. 3 floats * num vertices
                00258C88 subblock type pointer
                00258C8C subblock pointer
                00259078
                00000000
                400D895A float
                BD619B7A float
                BE5757CB float
                BDA67A34 float
                ----------------------- goto subblock header
                0016273C
                00258CB4 off triangles indices
                00258D20 normals, one for each vertex referenced by triangles
                0012     num triangles
                FFFF
                00000000
                00258DF8
                00258EA8 0x1d * 3*4
                00259004 0x1d * 4
                001D
                0000
                */
            }
            return po;
        }


        public R3PhysicalObject Clone() {
            R3PhysicalObject po = (R3PhysicalObject)MemberwiseClone();
            po.visualSet = new List<R3VisualSetLOD>();
            for (int i = 0; i < visualSet.Count; i++) {
                R3VisualSetLOD l = new R3VisualSetLOD();
                l.LODdistance = visualSet[i].LODdistance;
                l.off_data = visualSet[i].off_data;
                l.obj = visualSet[i].obj.Clone();
                po.visualSet.Add(l);
            }
            // Add collide set here too
            return po;
        }
    }
}
