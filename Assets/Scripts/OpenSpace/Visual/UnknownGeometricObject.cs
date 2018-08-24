using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    public class UnknownGeometricObject : IGeometricObject {
        public PhysicalObject po;
        public Pointer offset;

        public Pointer off_model;
        public List<ushort> ids;
        public List<Vector3> vector3s;
        public UnknownGeometricObject(PhysicalObject po, Pointer offset) {
            this.po = po;
            this.offset = offset;
            ids = new List<ushort>();
            vector3s = new List<Vector3>();
        }

        // I don't even know what this is yet here I am parsing it
        public static UnknownGeometricObject Read(Reader reader, PhysicalObject po, Pointer offset) {
            UnknownGeometricObject unk = new UnknownGeometricObject(po, offset);
            unk.off_model = Pointer.Read(reader);
            uint num_vector3s = reader.ReadUInt32();
            Pointer off_arrayStart = Pointer.Read(reader);
            if (off_arrayStart != null) {
                Pointer.Goto(ref reader, off_arrayStart);
                for (int i = 0; i < num_vector3s; i++) {
                    ushort id = reader.ReadUInt16();
                    reader.ReadUInt16();
                    float x = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    unk.ids.Add(id);
                    unk.vector3s.Add(new Vector3(x, y, z));
                }
            }
            return unk;
        }

        public IGeometricObject Clone() {
            UnknownGeometricObject lodObj = (UnknownGeometricObject)MemberwiseClone();
            return lodObj;
        }
    }
}
