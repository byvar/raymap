using OpenSpace.Collide;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object {
    public class Sector : IEngineObject, IReferenceable {

        public Pointer offset;
        public string name = "Sector";

        public LinkedList<Perso> persos;
        public LinkedList<LightInfo> staticLights;
        public LinkedList<int> dynamicLights; // Stub
        public LinkedList<NeighborSector> graphicSectors;
        public LinkedList<NeighborSector> collisionSectors;
        public LinkedList<Sector> activitySectors;

        public byte isSectorVirtual;
		public byte sectorPriority;
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
                    if (collider != null) {
                        collider.Gao.transform.SetParent(gao.transform);
                        collider.Gao.transform.localPosition = Vector3.zero;
                    }
					sc.sectorManager = MapLoader.Loader.controller.sectorManager;
					MapLoader.Loader.controller.sectorManager.AddSector(sc);
				}
                return gao;
            }
        }
        public GeometricObjectCollide collider; // Only for TT & Montreal

        private SuperObject superObject;
        public SuperObject SuperObject {
            get { return superObject; }
        }

        public ReferenceFields References { get; set; } = new ReferenceFields();

        public Sector(Pointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public void ProcessPointers(Reader reader) {
            MapLoader l = MapLoader.Loader;
            if (graphicSectors != null && graphicSectors.Count > 0) {
                graphicSectors.ReadEntries(ref reader, (off_element) => {
					//l.print(off_element);
                    NeighborSector n = new NeighborSector();
					if (CPA_Settings.s.game != CPA_Settings.Game.LargoWinch) {
						n.short0 = reader.ReadUInt16();
						n.short2 = reader.ReadUInt16();
					}
					Pointer sp = Pointer.Read(reader);
					n.sector = Sector.FromSuperObjectOffset(sp);
                    //l.print(name + " -> " + n.sector.name + ": " + n.short0 + " - " + n.short2);
                    if (CPA_Settings.s.linkedListType == LinkedList.Type.Minimize) {
                        n.off_next = off_element + 8; // No next pointer, each entry is immediately after the first one.
                    } else {
                        n.off_next = Pointer.Read(reader);
                        if (CPA_Settings.s.hasLinkedListHeaderPointers) {
                            n.off_previous = Pointer.Read(reader);
                            Pointer off_sector_start = Pointer.Read(reader);
                        }
                    }
                    return n;
                });
            }
            if (collisionSectors != null && collisionSectors.Count > 0) {
                //l.print(sectors_unk1.off_head + " - " + sectors_unk1.off_tail + " - " + sectors_unk1.Count);
                collisionSectors.ReadEntries(ref reader, (off_element) => {
                    NeighborSector n = new NeighborSector();
                    if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                        n.short0 = reader.ReadUInt16();
                        n.short2 = reader.ReadUInt16();
                    }
                    Pointer sp = Pointer.Read(reader);
                    n.sector = Sector.FromSuperObjectOffset(sp);
                    //l.print(name + " -> " + n.sector.name + ": " + n.short0 + " - " + n.short2);
                    if (CPA_Settings.s.linkedListType == LinkedList.Type.Minimize) {
                        n.off_next = off_element + 4; // No next pointer, each entry is immediately after the first one.
                    } else {
                        n.off_next = Pointer.Read(reader);
                        if (CPA_Settings.s.hasLinkedListHeaderPointers) {
                            n.off_previous = Pointer.Read(reader);
                            Pointer off_sector_start = Pointer.Read(reader);
                        }
                    }
                    return n;
                });
            }
            if (activitySectors != null && activitySectors.Count > 0) {
                //l.print(sectors_unk2.off_head + " - " + sectors_unk2.off_tail + " - " + sectors_unk2.Count);
                activitySectors.ReadEntries(ref reader, (off_element) => {
                    //l.print(Pointer.Current(reader) + " - " + off_element);
                    return Sector.FromSuperObjectOffset(off_element);
                },
                flags: LinkedList.Flags.ElementPointerFirst
                    | LinkedList.Flags.ReadAtPointer
                    | ((CPA_Settings.s.hasLinkedListHeaderPointers) ?
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
            if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.Montreal) {
                if (CPA_Settings.s.game == CPA_Settings.Game.TTSE) {
                    s.isSectorVirtual = reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                }
                Pointer off_collideObj = Pointer.Read(reader);
                Pointer.DoAt(ref reader, off_collideObj, () => {
                    s.collider = GeometricObjectCollide.Read(reader, off_collideObj, isBoundingVolume: true);
                    // This has the exact same structure as a CollideMeshObject but with a sector superobject as material for the collieMeshElements
                });
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double); // "environments list"
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double); // "surface list"
            }
            s.persos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
            s.staticLights = LinkedList<LightInfo>.Read(ref reader, Pointer.Current(reader),
                (off_element) => {
                    LightInfo li = l.FromOffsetOrRead<LightInfo>(reader, off_element);
                    if (li != null) li.containingSectors.Add(s);
                    return li;
                },
                flags: LinkedList.Flags.ElementPointerFirst
                    | LinkedList.Flags.ReadAtPointer
                    | ((CPA_Settings.s.hasLinkedListHeaderPointers) ?
                        LinkedList.Flags.HasHeaderPointers :
                        LinkedList.Flags.NoPreviousPointersForDouble),
                type: LinkedList.Type.Minimize
            );
            s.dynamicLights = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
            if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.Montreal) {
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader)); // "streams list", probably related to water
            }
            s.graphicSectors = LinkedList<NeighborSector>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Minimize);
            s.collisionSectors = LinkedList<NeighborSector>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Minimize);
            s.activitySectors = LinkedList<Sector>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Minimize);

            LinkedList<int>.ReadHeader(reader, Pointer.Current(reader)); // TT says: Sound Sectors
            LinkedList<int>.ReadHeader(reader, Pointer.Current(reader)); // Placeholder

            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal) {
                s.sectorBorder = BoundingVolume.Read(reader, Pointer.Current(reader), BoundingVolume.Type.Box);
				reader.ReadUInt32();
				if (CPA_Settings.s.game == CPA_Settings.Game.R2Revolution || CPA_Settings.s.game == CPA_Settings.Game.LargoWinch) {
					s.isSectorVirtual = reader.ReadByte();
					reader.ReadByte();
					s.sectorPriority = reader.ReadByte();
					reader.ReadByte();
				} else {
					s.isSectorVirtual = reader.ReadByte();
					reader.ReadByte();
					reader.ReadByte();
					s.sectorPriority = reader.ReadByte();
					if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.R2) {
						s.off_skyMaterial = Pointer.Read(reader);
						s.skyMaterial = VisualMaterial.FromOffsetOrRead(s.off_skyMaterial, reader);
					} else {
						reader.ReadUInt32();
					}
					reader.ReadByte();
					if (CPA_Settings.s.hasNames) {
						s.name = reader.ReadString(0x104);
						l.print(s.name);
					}
				}
            } else {
                if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                }
                if (CPA_Settings.s.game != CPA_Settings.Game.TTSE) {
                    s.isSectorVirtual = reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                }
                if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                    reader.ReadUInt32(); // activation flag
                }
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
