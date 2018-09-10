using OpenSpace.Collide;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object {
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

        public void ProcessPointers(Reader reader) {
            MapLoader l = MapLoader.Loader;
            if (neighbors != null && neighbors.Count > 0) {
                neighbors.ReadEntries(ref reader, (off_element) => {
                    NeighborSector n = new NeighborSector();
                    n.short0 = reader.ReadUInt16();
                    n.short2 = reader.ReadUInt16();
                    Pointer sp = Pointer.Read(reader);
                    n.sector = l.sectors.FirstOrDefault(s => s.SuperObject.offset == sp);
                    if (l.mode == MapLoader.Mode.RaymanArenaGC) {
                        n.off_next = off_element + 8; // No next pointer, each entry is immediately after the first one.
                    } else {
                        n.off_next = Pointer.Read(reader);
                        if (Settings.s.hasLinkedListHeaderPointers) {
                            n.off_previous = Pointer.Read(reader);
                            Pointer off_sector_start = Pointer.Read(reader);
                        }
                    }
                    return n;
                });
            }
            if (persos != null && persos.Count > 0) {
                persos.ReadEntries(ref reader, (off_element) => {
                    return l.persos.FirstOrDefault(p => p.SuperObject != null && p.SuperObject.offset == off_element);
                }, LinkedList.Flags.ElementPointerFirst);
            }
        }

        public static Sector Read(Reader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            Sector s = new Sector(offset, so);
            s.name = "Sector @ " + offset;
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) {
                if (Settings.s.game == Settings.Game.TTSE) reader.ReadUInt32(); // always 1 or 0. whether the sector is active or not?
                Pointer off_collideObj = Pointer.Read(reader);
                Pointer.DoAt(ref reader, off_collideObj, () => {
                    //CollideMeshObject collider = CollideMeshObject.Read(reader, off_collideObj);
                    // This has the exact same structure as a CollideMeshObject but with a sector superobject as material for the collieMeshElements
                });
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double); // only one sector pointer in this list
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double); // ??? always null?
            }
            s.persos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
            s.lights = LinkedList<LightInfo>.Read(ref reader, Pointer.Current(reader),
                (off_element) => {
                    LightInfo li = LightInfo.Read(reader, off_element);
                    if (li != null) li.containingSectors.Add(s);
                    return li;
                },
                flags: LinkedList.Flags.ElementPointerFirst
                    | LinkedList.Flags.ReadAtPointer
                    | ((Settings.s.hasLinkedListHeaderPointers) ?
                        LinkedList.Flags.HasHeaderPointers :
                        LinkedList.Flags.NoPreviousPointersForDouble),
                type: (l.mode == MapLoader.Mode.RaymanArenaGC) ? LinkedList.Type.SingleNoElementPointers : LinkedList.Type.Default
            );
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            s.neighbors = LinkedList<NeighborSector>.ReadHeader(reader, Pointer.Current(reader));
            LinkedList<Sector>.ReadHeader(reader, Pointer.Current(reader));
            s.sectors_unk = LinkedList<Sector>.ReadHeader(reader, Pointer.Current(reader));

            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();

            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();

            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                s.sectorBorder = BoundingVolume.Read(reader, Pointer.Current(reader), BoundingVolume.Type.Box);

                reader.ReadUInt32();
                s.isSectorVirtual = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt32();
                reader.ReadByte();
                if (Settings.s.hasNames) {
                    s.name = reader.ReadString(0x104);
                    l.print(s.name);
                }
            } else {
                if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                }
                if(Settings.s.game != Settings.Game.TTSE) reader.ReadUInt32();
                Pointer off_name = Pointer.Read(reader);
                Pointer.DoAt(ref reader, off_name, () => {
                    s.name = reader.ReadNullDelimitedString();
                });
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
