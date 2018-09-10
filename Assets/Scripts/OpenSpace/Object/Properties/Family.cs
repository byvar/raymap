using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object.Properties {
    public class Family : ILinkedListEntry {
        public Pointer offset;
        public Pointer off_family_next;
        public Pointer off_family_prev;
        public Pointer off_family_hdr; // at this offset, start and end pointers appear again
        public uint family_index;
        public LinkedList<State> states;
        public LinkedList<int> preloadAnim; // int is just a placeholder type, change to actual type when I finally read it
        public Pointer off_physical_list_default;
        public LinkedList<ObjectList> objectLists;
        public Pointer off_bounding_volume;
        public Pointer off_vector4s;
        public uint num_vector4s;
        public byte animBank;
        public byte properties;

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
            for(int i = 0; i < objectLists.Count; i++) {
                ObjectList ol = objectLists[i];
                if (ol != null && ol.offset == off_physicalList) return i;
            }
            return -1;
        }

        public void AddNewPhysicalList(ObjectList ol) {
            if(!objectLists.Contains(ol)) objectLists.Add(ol);
        }

        public static Family Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Family f = new Family(offset);
            f.off_family_next = Pointer.Read(reader);
            f.off_family_prev = Pointer.Read(reader);
            f.off_family_hdr = Pointer.Read(reader); // at this offset, start and end pointers appear again
            f.family_index = reader.ReadUInt32();
            f.name = l.objectTypes[0][f.family_index].name;
            //l.print("(" + f.family_index + ") " + f.name + " - " + offset);
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
            f.objectLists = LinkedList<ObjectList>.ReadHeader(reader, Pointer.Current(reader));
            if (f.objectLists.off_head == f.objectLists.off_tail && f.objectLists.Count > 1) f.objectLists.Count = 1; // Correction for Rayman 2
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

            f.objectLists.ReadEntries(ref reader, (off_list) => {
                ObjectList ol = ObjectList.FromOffsetOrRead(off_list, reader);
                if (ol.containingFamilies.Count == 0) {
                    ol.Gao.transform.SetParent(f.Gao.transform);
                }
                if(!ol.containingFamilies.Contains(f)) ol.containingFamilies.Add(f);
                return ol;
            });

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
