using OpenSpace.AI;
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
        public string fullName = "Perso";
        public string nameFamily = null;
        public string nameModel = null;
        public string namePerso = null;
        public Family family = null;
        public Pointer off_physicalObjects = null;
        public PhysicalObject[] physical_objects = null;
        public Brain brain = null;
        public State initialState = null;
        public Pointer off_currentState;
        public Pointer off_pointerToCurrentState;
        public MSWay msWay = null;

        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    if (nameFamily != null && nameModel != null && namePerso != null) {
                        fullName = "[" + nameFamily + "] " + nameModel + " | " + namePerso;
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

        public static Perso Read(EndianBinaryReader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            //l.print("Offset: " + offset);
            Perso p = new Perso(offset, so);
            l.persos.Add(p);
            Pointer off_perso = Pointer.Read(reader); // 0x0
            Pointer off_stdGame = Pointer.Read(reader); // 4 Standard Game info
            Pointer off_unknown = Pointer.Read(reader); // 0x8
            Pointer off_brain = Pointer.Read(reader); // 0xC
            reader.ReadUInt32(); // 0x10 is Camera in Rayman 2
            Pointer off_collset = Pointer.Read(reader); // 0x14 platform info
            if (off_collset != null) l.print(off_collset);
            Pointer off_msWay = Pointer.Read(reader); // 0x18
            reader.ReadUInt32(); // 0x1C
            Pointer off_sectInfo = Pointer.Read(reader); // 0x20 // Pointer to struct that points to active sector
            reader.ReadUInt32(); // 0x24
            reader.ReadUInt32();
            if (l.mode == MapLoader.Mode.RaymanArenaPC || l.mode == MapLoader.Mode.RaymanArenaGC) reader.ReadUInt32();
            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            //R3Pointer.Goto(ref reader, off_perso);
            Pointer.Read(reader); // same as next
            p.off_pointerToCurrentState = Pointer.Current(reader);
            p.off_currentState = Pointer.Read(reader);
            Pointer.Read(reader); // same as previous
            p.off_physicalObjects = Pointer.Read(reader);
            reader.ReadUInt32(); // same address?
            Pointer off_family = Pointer.Read(reader);
            p.family = Family.FromOffset(off_family);
            p.initialState = State.FromOffset(p.family, p.off_currentState);

            if (off_stdGame != null) {
                Pointer off_current = Pointer.Goto(ref reader, off_stdGame);
                uint index0 = reader.ReadUInt32();
                uint index1 = reader.ReadUInt32();
                uint index2 = reader.ReadUInt32();
                if (index0 >= 0 && index0 < l.objectTypes[0].Length) p.nameFamily = l.objectTypes[0][index0].name;
                if (index1 >= 0 && index1 < l.objectTypes[1].Length) p.nameModel = l.objectTypes[1][index1].name;
                if (index2 >= 0 && index2 < l.objectTypes[2].Length) p.namePerso = l.objectTypes[2][index2].name;
                Pointer.Goto(ref reader, off_current);
            }
            l.print("[" + p.nameFamily + "] " + p.nameModel + " | " + p.namePerso + " - offset: " + offset + " - POs: " + p.off_physicalObjects);

            if (off_brain != null) {
                Pointer off_current = Pointer.Goto(ref reader, off_brain);
                p.brain = Brain.Read(reader, off_brain);
                if (p.brain.mind != null && p.brain.mind.AI_model != null && p.nameModel != null) p.brain.mind.AI_model.name = p.nameModel;
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
            if (p.brain != null && p.brain.mind != null && p.brain.mind.AI_model != null && p.family != null) {
                // Add physical objects tables hidden in scripts
                AIModel ai = p.brain.mind.AI_model;
                if (ai.behaviors_normal != null) {
                    for (int i = 0; i < ai.behaviors_normal.Length; i++) {
                        if (ai.behaviors_normal[i].scripts != null) {
                            for (int j = 0; j < ai.behaviors_normal[i].scripts.Length; j++) {
                                List<ScriptNode> nodes = p.brain.mind.AI_model.behaviors_normal[i].scripts[j].scriptNodes;
                                foreach (ScriptNode node in nodes) {
                                    if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ObjectTableRef
                                        && p.family.GetIndexOfPhysicalList(node.param_ptr) == -1) {
                                        Pointer off_current = Pointer.Goto(ref reader, node.param_ptr);
                                        p.family.ReadNewPhysicalList(reader, node.param_ptr);
                                        Pointer.Goto(ref reader, off_current);
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
                                        && p.family.GetIndexOfPhysicalList(node.param_ptr) == -1) {
                                        Pointer off_current = Pointer.Goto(ref reader, node.param_ptr);
                                        p.family.ReadNewPhysicalList(reader, node.param_ptr);
                                        Pointer.Goto(ref reader, off_current);
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
                                    && p.family.GetIndexOfPhysicalList(node.param_ptr) == -1) {
                                    Pointer off_current = Pointer.Goto(ref reader, node.param_ptr);
                                    p.family.ReadNewPhysicalList(reader, node.param_ptr);
                                    Pointer.Goto(ref reader, off_current);
                                }
                            }
                        }
                    }
                }
            }
            //if (off_physicalObjects != null && off_physicalObjects.offset == 0x7029) off_physicalObjects.offset = 0x7659;
            if (p.family != null && p.family.GetIndexOfPhysicalList(p.off_physicalObjects) != -1) {
                p.physical_objects = p.family.physical_objects[p.family.GetIndexOfPhysicalList(p.off_physicalObjects)];
            }
            return p;
        }

        public static Perso FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.persos.FirstOrDefault(f => f.offset == offset);
        }

        public static void Write(Perso perso, EndianBinaryWriter writer)
        {
            PersoBehaviour persoBehaviour = perso.gao.GetComponent<PersoBehaviour>();
            if (perso.off_pointerToCurrentState!=null && persoBehaviour != null && persoBehaviour.state!=null && persoBehaviour.state.offset!=null) {
                if (persoBehaviour.state.offset != perso.off_currentState)
                {
                    MapLoader.Loader.print("write state perso " + perso.fullName + ".off_pointerToCurrentState = " + perso.off_pointerToCurrentState);
                    Pointer.Goto(ref writer, perso.off_pointerToCurrentState);
                    writer.Write(persoBehaviour.state.offset.offset);
                } else
                {
                    MapLoader.Loader.print("do not write state for perso " + perso.fullName);
                }
            }
        }
    }
}
