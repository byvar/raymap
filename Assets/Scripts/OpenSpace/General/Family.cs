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
        public Pointer off_physical_list_default;
        public Pointer off_physical_list_first = null;
        public Pointer off_physical_list_last = null;
        public Pointer[] off_physical_lists;
        public uint num_physical_lists;
        public Pointer off_bounding_volume;
        public Pointer off_vector4s;
        public uint num_vector4s;
        public byte animBank;
        public byte properties;

        public State[] states = null;
        public PhysicalObject[][] physical_objects = null;

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

        public int GetIndexOfPhysicalList(Pointer off_physicalList) {
            return Array.IndexOf(off_physical_lists, off_physicalList);
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
            f.off_physical_list_default = Pointer.Read(reader); // Default objects table
            f.off_physical_list_first = Pointer.Read(reader);                       // first physical list
            if(l.mode != MapLoader.Mode.RaymanArenaGC) f.off_physical_list_last = Pointer.Read(reader); // last physical list
            f.num_physical_lists = reader.ReadUInt32();
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
            
            f.off_physical_lists = new Pointer[f.num_physical_lists]; // Offset for each list of POs
            f.physical_objects = new PhysicalObject[f.num_physical_lists][]; // Each list of POs. Each perso has zero/one of these lists and can switch between them.
            if (f.off_physical_list_first != null) {
                Pointer.Goto(ref reader, f.off_physical_list_first);
                for (uint i = 0; i < f.num_physical_lists; i++) {
                    f.off_physical_lists[i] = Pointer.Current(reader);
                    Pointer off_list_hdr_next = Pointer.Read(reader);
                    if (l.mode == MapLoader.Mode.Rayman3GC) {
                        Pointer off_list_hdr_prev = Pointer.Read(reader);
                        Pointer off_list_hdr = Pointer.Read(reader);
                    }
                    Pointer off_list_start = Pointer.Read(reader);
                    Pointer off_list_2 = Pointer.Read(reader); // is this a copy of the list or something?
                    ushort num_entries = reader.ReadUInt16();
                    reader.ReadUInt16();


                    /*// format of list_hdr:
                    if (l.mode == MapLoader.Mode.Rayman3PC || l.mode == MapLoader.Mode.Rayman3GC) {
                        Pointer off_list_hdr_first = Pointer.Read(reader);
                        Pointer off_list_hdr_last = Pointer.Read(reader);
                        uint num_lists = reader.ReadUInt32(); // 1?
                    }*/
                    if (off_list_start != null) {
                        Pointer.Goto(ref reader, off_list_start);
                        f.physical_objects[i] = new PhysicalObject[num_entries];
                        for (uint j = 0; j < num_entries; j++) {
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
                                    f.physical_objects[i][j] = subobj;
                                    subobj.Gao.transform.parent = f.Gao.transform;
                                }
                                Pointer.Goto(ref reader, curPos);
                            }
                        }
                    }
                    if (off_list_hdr_next != null) Pointer.Goto(ref reader, off_list_hdr_next);
                }
            }

            /*if (l.mode == MapLoader.Mode.Rayman3GC) {
                Pointer off_list_hdr_next = Pointer.Read(reader);
                Pointer off_list_hdr_prev = Pointer.Read(reader);
                Pointer off_list_hdr = Pointer.Read(reader);
                //if (off_list_hdr != null) Pointer.Goto(ref reader, off_list_hdr);
            } else if (l.mode == MapLoader.Mode.Rayman3PC || l.mode == MapLoader.Mode.RaymanArenaPC) {
                reader.ReadUInt32(); // 0
            } else if (l.mode == MapLoader.Mode.Rayman2PC) {
                Pointer off_list_hdr = Pointer.Read(reader);
                //if (off_list_hdr != null) Pointer.Goto(ref reader, off_list_hdr);
            }
            if (l.mode == MapLoader.Mode.Rayman3PC || l.mode == MapLoader.Mode.Rayman3GC) {
                Pointer off_list_hdr_1 = Pointer.Read(reader); // copy of off_subblocklist?
                Pointer off_list_hdr_2 = Pointer.Read(reader); // same?
                reader.ReadUInt32(); // 1?
            }*/
            return f;
        }

        public static Family FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.families.FirstOrDefault(f => f.offset == offset);
        }
    }
}
