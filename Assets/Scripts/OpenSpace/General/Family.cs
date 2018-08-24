using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class Family : ILinkedListEntry {
        public Pointer offset;
        public Pointer off_family_next;
        public Pointer off_family_prev;
        public Pointer off_family_hdr; // at this offset, start and end pointers appear again
        public uint family_index;
        public LinkedList<State> states;
        public LinkedList<int> preloadAnim; // int is just a placeholder type, change to actual type when I finally read it
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
        
        public PhysicalObject[][] physical_objects = null;

        public string name;
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("[Family] " + name);
                    FamilyComponent component = gao.AddComponent<FamilyComponent>();
                    component.Init(this);
                }
                return gao;
            }
        }

        public Pointer NextEntry {
            get { return off_family_next; }
        }

        public Pointer PreviousEntry {
            get { return off_family_prev; }
        }

        public Family(Pointer offset) {
            this.offset = offset;
        }

        public int GetIndexOfPhysicalList(Pointer off_physicalList) {
            return Array.IndexOf(off_physical_lists, off_physicalList);
        }

        public void ReadNewPhysicalList(Reader reader, Pointer off_physicalList) {
            MapLoader l = MapLoader.Loader;
            Array.Resize(ref off_physical_lists, off_physical_lists.Length + 1);
            Array.Resize(ref physical_objects, physical_objects.Length + 1);
            off_physical_lists[off_physical_lists.Length - 1] = Pointer.Current(reader);
            Pointer off_list_hdr_next = null;
            if (l.mode != MapLoader.Mode.RaymanArenaGC) off_list_hdr_next = Pointer.Read(reader);
            if (Settings.s.hasLinkedListHeaderPointers) {
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
                physical_objects[physical_objects.Length-1] = new PhysicalObject[num_entries];
                for (uint j = 0; j < num_entries; j++) {
                    // each entry is 0x14
                    Pointer off_po_scale = Pointer.Read(reader);
                    Pointer off_po = Pointer.Read(reader);
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    uint lastvalue = reader.ReadUInt32();
                    if (lastvalue != 0 && off_po != null) {

                        Pointer curPos = Pointer.Goto(ref reader, off_po);
                        PhysicalObject po = PhysicalObject.Read(reader, off_po);
                        Vector3? scaleMultiplier = null;
                        if (off_po_scale != null) {
                            Pointer.Goto(ref reader, off_po_scale);
                            float x = reader.ReadSingle();
                            float z = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            scaleMultiplier = new Vector3(x, y, z);
                        }
                        if (po != null) {
                            physical_objects[physical_objects.Length-1][j] = po;
                            po.Gao.transform.parent = Gao.transform;
                            po.scaleMultiplier = scaleMultiplier;
                        }
                        Pointer.Goto(ref reader, curPos);
                    }
                }
            }
        }

        public static Family Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Family f = new Family(offset);
            f.off_family_next = Pointer.Read(reader);
            f.off_family_prev = Pointer.Read(reader);
            f.off_family_hdr = Pointer.Read(reader); // at this offset, start and end pointers appear again
            f.family_index = reader.ReadUInt32();
            f.name = l.objectTypes[0][f.family_index].name;

            int stateIndex = 0;
            f.states = LinkedList<State>.Read(ref reader, Pointer.Current(reader), (off_element) => {
                State s = State.Read(reader, off_element, f, stateIndex++);
                return s;
            });
            if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
                // (0x10 blocks: next, prev, list end, a3d pointer)
                f.preloadAnim = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader));
            }
            f.off_physical_list_default = Pointer.Read(reader); // Default objects table
            f.off_physical_list_first = Pointer.Read(reader);                       // first physical list
            if (Settings.s.linkedListType == LinkedList.Type.Double) f.off_physical_list_last = Pointer.Read(reader); // last physical list
            f.num_physical_lists = reader.ReadUInt32();
            if (f.off_physical_list_first == f.off_physical_list_last && f.num_physical_lists > 1) f.num_physical_lists = 1; // Correction for Rayman 2
            f.off_bounding_volume = Pointer.Read(reader);
            if (l.mode == MapLoader.Mode.Rayman3GC || l.mode == MapLoader.Mode.Rayman3PC) {
                f.off_vector4s = Pointer.Read(reader);
                f.num_vector4s = reader.ReadUInt32();
                reader.ReadUInt32();
            }
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
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
            //l.print(f.name + " - Anim bank: " + f.animBank + " - id: " + l.objectTypes[0][f.family_index].id);
            
            f.off_physical_lists = new Pointer[f.num_physical_lists]; // Offset for each list of POs
            f.physical_objects = new PhysicalObject[f.num_physical_lists][]; // Each list of POs. Each perso has zero/one of these lists and can switch between them.
            if (f.off_physical_list_first != null) {
                //if (f.off_physical_list_first.offset == 0x7029) f.off_physical_list_first.offset = 0x7659;
                Pointer.Goto(ref reader, f.off_physical_list_first);
                for (uint i = 0; i < f.num_physical_lists; i++) {
                    f.off_physical_lists[i] = Pointer.Current(reader);
                    Pointer off_list_hdr_next = null;
                    if(l.mode != MapLoader.Mode.RaymanArenaGC) off_list_hdr_next = Pointer.Read(reader);
                    if (Settings.s.hasLinkedListHeaderPointers) {
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
                            Pointer off_po_scale = Pointer.Read(reader);
                            Pointer off_po = Pointer.Read(reader);
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            uint lastvalue = reader.ReadUInt32();
                            if (lastvalue != 0 && off_po != null) {

                                Pointer curPos = Pointer.Goto(ref reader, off_po);
                                PhysicalObject po = PhysicalObject.Read(reader, off_po);
                                Vector3? scaleMultiplier = null;
                                if (off_po_scale != null) {
                                    Pointer.Goto(ref reader, off_po_scale);
                                    float x = reader.ReadSingle();
                                    float z = reader.ReadSingle();
                                    float y = reader.ReadSingle();
                                    scaleMultiplier = new Vector3(x, y, z);
                                }
                                if (po != null) {
                                    f.physical_objects[i][j] = po;
                                    po.Gao.transform.parent = f.Gao.transform;
                                    po.scaleMultiplier = scaleMultiplier;
                                }
                                Pointer.Goto(ref reader, curPos);
                            }
                        }
                    }
                    if (off_list_hdr_next != null) {
                        Pointer.Goto(ref reader, off_list_hdr_next);
                    } else {
                        if (l.mode != MapLoader.Mode.RaymanArenaGC) break;
                    }
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
