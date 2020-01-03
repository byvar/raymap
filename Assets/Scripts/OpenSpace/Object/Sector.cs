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
        public LinkedList<LightInfo> staticLights;
        public LinkedList<int> dynamicLights; // Stub
        public LinkedList<NeighborSector> neighbors;
        public LinkedList<NeighborSector> sectors_unk1;
        public LinkedList<Sector> sectors_unk2;

        public byte isSectorVirtual;
        public Pointer off_skyMaterial;
        public VisualMaterial skyMaterial;

        public BoundingVolume sectorBorder;
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);
					SectorComponent sc = gao.AddComponent<SectorComponent>();
					sc.sector = this;
					sc.sectorManager = MapLoader.Loader.controller.sectorManager;
					MapLoader.Loader.controller.sectorManager.AddSector(sc);
				}
                return gao;
            }
        }

        private SuperObject superObject;
        public SuperObject SuperObject {
            get { return superObject; }
        }


        public Sector(Pointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public void ProcessPointers(Reader reader) {
            MapLoader l = MapLoader.Loader;
            if (neighbors != null && neighbors.Count > 0) {
                neighbors.ReadEntries(ref reader, (off_element) => {
					//l.print(off_element);
                    NeighborSector n = new NeighborSector();
					if (Settings.s.game != Settings.Game.LargoWinch) {
						n.short0 = reader.ReadUInt16();
						n.short2 = reader.ReadUInt16();
					}
					Pointer sp = Pointer.Read(reader);
					n.sector = Sector.FromSuperObjectOffset(sp);
                    //l.print(name + " -> " + n.sector.name + ": " + n.short0 + " - " + n.short2);
                    if (Settings.s.linkedListType == LinkedList.Type.Minimize) {
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
            if (sectors_unk1 != null && sectors_unk1.Count > 0) {
                //l.print(sectors_unk1.off_head + " - " + sectors_unk1.off_tail + " - " + sectors_unk1.Count);
                sectors_unk1.ReadEntries(ref reader, (off_element) => {
                    NeighborSector n = new NeighborSector();
                    if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                        n.short0 = reader.ReadUInt16();
                        n.short2 = reader.ReadUInt16();
                    }
                    Pointer sp = Pointer.Read(reader);
                    n.sector = Sector.FromSuperObjectOffset(sp);
                    //l.print(name + " -> " + n.sector.name + ": " + n.short0 + " - " + n.short2);
                    if (Settings.s.linkedListType == LinkedList.Type.Minimize) {
                        n.off_next = off_element + 4; // No next pointer, each entry is immediately after the first one.
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
            if (sectors_unk2 != null && sectors_unk2.Count > 0) {
                //l.print(sectors_unk2.off_head + " - " + sectors_unk2.off_tail + " - " + sectors_unk2.Count);
                sectors_unk2.ReadEntries(ref reader, (off_element) => {
                    //l.print(Pointer.Current(reader) + " - " + off_element);
                    return Sector.FromSuperObjectOffset(off_element);
                },
                flags: LinkedList.Flags.ElementPointerFirst
                    | LinkedList.Flags.ReadAtPointer
                    | ((Settings.s.hasLinkedListHeaderPointers) ?
                        LinkedList.Flags.HasHeaderPointers :
                        LinkedList.Flags.NoPreviousPointersForDouble));
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
            s.name = "Sector @ " + offset + ", SPO @ "+so.offset;
			//l.print(s.name);
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) {
                if (Settings.s.game == Settings.Game.TTSE) reader.ReadUInt32(); // always 1 or 0. whether the sector is active or not?
                Pointer off_collideObj = Pointer.Read(reader);
                Pointer.DoAt(ref reader, off_collideObj, () => {
                    //CollideMeshObject collider = CollideMeshObject.Read(reader, off_collideObj);
                    // This has the exact same structure as a CollideMeshObject but with a sector superobject as material for the collieMeshElements
                });
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double); // "environments list"
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double); // "surface list"
            }
            s.persos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
            s.staticLights = LinkedList<LightInfo>.Read(ref reader, Pointer.Current(reader),
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
                type: LinkedList.Type.Minimize
            );
            s.dynamicLights = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) {
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader)); // "streams list", probably related to water
            }
            s.neighbors = LinkedList<NeighborSector>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Minimize);
            s.sectors_unk1 = LinkedList<NeighborSector>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Minimize);
            s.sectors_unk2 = LinkedList<Sector>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Minimize);

            LinkedList<int>.ReadHeader(reader, Pointer.Current(reader)); // Placeholder
            LinkedList<int>.ReadHeader(reader, Pointer.Current(reader)); // Placeholder

            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                s.sectorBorder = BoundingVolume.Read(reader, Pointer.Current(reader), BoundingVolume.Type.Box);
				reader.ReadUInt32();
				s.isSectorVirtual = reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();

				if (Settings.s.game != Settings.Game.R2Revolution) {
					if (Settings.s.engineVersion <= Settings.EngineVersion.R2) {
						s.off_skyMaterial = Pointer.Read(reader);
						s.skyMaterial = VisualMaterial.FromOffsetOrRead(s.off_skyMaterial, reader);
					} else {
						reader.ReadUInt32();
					}
					reader.ReadByte();
					if (Settings.s.hasNames) {
						s.name = reader.ReadString(0x104);
						l.print(s.name);
					}
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
                    s.name = reader.ReadNullDelimitedString() + " @ " + offset;
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

        public static Sector FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.sectors.FirstOrDefault(s => s.offset == offset);
        }

        public static Sector FromSuperObjectOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.sectors.FirstOrDefault(s => s.SuperObject.offset == offset);
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
