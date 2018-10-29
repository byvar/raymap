using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    public class MeshModificationObject : IGeometricObject {
        public PhysicalObject po;
        public Pointer offset;

        public Pointer off_model;
        public uint num_properties;
        public Pointer off_properties;

        public MeshObject mesh = null;
        public MeshModificationProperty[] properties;


        public GameObject Gao {
            get {
                if (mesh != null) return mesh.Gao;
                return null;
            }
        }

        public MeshModificationObject(PhysicalObject po, Pointer offset) {
            this.po = po;
            this.offset = offset;
        }

        // I don't even know what this is yet here I am parsing it
        public static MeshModificationObject Read(Reader reader, PhysicalObject po, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            //Debug.LogWarning("Unknown object @ " + offset);
            MeshModificationObject mod = new MeshModificationObject(po, offset);
            mod.off_model = Pointer.Read(reader);
            /*Pointer.DoAt(ref reader, mod.off_model, () => {
                mod.mesh = MeshObject.Read(reader, po, mod.off_model);
            });*/

            mod.num_properties = reader.ReadUInt32();
            mod.off_properties = Pointer.Read(reader);
            Pointer.DoAt(ref reader, mod.off_properties, () => {
                mod.properties = new MeshModificationProperty[mod.num_properties];
                for (int i = 0; i < mod.num_properties; i++) {
                    mod.properties[i] = new MeshModificationProperty();
                    mod.properties[i].id = reader.ReadUInt16();
                    mod.properties[i].unk = reader.ReadUInt16();
                    float x = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    mod.properties[i].pos = new Vector3(x, y, z);
                }
            });
            return mod;
        }

        public IGeometricObject Clone() {
            MeshModificationObject lodObj = (MeshModificationObject)MemberwiseClone();
            return lodObj;
        }
    }
}
