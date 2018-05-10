using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3PhysicalObject : IEquatable<R3PhysicalObject> {
        public R3Pointer off_header;
        public R3Pointer off_data;
        public R3PhysicalObject(R3Pointer off_header, R3Pointer off_data) {
            this.off_header = off_header;
            this.off_data = off_data;
        }
        public override bool Equals(System.Object obj) {
            return obj is R3PhysicalObject && this == (R3PhysicalObject)obj;
        }
        public override int GetHashCode() {
            return off_header.GetHashCode();
        }

        public bool Equals(R3PhysicalObject other) {
            return this == (R3PhysicalObject)other;
        }

        public static bool operator ==(R3PhysicalObject x, R3PhysicalObject y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.off_header == y.off_header;
        }
        public static bool operator !=(R3PhysicalObject x, R3PhysicalObject y) {
            return !(x == y);
        }

        public static R3PhysicalObject Read(EndianBinaryReader reader, R3Pointer off_header) {
            R3Pointer off_visualSet = R3Pointer.Read(reader);
            R3Pointer off_collideSet = R3Pointer.Read(reader);
            R3Pointer off_visualBoundingVolume = R3Pointer.Read(reader);
            R3Pointer off_collideBoundingVolume = R3Pointer.Read(reader);

            // We'll only parse the visual set for now
            R3Pointer.Goto(ref reader, off_visualSet);
            reader.ReadUInt32(); // 0
            uint numberOfLOD = reader.ReadUInt16(); // never more than one in R3, so let's only parse the first LOD
            uint type = reader.ReadUInt16();
            for (uint i = 0; i < numberOfLOD; i++) {
                // if distance > the float at this offset, game engine uses next LOD if there is one
                R3Pointer off_LODDistance = R3Pointer.Read(reader);
                R3Pointer off_data = R3Pointer.Read(reader);
                reader.ReadUInt32(); // always 0?
                reader.ReadUInt32(); // always 0?
                if (off_data != null) {
                    R3Pointer.Goto(ref reader, off_data);
                    switch (type) {
                        case 0:
                            return R3Mesh.Read(reader, off_header, off_data);
                        case 1:
                            return R3Unknown.Read(reader, off_header, off_data);
                        default:
                            R3Loader.Loader.print("unknown type " + type + " at offset: " + off_header);
                            return null;
                    }
                }
            }
            return null;
        }
    }
}
