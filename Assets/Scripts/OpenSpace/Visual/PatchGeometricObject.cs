using Newtonsoft.Json;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    public class PatchGeometricObject : IGeometricObject {
		[JsonIgnore] public PhysicalObject po;
        public Pointer offset;
		
        public Pointer off_geometricObject;
        public uint num_properties;
        public Pointer off_properties;

        [JsonIgnore] public GeometricObject mesh = null;
        public PatchGeometricObjectProperty[] properties;


        public GameObject Gao {
            get {
                //if (mesh != null) return mesh.Gao;
                return null;
            }
        }

        public PatchGeometricObject(PhysicalObject po, Pointer offset) {
            this.po = po;
            this.offset = offset;
        }

        // I don't even know what this is yet here I am parsing it
        public static PatchGeometricObject Read(Reader reader, PhysicalObject po, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            //Debug.LogWarning("Unknown object @ " + offset);
            PatchGeometricObject patch = new PatchGeometricObject(po, offset);
            patch.off_geometricObject = Pointer.Read(reader);

            patch.num_properties = reader.ReadUInt32();
            patch.off_properties = Pointer.Read(reader);
            Pointer.DoAt(ref reader, patch.off_properties, () => {
                patch.properties = new PatchGeometricObjectProperty[patch.num_properties];
                for (int i = 0; i < patch.num_properties; i++) {
                    patch.properties[i] = new PatchGeometricObjectProperty();
                    patch.properties[i].ind_vertex = reader.ReadUInt16();
                    patch.properties[i].unk = reader.ReadUInt16();
                    float x = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    patch.properties[i].pos = new Vector3(x, y, z);
                    //l.print(mod.off_geometricObject + ": " + mod.properties[i].ind_vertex + " - " + mod.properties[i].unk + " - " + mod.properties[i].pos);
                }
            });
            /*Pointer.DoAt(ref reader, mod.off_geometricObject, () => {
                mod.mesh = GeometricObject.Read(reader, mod.off_geometricObject);
                mod.mesh.Gao.name += " - " + mod.offset;
            });*/
            return patch;
        }

        public IGeometricObject Clone() {
            PatchGeometricObject lodObj = (PatchGeometricObject)MemberwiseClone();
            return lodObj;
        }
    }
}
