using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.EngineObject {
    /// <summary>
    /// Also called "Actor" in the code which might be a better name, but I'll stick to the R2 one for now
    /// </summary>
    public class Perso : IEngineObject {
        public Pointer offset;
        public string fullName = "Perso";
        public string name0 = null;
        public string name1 = null;
        public string name2 = null;
        public Family family = null;
        public PhysicalObject[] physical_objects = null;
        public Brain brain = null;

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

        private SuperObject superObject;
        public SuperObject SuperObject {
            get {
                return superObject;
            }
        }

        public Perso(Pointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public static Perso Read(EndianBinaryReader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            //l.print("Offset: " + offset);
            Perso p = new Perso(offset, so);
            Pointer off_perso = Pointer.Read(reader);
            Pointer off_nameIndices = Pointer.Read(reader);
            Pointer off_unknown = Pointer.Read(reader);
            Pointer off_brain = Pointer.Read(reader);
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (l.mode == MapLoader.Mode.RaymanArenaPC || l.mode == MapLoader.Mode.RaymanArenaGC) reader.ReadUInt32();
            if (l.mode == MapLoader.Mode.Rayman2PC) {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            //R3Pointer.Goto(ref reader, off_perso);
            Pointer.Read(reader); // same as next
            Pointer off_currentState = Pointer.Read(reader);
            Pointer.Read(reader); // same as previous
            Pointer off_subblocklist = Pointer.Read(reader);
            reader.ReadUInt32(); // same address?
            Pointer off_family = Pointer.Read(reader);
            p.family = Family.FromOffset(off_family);

            if (off_nameIndices != null) {
                Pointer off_current = Pointer.Goto(ref reader, off_nameIndices);
                uint index0 = reader.ReadUInt32();
                uint index1 = reader.ReadUInt32();
                uint index2 = reader.ReadUInt32();
                p.name0 = l.objectTypes[0][index0].name;
                p.name1 = l.objectTypes[1][index1].name;
                p.name2 = l.objectTypes[2][index2].name;
                Pointer.Goto(ref reader, off_current);
            }
            l.print("[" + p.name0 + "] " + p.name1 + " | " + p.name2 + " - offset: " + offset);

            if (off_brain != null) {
                Pointer off_current = Pointer.Goto(ref reader, off_brain);
                p.brain = Brain.Read(reader, off_brain);
                Pointer.Goto(ref reader, off_current);
            }
            //if (off_intelligence != null) l.print("Intelligence for " + p.name2 + ": " + off_intelligence);
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
                p.physical_objects = new PhysicalObject[p.family.physical_objects.Length];
                for (int i = 0; i < p.family.physical_objects.Length; i++) {
                    PhysicalObject o = p.family.physical_objects[i];
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
