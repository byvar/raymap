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

        public Pointer off_persosInSector_first;
        public Pointer off_persosInSector_last;
        public uint num_persosInSector;

        public Pointer off_lights_first;
        public Pointer off_lights_last;
        public uint num_lights;

        public Pointer off_neighbors_first;
        public Pointer off_neighbors_last;
        public uint num_neighbors;

        public byte isSectorVirtual;

        public BoundingVolume sectorBorder;
        public List<LightInfo> sectorLights;
        public List<Sector> neighbors;
        public List<Pointer> neighborsPointers;
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
            get {
                return superObject;
            }
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
            sectorLights = new List<LightInfo>();
            neighbors = new List<Sector>();
            neighborsPointers = new List<Pointer>();
        }

        public void ProcessNeighbors() {
            MapLoader l = MapLoader.Loader;
            for (int i = 0; i < neighborsPointers.Count; i++) {
                Sector neigh = l.sectors.FirstOrDefault(s => s.SuperObject.offset == neighborsPointers[i]);
                if (neigh != null) neighbors.Add(neigh);
            }
        }

        public static Sector Read(EndianBinaryReader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            Sector s = new Sector(offset, so);
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            s.off_lights_first = Pointer.Read(reader);
            if (Settings.s.linkedListType == Settings.LinkedListType.Double) {
                s.off_lights_last = Pointer.Read(reader);
            }
            s.num_lights = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            s.off_neighbors_first = Pointer.Read(reader);
            if (Settings.s.linkedListType == Settings.LinkedListType.Double) {
                s.off_neighbors_last = Pointer.Read(reader);
            }
            s.num_neighbors = reader.ReadUInt32();
            reader.ReadUInt32();
            if (Settings.s.linkedListType == Settings.LinkedListType.Double) reader.ReadUInt32();
            reader.ReadUInt32();
            Pointer off_subsector_unk_first = Pointer.Read(reader);
            if (Settings.s.linkedListType == Settings.LinkedListType.Double) {
                Pointer off_subsector_unk_last = Pointer.Read(reader);
            }
            uint num_subsectors_unk = reader.ReadUInt32();
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
            if (s.num_lights > 0 && s.off_lights_first != null) {
                if (l.mode == MapLoader.Mode.RaymanArenaGC) {
                    Pointer.Goto(ref reader, s.off_lights_first);
                    for (int i = 0; i < s.num_lights; i++) {
                        Pointer off_light = Pointer.Read(reader);
                        if (off_light != null) {
                            Pointer off_current = Pointer.Goto(ref reader, off_light);
                            LightInfo li = LightInfo.Read(reader, off_light);
                            if (li != null) {
                                s.sectorLights.Add(li);
                                li.containingSectors.Add(s);
                                /*Light l = r3l.Light;
                                if (l != null) {
                                    l.transform.parent = gao.transform;
                                }*/
                            }
                            Pointer.Goto(ref reader, off_current);
                        }
                    }
                } else {
                    Pointer off_lights_next = s.off_lights_first;
                    for (int i = 0; i < s.num_lights; i++) {
                        Pointer.Goto(ref reader, off_lights_next);
                        //reader.ReadUInt32();
                        Pointer off_light = Pointer.Read(reader);
                        off_lights_next = Pointer.Read(reader);
                        if (l.mode == MapLoader.Mode.Rayman3GC) {
                            Pointer off_lights_prev = Pointer.Read(reader);
                            Pointer off_lights_header = Pointer.Read(reader); // points back to the pos in the header where off_lights first and last are defined
                        }
                        if (off_light != null) {
                            Pointer.Goto(ref reader, off_light);
                            LightInfo li = LightInfo.Read(reader, off_light);
                            if (li != null) {
                                s.sectorLights.Add(li);
                                li.containingSectors.Add(s);
                                /*Light l = r3l.Light;
                                if (l != null) {
                                    l.transform.parent = gao.transform;
                                }*/
                            }
                        }
                    }
                }
            }
            if (s.num_neighbors > 0 && s.off_neighbors_first != null) {
                Pointer off_neighbor_next = s.off_neighbors_first;
                for (int i = 0; i < s.num_neighbors; i++) {
                    Pointer.Goto(ref reader, off_neighbor_next);
                    reader.ReadUInt16();
                    reader.ReadUInt16();
                    Pointer off_neighbor = Pointer.Read(reader);
                    if (l.mode == MapLoader.Mode.RaymanArenaGC) {
                        off_neighbor_next += 8; // We just read 8 bytes
                    } else {
                        off_neighbor_next = Pointer.Read(reader);
                    }
                    if (l.mode == MapLoader.Mode.Rayman3GC) {
                        Pointer off_neighbor_prev = Pointer.Read(reader);
                        Pointer off_sector_start = Pointer.Read(reader);
                    }
                    if (off_neighbor != null) {
                        s.neighborsPointers.Add(off_neighbor);
                    }
                }
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
    }
}
