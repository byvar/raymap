using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// Also called "Actor" in the code which might be a better name, but I'll stick to the R2 one for now
    /// </summary>
    public class R3Perso : IR3Data {
        public R3Pointer offset;
        public string fullName = "Perso";
        public string name0 = null;
        public string name1 = null;
        public string name2 = null;
        public R3Family family = null;
        public R3PhysicalObject[] physical_objects = null;

        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    if (name0 != null && name1 != null && name2 != null) {
                        fullName = "[" + name0 + "] " + name1 + " | " + name2;
                        /*if (superObject != null) {
                            fullName = superObject.matrix.type + " " + superObject.matrix.v + fullName;
                        }*/
                    }
                    gao = new GameObject(fullName);
                }
                return gao;
            }
        }

        private R3SuperObject superObject;
        public R3SuperObject SuperObject {
            get {
                return superObject;
            }
        }

        public R3Perso(R3Pointer offset, R3SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public static R3Perso Read(EndianBinaryReader reader, R3Pointer offset, R3SuperObject so) {
            R3Loader l = R3Loader.Loader;
            R3Perso p = new R3Perso(offset, so);
            R3Pointer off_perso = R3Pointer.Read(reader);
            R3Pointer off_nameIndices = R3Pointer.Read(reader);
            R3Pointer off_unknown = R3Pointer.Read(reader);
            R3Pointer off_intelligence = R3Pointer.Read(reader);
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (l.mode == R3Loader.Mode.RaymanArenaPC || l.mode == R3Loader.Mode.RaymanArenaGC) reader.ReadUInt32();
            //R3Pointer.Goto(ref reader, off_perso);
            R3Pointer.Read(reader); // same as next
            R3Pointer off_currentState = R3Pointer.Read(reader);
            R3Pointer.Read(reader); // same as previous
            R3Pointer off_subblocklist = R3Pointer.Read(reader);
            reader.ReadUInt32(); // same address?
            R3Pointer off_family = R3Pointer.Read(reader);
            p.family = R3Family.FromOffset(off_family);

            if (off_nameIndices != null) {
                R3Pointer off_current = R3Pointer.Goto(ref reader, off_nameIndices);
                uint index0 = reader.ReadUInt32();
                uint index1 = reader.ReadUInt32();
                uint index2 = reader.ReadUInt32();
                p.name0 = l.objectTypes[0][index0].name;
                p.name1 = l.objectTypes[1][index1].name;
                p.name2 = l.objectTypes[2][index2].name;
                R3Pointer.Goto(ref reader, off_current);
            }
            l.print("[" + p.name0 + "] " + p.name1 + " | " + p.name2 + " - offset: " + offset);
            /*if (off_intelligence != null) {
                R3Pointer.Goto(ref reader, off_intelligence);
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                R3Pointer off_name = R3Pointer.Read(reader);
                if (off_name != null) {
                    R3Pointer.Goto(ref reader, off_name);
                    name = reader.ReadNullDelimitedString();
                }
            }*/
            if (off_subblocklist != null && p.family != null && off_subblocklist == p.family.off_physical_list && p.family.physical_objects != null) {
                // Clone family's physical objects into this perso
                p.physical_objects = new R3PhysicalObject[p.family.physical_objects.Length];
                for (int i = 0; i < p.family.physical_objects.Length; i++) {
                    R3PhysicalObject o = p.family.physical_objects[i];
                    if (o != null) {
                        p.physical_objects[i] = o.Clone();
                        p.physical_objects[i].Gao.transform.parent = p.Gao.transform;
                        /*if (p.physical_objects[i].visualSet.Count > 0 && p.physical_objects[i].visualSet[0].obj is R3Mesh) {
                            GameObject meshGAO = ((R3Mesh)p.physical_objects[i].visualSet[0].obj).gao;
                            meshGAO.transform.parent = p.Gao.transform;
                        }*/
                    }
                }
            } else if (off_subblocklist != null) {
                l.print("Perso's physical list does not match family list at position " + offset);
            }
            return p;
        }
    }
}
