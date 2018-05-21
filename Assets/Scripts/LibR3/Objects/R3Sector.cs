using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3Sector : IR3Data {
        public R3Pointer offset;
        public string name = "Sector";
        public Vector3 minBorder;
        public Vector3 maxBorder;
        public List<R3Light> sectorLights;
        public List<R3Sector> neighbors;
        public List<R3Pointer> neighborsPointers;
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);
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


        public R3Sector(R3Pointer offset, R3SuperObject so) {
            this.offset = offset;
            this.superObject = so;
            sectorLights = new List<R3Light>();
            neighbors = new List<R3Sector>();
            neighborsPointers = new List<R3Pointer>();
        }

        public void ProcessNeighbors() {
            R3Loader l = R3Loader.Loader;
            for (int i = 0; i < neighborsPointers.Count; i++) {
                R3Sector neigh = l.sectors.FirstOrDefault(s => s.SuperObject.off_superObject == neighborsPointers[i]);
                if (neigh != null) neighbors.Add(neigh);
            }
        }

        public static R3Sector Read(EndianBinaryReader reader, R3Pointer offset, R3SuperObject so) {
            R3Loader l = R3Loader.Loader;
            R3Sector sect = new R3Sector(offset, so);
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            R3Pointer off_lights_first = R3Pointer.Read(reader);
            if (l.mode != R3Loader.Mode.RaymanArenaGC) {
                R3Pointer off_lights_last = R3Pointer.Read(reader);
            }
            uint num_lights = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            R3Pointer off_neighbor_first = R3Pointer.Read(reader);
            if (l.mode != R3Loader.Mode.RaymanArenaGC) {
                R3Pointer off_subsector_last = R3Pointer.Read(reader);
            }
            uint num_neighbors = reader.ReadUInt32();
            reader.ReadUInt32();
            if (l.mode != R3Loader.Mode.RaymanArenaGC) reader.ReadUInt32();
            reader.ReadUInt32();
            R3Pointer off_subsector_unk_first = R3Pointer.Read(reader);
            if (l.mode != R3Loader.Mode.RaymanArenaGC) {
                R3Pointer off_subsector_unk_last = R3Pointer.Read(reader);
            }
            uint num_subsectors_unk = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();

            float minPoint_x = reader.ReadSingle();
            float minPoint_z = reader.ReadSingle();
            float minPoint_y = reader.ReadSingle();
            float maxPoint_x = reader.ReadSingle();
            float maxPoint_z = reader.ReadSingle();
            float maxPoint_y = reader.ReadSingle();
            sect.minBorder = new Vector3(minPoint_x, minPoint_y, minPoint_z);
            sect.maxBorder = new Vector3(maxPoint_x, maxPoint_y, maxPoint_z);

            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadByte();
            if (l.mode == R3Loader.Mode.Rayman3GC) {
                sect.name = new string(reader.ReadChars(0x104));
                l.print(sect.name);
            }
            if (num_lights > 0 && off_lights_first != null) {
                if (l.mode == R3Loader.Mode.RaymanArenaGC) {
                    R3Pointer.Goto(ref reader, off_lights_first);
                    for (int i = 0; i < num_lights; i++) {
                        R3Pointer off_light = R3Pointer.Read(reader);
                        if (off_light != null) {
                            R3Pointer off_current = R3Pointer.Goto(ref reader, off_light);
                            R3Light r3l = R3Light.Read(reader, off_light);
                            if (r3l != null) {
                                sect.sectorLights.Add(r3l);
                                r3l.containingSectors.Add(sect);
                                /*Light l = r3l.Light;
                                if (l != null) {
                                    l.transform.parent = gao.transform;
                                }*/
                            }
                            R3Pointer.Goto(ref reader, off_current);
                        }
                    }
                } else {
                    R3Pointer off_lights_next = off_lights_first;
                    for (int i = 0; i < num_lights; i++) {
                        R3Pointer.Goto(ref reader, off_lights_next);
                        //reader.ReadUInt32();
                        R3Pointer off_light = R3Pointer.Read(reader);
                        off_lights_next = R3Pointer.Read(reader);
                        if (l.mode == R3Loader.Mode.Rayman3GC) {
                            R3Pointer off_lights_prev = R3Pointer.Read(reader);
                            R3Pointer off_lights_header = R3Pointer.Read(reader); // points back to the pos in the header where off_lights first and last are defined
                        }
                        if (off_light != null) {
                            R3Pointer.Goto(ref reader, off_light);
                            R3Light r3l = R3Light.Read(reader, off_light);
                            if (r3l != null) {
                                sect.sectorLights.Add(r3l);
                                r3l.containingSectors.Add(sect);
                                /*Light l = r3l.Light;
                                if (l != null) {
                                    l.transform.parent = gao.transform;
                                }*/
                            }
                        }
                    }
                }
            }
            if (num_neighbors > 0 && off_neighbor_first != null) {
                R3Pointer off_neighbor_next = off_neighbor_first;
                for (int i = 0; i < num_neighbors; i++) {
                    R3Pointer.Goto(ref reader, off_neighbor_next);
                    reader.ReadUInt16();
                    reader.ReadUInt16();
                    R3Pointer off_neighbor = R3Pointer.Read(reader);
                    if (l.mode == R3Loader.Mode.RaymanArenaGC) {
                        off_neighbor_next += 8; // We just read 8 bytes
                    } else {
                        off_neighbor_next = R3Pointer.Read(reader);
                    }
                    if (l.mode == R3Loader.Mode.Rayman3GC) {
                        R3Pointer off_neighbor_prev = R3Pointer.Read(reader);
                        R3Pointer off_sector_start = R3Pointer.Read(reader);
                    }
                    if (off_neighbor != null) {
                        sect.neighborsPointers.Add(off_neighbor);
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

            l.sectors.Add(sect);
            return sect;
        }
    }
}
