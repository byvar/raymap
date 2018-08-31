using OpenSpace.AI;
using OpenSpace.Collide;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.EngineObject {
    /// <summary>
    /// Also called "Actor" in the code which might be a better name, but I'll stick to the R2 one for now
    /// </summary>
    public class Perso : IEngineObject {
        public Pointer offset;

        // Struct
        public Pointer off_3dData;
        public Pointer off_stdGame;
        public Pointer off_dynam;
        public Pointer off_brain;
        public Pointer off_camera;
        public Pointer off_collSet;
        public Pointer off_msWay;
        public Pointer off_sectInfo;


        // Derived
        public string fullName = "Perso";
        public string nameFamily = null;
        public string nameModel = null;
        public string namePerso = null;

        public Perso3dData p3dData;
        public StandardGame stdGame;
        public Dynam dynam;
        public Brain brain = null;
        public MSWay msWay = null;
        public CollSet collset;

        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    if (nameFamily != null && nameModel != null && namePerso != null) {
                        fullName = "[" + nameFamily + "] " + nameModel + " | " + namePerso; // + " @ " + offset;
                        /*if (superObject != null) {
                            fullName = superObject.matrix.type + " " + superObject.matrix.v + fullName;
                        }*/
                    }
                    gao = new GameObject(fullName);
                    
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

        public Perso(Pointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public static Perso Read(Reader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            //l.print("Offset: " + offset);
            Perso p = new Perso(offset, so);
            l.persos.Add(p);
            p.off_3dData = Pointer.Read(reader); // 0x0
            p.off_stdGame = Pointer.Read(reader); // 4 Standard Game info
            p.off_dynam = Pointer.Read(reader); // 0x8 Dynam
            p.off_brain = Pointer.Read(reader); // 0xC
            p.off_camera = Pointer.Read(reader); // 0x10 is Camera in Rayman 2
            p.off_collSet = Pointer.Read(reader); // 0x14 collset
            p.off_msWay = Pointer.Read(reader); // 0x18
            reader.ReadUInt32(); // 0x1C
            p.off_sectInfo = Pointer.Read(reader); // 0x20 // Pointer to struct that points to active sector
            reader.ReadUInt32(); // 0x24
            reader.ReadUInt32();
            if (l.mode == MapLoader.Mode.RaymanArenaPC || l.mode == MapLoader.Mode.RaymanArenaGC) reader.ReadUInt32();
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            }

            Pointer.DoAt(ref reader, p.off_3dData, () => {
                p.p3dData = Perso3dData.Read(reader, p.off_3dData);
            });

            if (p.off_stdGame != null) {
                Pointer off_current = Pointer.Goto(ref reader, p.off_stdGame);
                p.stdGame = StandardGame.Read(reader, p.off_stdGame);
                p.nameFamily = p.stdGame.GetName(0);
                p.nameModel = p.stdGame.GetName(1);
                p.namePerso = p.stdGame.GetName(2);

                Pointer.Goto(ref reader, off_current);
            }
            l.print("[" + p.nameFamily + "] " + p.nameModel + " | " + p.namePerso + " - offset: " + offset);
            if (p.off_dynam != null && Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                Pointer off_current = Pointer.Goto(ref reader, p.off_dynam);
                p.dynam = Dynam.Read(reader, p.off_dynam);
                Pointer.Goto(ref reader, off_current);
            }

            if (p.off_brain != null && Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                Pointer off_current = Pointer.Goto(ref reader, p.off_brain);
                p.brain = Brain.Read(reader, p.off_brain);
                if (p.brain != null && p.brain.mind != null && p.brain.mind.AI_model != null && p.nameModel != null) p.brain.mind.AI_model.name = p.nameModel;
                Pointer.Goto(ref reader, off_current);
            }

            /*if (l.mode == MapLoader.Mode.Rayman2PC && off_msWay != null) {
             * MS_Way is always empty at start, instead check DsgVars for graphs
                Pointer off_current = Pointer.Goto(ref reader, off_msWay);

                p.msWay = MSWay.Read(reader, off_msWay);
                Pointer.Goto(ref reader, off_current);

                // Graph read?
                if (p.msWay.graph != null) {
                    GameObject go_msWay = new GameObject("MSWay");
                    go_msWay.transform.SetParent(p.Gao.transform);

                    GameObject go_graph = new GameObject("Graph");
                    go_graph.transform.SetParent(go_msWay.transform);

                    int nodeNum = 0;
                    foreach (GraphNode node in p.msWay.graph.nodeList) {
                        GameObject go_graphNode = new GameObject("GraphNode[" + nodeNum + "].WayPoint");
                        go_graphNode.transform.position.Set(node.wayPoint.position.x, node.wayPoint.position.y, node.wayPoint.position.z);
                        go_graphNode.transform.SetParent(go_graph.transform);
                        nodeNum++;
                    }
                }
            }*/
            if (p.p3dData != null && p.p3dData.family != null) {
                if (p.p3dData.off_physicalObjects != null && p.p3dData.family.GetIndexOfPhysicalList(p.p3dData.off_physicalObjects) == -1) {
                    Pointer.DoAt(ref reader, p.p3dData.off_physicalObjects, () => {
                        p.p3dData.family.ReadNewPhysicalList(reader, p.p3dData.off_physicalObjects);
                    });
                }
                if (p.p3dData.off_physicalObjectsInitial != null && p.p3dData.family.GetIndexOfPhysicalList(p.p3dData.off_physicalObjectsInitial) == -1) {
                    Pointer.DoAt(ref reader, p.p3dData.off_physicalObjectsInitial, () => {
                        p.p3dData.family.ReadNewPhysicalList(reader, p.p3dData.off_physicalObjectsInitial);
                    });
                }
                if (p.brain != null && p.brain.mind != null && p.brain.mind.AI_model != null
                    && !(Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.loadFromMemory)) { // Weird bug for R3 memory loading
                                                                                                          // Add physical objects tables hidden in scripts
                    AIModel ai = p.brain.mind.AI_model;
                    if (ai.behaviors_normal != null) {
                        for (int i = 0; i < ai.behaviors_normal.Length; i++) {
                            if (ai.behaviors_normal[i].scripts != null) {
                                for (int j = 0; j < ai.behaviors_normal[i].scripts.Length; j++) {
                                    List<ScriptNode> nodes = p.brain.mind.AI_model.behaviors_normal[i].scripts[j].scriptNodes;
                                    foreach (ScriptNode node in nodes) {
                                        if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ObjectTableRef
                                            && p.p3dData.family.GetIndexOfPhysicalList(node.param_ptr) == -1) {
                                            Pointer.DoAt(ref reader, node.param_ptr, () => {
                                                p.p3dData.family.ReadNewPhysicalList(reader, node.param_ptr);
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (ai.behaviors_reflex != null) {
                        for (int i = 0; i < ai.behaviors_reflex.Length; i++) {
                            if (ai.behaviors_reflex[i].scripts != null) {
                                for (int j = 0; j < ai.behaviors_reflex[i].scripts.Length; j++) {
                                    List<ScriptNode> nodes = p.brain.mind.AI_model.behaviors_reflex[i].scripts[j].scriptNodes;
                                    foreach (ScriptNode node in nodes) {
                                        if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ObjectTableRef
                                            && p.p3dData.family.GetIndexOfPhysicalList(node.param_ptr) == -1) {
                                            Pointer.DoAt(ref reader, node.param_ptr, () => {
                                                p.p3dData.family.ReadNewPhysicalList(reader, node.param_ptr);
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (ai.macros != null) {
                        for (int i = 0; i < ai.macros.Length; i++) {
                            if (ai.macros[i].script != null) {
                                List<ScriptNode> nodes = p.brain.mind.AI_model.macros[i].script.scriptNodes;
                                foreach (ScriptNode node in nodes) {
                                    if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ObjectTableRef
                                        && p.p3dData.family.GetIndexOfPhysicalList(node.param_ptr) == -1) {
                                        Pointer.DoAt(ref reader, node.param_ptr, () => {
                                            p.p3dData.family.ReadNewPhysicalList(reader, node.param_ptr);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                if (p.p3dData.family.GetIndexOfPhysicalList(p.p3dData.off_physicalObjects) != -1) {
                    p.p3dData.physical_objects = p.p3dData.family.physical_objects[p.p3dData.family.GetIndexOfPhysicalList(p.p3dData.off_physicalObjects)];
                }
            }

            if (p.off_collSet!=null && Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                Pointer.Goto(ref reader, p.off_collSet);
                p.collset = CollSet.Read(reader, p, p.off_collSet);
            }

            return p;
        }

        public static Perso FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.persos.FirstOrDefault(f => f.offset == offset);
        }

        public void Write(Writer writer) {
            PersoBehaviour persoBehaviour = gao.GetComponent<PersoBehaviour>();
            if (p3dData != null && persoBehaviour != null && persoBehaviour.state!=null && persoBehaviour.state.offset!=null) {
                if (persoBehaviour.state.offset != p3dData.off_stateCurrent) {
                    p3dData.off_stateCurrent = persoBehaviour.state.offset;
                    Pointer.Goto(ref writer, p3dData.offset);
                    p3dData.Write(writer);
                } /*else {
                    MapLoader.Loader.print("do not write state for perso " + fullName);
                }*/
            }

            if (persoBehaviour.clearTheBrain) {
                Pointer.Goto(ref writer, this.offset + 0xC); // perso + 0xC = Brain * 
                Pointer.Write(writer, null);
                persoBehaviour.clearTheBrain = false;
            }

            CustomBitsComponent customBits = gao.GetComponent<CustomBitsComponent>();
            if (customBits != null && customBits.modified && stdGame != null) {
                Pointer.Goto(ref writer, stdGame.offset);
                stdGame.Write(writer);
            }
        }
    }
}
