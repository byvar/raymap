using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.EngineObject {
    public class Sector : IEngineObject {

        public Pointer offset;
        public string name = "Sector";

        public LinkedList<Perso> persos;
        public LinkedList<LightInfo> lights;
        public LinkedList<NeighborSector> neighbors;
        public LinkedList<Sector> sectors_unk;

        public byte isSectorVirtual;

        public BoundingVolume sectorBorder;
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);
                }
                return gao;
            }
        }

        private SuperObject superObject;
        public SuperObject SuperObject {
            get { return superObject; }
        }

        private bool active = true;
        private bool neighborActive = true;
        public bool Active {
            get { return active; }
            set { active = value; }
        }
        public bool Loaded {
            get { return active || neighborActive; }
            set { neighborActive = value; }
        }


        public Sector(Pointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public void ProcessPointers(EndianBinaryReader reader) {
            MapLoader l = MapLoader.Loader;
            if (neighbors != null && neighbors.Count > 0) {
                neighbors.ReadEntries(reader, (EndianBinaryReader r, Pointer o) => {
                    NeighborSector n = new NeighborSector();
                    n.short0 = r.ReadUInt16();
                    n.short2 = r.ReadUInt16();
                    Pointer sp = Pointer.Read(r);
                    n.sector = l.sectors.FirstOrDefault(s => s.SuperObject.offset == sp);
                    if (l.mode == MapLoader.Mode.RaymanArenaGC) {
                        n.off_next = o + 8; // We just read 8 bytes
                    } else {
                        n.off_next = Pointer.Read(r);
                        if (l.mode == MapLoader.Mode.Rayman3GC) {
                            n.off_previous = Pointer.Read(r);
                            Pointer off_sector_start = Pointer.Read(r);
                        }
                    }
                    return n;
                });
            }
            if (persos != null && persos.Count > 0) {
                persos.ReadEntries(reader, (EndianBinaryReader r, Pointer o) => {
                    return l.persos.FirstOrDefault(p => p.SuperObject != null && p.SuperObject.offset == o);
                }, LinkedList.Flags.ElementPointerFirst);
            }
        }

        public static Sector Read(EndianBinaryReader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            Sector s = new Sector(offset, so);
            s.persos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
            s.lights = LinkedList<LightInfo>.Read(reader, Pointer.Current(reader),
                (EndianBinaryReader r, Pointer o) => {
                    LightInfo li = LightInfo.Read(r, o);
                    if (li != null) li.containingSectors.Add(s);
                    return li;
                },
                flags: LinkedList.Flags.ElementPointerFirst
                    | LinkedList.Flags.ReadAtPointer
                    | ((l.mode == MapLoader.Mode.Rayman3GC) ?
                        LinkedList.Flags.HasHeaderPointers :
                        LinkedList.Flags.NoPreviousPointersForDouble),
                type: (l.mode == MapLoader.Mode.RaymanArenaGC) ? LinkedList.Type.SingleNoElementPointers : LinkedList.Type.Default
            );
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            s.neighbors = LinkedList<NeighborSector>.ReadHeader(reader, Pointer.Current(reader));
            reader.ReadUInt32();
            if (Settings.s.linkedListType == LinkedList.Type.Double) reader.ReadUInt32();
            reader.ReadUInt32();
            s.sectors_unk = LinkedList<Sector>.ReadHeader(reader, Pointer.Current(reader));
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();

            s.sectorBorder = BoundingVolume.Read(reader, Pointer.Current(reader), BoundingVolume.Type.Box);

            reader.ReadUInt32();
            s.isSectorVirtual = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadUInt32();
            reader.ReadByte();
            if (Settings.s.hasNames) {
                s.name = new string(reader.ReadChars(0x104));
                l.print(s.name);
            } else {
                s.name = "Sector @ " + offset;
            }
            /*if(num_subsectors_unk > 0 && off_subsector_unk_first != null) { // only for father sector
                R3Pointer off_subsector_next = off_subsector_unk_first;
                for (int i = 0; i < num_subsectors_unk; i++) {
                    R3Pointer.Goto(ref reader, off_subsector_next);
                    R3Pointer off_subsector = R3Pointer.Read(reader);
                    off_subsector_next = R3Pointer.Read(reader);
                    R3Pointer off_subsector_prev = R3Pointer.Read(reader);
                    R3Pointer off_sector_start = R3Pointer.Read(reader);
                    if (off_subsector != null) {
                        sect.neighborsPointers.Add(off_subsector);
                    }
                }
            }*/

            l.sectors.Add(s);
            return s;
        }

        public struct NeighborSector : ILinkedListEntry {
            public ushort short0;
            public ushort short2;
            public Sector sector;

            public Pointer off_next;
            public Pointer off_previous;

            public Pointer NextEntry { get { return off_next; } }
            public Pointer PreviousEntry { get { return off_previous; } }
        }
    }
}
