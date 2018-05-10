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
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            R3Pointer off_subblocklist = R3Pointer.Read(reader);
            reader.ReadUInt32(); // same address?
            R3Pointer off_rulelist = R3Pointer.Read(reader);

            if (off_nameIndices != null) {
                R3Pointer off_current = R3Pointer.Goto(ref reader, off_nameIndices);
                uint index0 = reader.ReadUInt32();
                uint index1 = reader.ReadUInt32();
                uint index2 = reader.ReadUInt32();
                p.name0 = l.names[0][index0];
                p.name1 = l.names[1][index1];
                p.name2 = l.names[2][index2];
                R3Pointer.Goto(ref reader, off_current);
            }
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
            if (off_subblocklist != null) {
                //print("subblocks for " + name + " @ " + String.Format("0x{0:X}", off_subblocklist.offset));
                R3Pointer.Goto(ref reader, off_subblocklist);
                if (l.mode == R3Loader.Mode.Rayman3GC) {
                    reader.ReadUInt32(); // 0
                    reader.ReadUInt32(); // 0
                    R3Pointer off_list_hdr_ptr = R3Pointer.Read(reader);
                } else if (l.mode == R3Loader.Mode.Rayman3PC || l.mode == R3Loader.Mode.RaymanArenaPC) {
                    reader.ReadUInt32(); // 0
                }
                R3Pointer off_list_start = R3Pointer.Read(reader);
                R3Pointer off_list_2 = R3Pointer.Read(reader); // is this a copy of the list or something?
                uint num_entries = reader.ReadUInt16();
                reader.ReadUInt16();
                if (l.mode == R3Loader.Mode.Rayman3PC || l.mode == R3Loader.Mode.Rayman3GC) {
                    R3Pointer off_list_hdr_1 = R3Pointer.Read(reader); // copy of off_subblocklist?
                    R3Pointer off_list_hdr_2 = R3Pointer.Read(reader); // same?
                    reader.ReadUInt32(); // 1?
                }
                R3Pointer.Goto(ref reader, off_list_start);
                R3PhysicalObject[] subblocks = new R3PhysicalObject[num_entries];
                for (uint i = 0; i < num_entries; i++) {
                    // each entry is 0x14
                    R3Pointer off1 = R3Pointer.Read(reader);
                    R3Pointer off_subblock = R3Pointer.Read(reader);
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    uint lastvalue = reader.ReadUInt32();
                    //if ((raymanArena || off1 == null) && lastvalue != 0 && off_subblock != null) {
                    if (lastvalue != 0 && off_subblock != null) {

                        R3Pointer curPos = R3Pointer.Goto(ref reader, off_subblock);
                        //print("Found mesh @ " + curPos.offset);
                        R3PhysicalObject subobj = R3PhysicalObject.Read(reader, off_subblock);
                        if (subobj != null) {
                            subblocks[i] = subobj;
                            if (subobj is R3Mesh) {
                                GameObject meshGAO = ((R3Mesh)subobj).gao;
                                meshGAO.transform.parent = p.Gao.transform;
                            } /*else if(subobj is R3AnimationObject) {
                            print(((R3AnimationObject)subobj).vector3s.Count);
                        }*/
                        }
                        R3Pointer.Goto(ref reader, curPos);
                    }
                }

                for (uint i = 0; i < num_entries; i++) {
                    R3PhysicalObject o = subblocks[i];
                    if (o != null && o is R3Unknown) {
                        R3Unknown a = (R3Unknown)o;
                        if (a.off_model != null) {
                            R3PhysicalObject model = subblocks.Where(s => s != null && s is R3Mesh && ((R3Mesh)s).off_modelstart == a.off_model).FirstOrDefault();
                            if (model != null && model is R3Mesh) {
                                ((R3Mesh)model).listUnknown.Add(a);
                                //((R3MeshObject)model).gao.name += "!";
                            }
                        }
                    }
                }
            } /*else {
                print(String.Format("0x{0:X}", off_perso.offset));
            }*/
            return p;
        }
    }
}
