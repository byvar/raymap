using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3Unknown : R3PhysicalObject {
        public R3Pointer off_model;
        public List<Vector3> vector3s;
        public R3Unknown(R3Pointer off_header, R3Pointer off_data) : base(off_header, off_data) {
            vector3s = new List<Vector3>();
        }

        // I don't even know what this is yet here I am parsing it
        public static R3Unknown Read(EndianBinaryReader reader, R3Pointer off_header, R3Pointer off_data) {
            R3Unknown lodObj = new R3Unknown(off_header, off_data);
            R3Pointer off_start = R3Pointer.Read(reader);
            R3Pointer.Goto(ref reader, off_start);
            lodObj.off_model = R3Pointer.Read(reader);
            uint num_vector3s = reader.ReadUInt32();
            R3Pointer off_arrayStart = R3Pointer.Read(reader);
            if (off_arrayStart != null) {
                R3Pointer.Goto(ref reader, off_arrayStart);
                for (int i = 0; i < num_vector3s; i++) {
                    uint id = reader.ReadUInt16();
                    reader.ReadUInt16();
                    float x = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    lodObj.vector3s.Add(new Vector3(x, y, z));
                }
            }
            return lodObj;
        }
    }
}
