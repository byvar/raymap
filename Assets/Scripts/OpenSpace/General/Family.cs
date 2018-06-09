using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class Family {
        public Pointer offset;
        public Pointer off_family_next;
        public Pointer off_family_prev;
        public Pointer off_family_unk; // at this offset, start and end pointers appear again
        public uint family_index;
        public Pointer off_states_first;
        public Pointer off_states_last;
        public uint num_states;
        public Pointer off_preloadAnim_first = null; // (0x10 blocks: next, prev, list end, a3d pointer)
        public Pointer off_preloadAnim_last = null;
        public uint num_preloadAnim = 0;
        public Pointer off_physical_list;
        public Pointer off_bounding_volume;
        public Pointer off_vector4s;
        public uint num_vector4s;
        public byte animBank;
        public byte properties;

        public State[] states = null;
        public PhysicalObject[] physical_objects = null;

        public string name;
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("[Family] " + name);
                }
                return gao;
            }
        }

        public Family(Pointer offset) {
            this.offset = offset;
        }

        public static Family Read(EndianBinaryReader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Family f = new Family(offset);
            f.off_family_next = Pointer.Read(reader);
            f.off_family_prev = Pointer.Read(reader);
            f.off_family_unk = Pointer.Read(reader); // at this offset, start and end pointers appear again
            f.family_index = reader.ReadUInt32();
            f.name = l.objectTypes[0][f.family_index].name;
            f.off_states_first = Pointer.Read(reader);
            if (l.mode != MapLoader.Mode.RaymanArenaGC) f.off_states_last = Pointer.Read(reader);
            f.num_states = reader.ReadUInt32();
            if (l.mode != MapLoader.Mode.Rayman2PC) {
                f.off_preloadAnim_first = Pointer.Read(reader); // (0x10 blocks: next, prev, list end, a3d pointer)
                if (l.mode != MapLoader.Mode.RaymanArenaGC) f.off_preloadAnim_last = Pointer.Read(reader);
                f.num_preloadAnim = reader.ReadUInt32();
            }
            f.off_physical_list = Pointer.Read(reader); // Default objects table
            Pointer.Read(reader);                       // Current objects table
            if(l.mode != MapLoader.Mode.RaymanArenaGC) Pointer.Read(reader); // same address always?
            reader.ReadUInt32(); // always 1?
            f.off_bounding_volume = Pointer.Read(reader);
            if (l.mode == MapLoader.Mode.Rayman3GC || l.mode == MapLoader.Mode.Rayman3PC) {
                f.off_vector4s = Pointer.Read(reader);
                f.num_vector4s = reader.ReadUInt32();
                reader.ReadUInt32();
            }
            if (l.mode == MapLoader.Mode.Rayman2PC) {
                reader.ReadUInt32();
                f.animBank = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                f.properties = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            } else {
                reader.ReadUInt32();
                reader.ReadByte();
                reader.ReadByte();
                f.animBank = reader.ReadByte();
                f.properties = reader.ReadByte();
            }

            f.states = new State[f.num_states];
            if (f.num_states > 0) {
                Pointer off_states_current = f.off_states_first;
                for (int i = 0; i < f.num_states; i++) {
                    Pointer.Goto(ref reader, off_states_current);
                    f.states[i] = State.Read(reader, off_states_current, f);
                    if (l.mode == MapLoader.Mode.RaymanArenaGC) {
                        off_states_current = f.states[i].offset + 0x28;
                    } else {
                        off_states_current = f.states[i].off_state_next;
                    }
                }
            }

            if (f.off_physical_list != null) {
                Pointer.Goto(ref reader, f.off_physical_list);
                if (l.mode == MapLoader.Mode.Rayman3GC) {
                    reader.ReadUInt32(); // 0
                    reader.ReadUInt32(); // 0
                    Pointer off_list_hdr_ptr = Pointer.Read(reader);
                } else if (l.mode == MapLoader.Mode.Rayman3PC || l.mode == MapLoader.Mode.RaymanArenaPC) {
                    reader.ReadUInt32(); // 0
                } else if (l.mode == MapLoader.Mode.Rayman2PC) {
                    Pointer off_list_hdr_ptr = Pointer.Read(reader);
                }
                Pointer off_list_start = Pointer.Read(reader);
                Pointer off_list_2 = Pointer.Read(reader); // is this a copy of the list or something?
                ushort num_entries = reader.ReadUInt16();
                reader.ReadUInt16();
                if (l.mode == MapLoader.Mode.Rayman3PC || l.mode == MapLoader.Mode.Rayman3GC) {
                    Pointer off_list_hdr_1 = Pointer.Read(reader); // copy of off_subblocklist?
                    Pointer off_list_hdr_2 = Pointer.Read(reader); // same?
                    reader.ReadUInt32(); // 1?
                }
                Pointer.Goto(ref reader, off_list_start);
                f.physical_objects = new PhysicalObject[num_entries];
                for (uint i = 0; i < num_entries; i++) {
                    // each entry is 0x14
                    Pointer off1 = Pointer.Read(reader);
                    Pointer off_subblock = Pointer.Read(reader);
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    uint lastvalue = reader.ReadUInt32();
                    if (lastvalue != 0 && off_subblock != null) {

                        Pointer curPos = Pointer.Goto(ref reader, off_subblock);
                        PhysicalObject subobj = PhysicalObject.Read(reader, off_subblock);
                        if (subobj != null) {
                            f.physical_objects[i] = subobj;
                            subobj.Gao.transform.parent = f.Gao.transform;
                            /*if (subobj.visualSet.Count > 0 && subobj.visualSet[0].obj is R3Mesh) {
                                GameObject meshGAO = ((R3Mesh)subobj.visualSet[0].obj).gao;
                                meshGAO.transform.parent = f.Gao.transform;
                            }*/
                        }
                        Pointer.Goto(ref reader, curPos);
                    }
                }

                /*for (uint i = 0; i < num_entries; i++) {
                    R3PhysicalObject o = subblocks[i];
                    if (o != null && o.visualSet.Count > 0 && o.visualSet[0].obj is R3Unknown) {
                        R3Unknown a = (R3Unknown)o.visualSet[0].obj;
                        if (a.off_model != null) {
                            R3PhysicalObject po = subblocks.Where(
                                s => s != null
                                && s.visualSet.Count > 0
                                && s.visualSet[0].obj is R3Mesh
                                && ((R3Mesh)s.visualSet[0].obj).off_modelstart == a.off_model
                            ).FirstOrDefault();
                            if (po != null) {
                                ((R3Mesh)po.visualSet[0].obj).listUnknown.Add(a);
                                //((R3MeshObject)model).gao.name += "!";
                            }
                        }
                    }
                }*/
            }
            return f;
        }

        public static Family FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.families.FirstOrDefault(f => f.offset == offset);
        }
    }
}
