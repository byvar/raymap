using Newtonsoft.Json;
using OpenSpace.AI;
using OpenSpace.Collide;
using OpenSpace.Object.Properties;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object {
    /// <summary>
    /// Also called "Actor" in the code which might be a better name, but I'll stick to the R2 one for now
    /// </summary>
    public class Perso : IEngineObject, IReferenceable {
        public Pointer offset;

        // Struct
        public Pointer off_3dData;
        public Pointer off_stdGame;
        public Pointer off_dynam;
        public Pointer off_brain;
        public Pointer off_camera;
        public Pointer off_collSet;
        public Pointer off_msWay;
        public Pointer off_msLight;
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
        public PersoSectorInfo sectInfo;

        [JsonIgnore]
        public ReferenceFields References { get; set; } = new ReferenceFields();

        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    if (nameFamily != null && nameModel != null && namePerso != null) {
                        fullName = "[" + nameFamily + "] " + nameModel + " | " + namePerso; // + " @ " + offset;
                        /*if (superObject != null) {
                            fullName += " - " + superObject.offset;
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
            Perso p = new Perso(offset, so);
			//l.print("Perso " + offset);
			l.persos.Add(p);
            p.off_3dData = Pointer.Read(reader); // 0x0
            p.off_stdGame = Pointer.Read(reader); // 4 Standard Game info
            p.off_dynam = Pointer.Read(reader); // 0x8 Dynam
            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) reader.ReadUInt32();
            p.off_brain = Pointer.Read(reader); // 0xC
            p.off_camera = Pointer.Read(reader); // 0x10 is Camera in Rayman 2
            p.off_collSet = Pointer.Read(reader); // 0x14 collset
            p.off_msWay = Pointer.Read(reader); // 0x18
            p.off_msLight = Pointer.Read(reader); // 0x1C - MSLight
            if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.Montreal) reader.ReadUInt32();
            p.off_sectInfo = Pointer.Read(reader); // 0x20 // Pointer to struct that points to active sector
            reader.ReadUInt32(); // 0x24
            reader.ReadUInt32();
            if (CPA_Settings.s.game == CPA_Settings.Game.RA || CPA_Settings.s.game == CPA_Settings.Game.RM) reader.ReadUInt32();
            if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            }

            Pointer.DoAt(ref reader, p.off_3dData, () => {
                p.p3dData = Perso3dData.Read(reader, p.off_3dData);
            });

			Pointer.DoAt(ref reader, p.off_stdGame, () => {
				p.stdGame = StandardGame.Read(reader, p.off_stdGame);
				if (CPA_Settings.s.hasObjectTypes) {
					p.nameFamily = p.stdGame.GetName(0);
					p.nameModel = p.stdGame.GetName(1);
					p.namePerso = p.stdGame.GetName(2);
				} else {
					p.nameFamily = "Family" + p.stdGame.objectTypes[0];
					p.nameModel = "Model" + p.stdGame.objectTypes[1];
					p.namePerso = "Instance" + p.stdGame.objectTypes[2];
					if (p.p3dData != null && p.p3dData.family != null && p.p3dData.family.name == null) {
						p.p3dData.family.name = p.nameFamily;
						p.p3dData.family.family_index = p.stdGame.objectTypes[0];

                        if (UnitySettings.CreateFamilyGameObjects && p.p3dData.family.Gao != null) {
							p.p3dData.family.Gao.name = "[Family] " + p.nameFamily;
						}
					}
				}
			});
			
            l.print("[" + p.nameFamily + "] " + p.nameModel + " | " + p.namePerso + " - offset: " + offset+" superObject offset: "+(so!=null?so.offset.ToString():"null"));
            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal && CPA_Settings.s.game != CPA_Settings.Game.R2Revolution) {
				Pointer.DoAt(ref reader, p.off_dynam, () => {
					p.dynam = Dynam.Read(reader, p.off_dynam);
				});
            }

            Pointer.DoAt(ref reader, p.off_brain, () => {
                p.brain = Brain.Read(reader, p.off_brain);
                if (p.brain != null && p.brain.mind != null && p.brain.mind.AI_model != null && p.nameModel != null) p.brain.mind.AI_model.name = p.nameModel;
            });

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
                if (p.p3dData.off_objectList != null && p.p3dData.family.GetIndexOfPhysicalList(p.p3dData.off_objectList) == -1) {
					ObjectList ol = ObjectList.FromOffsetOrRead(p.p3dData.off_objectList, reader);
					p.p3dData.family.AddNewPhysicalList(ol);
					/*if (ol != null) {
						p.p3dData.family.AddNewPhysicalList(ol);
						ol.Gao.transform.SetParent(p.p3dData.family.Gao.transform);
					}*/
				}
                if (p.p3dData.off_objectListInitial != null && p.p3dData.family.GetIndexOfPhysicalList(p.p3dData.off_objectListInitial) == -1) {
					ObjectList ol = ObjectList.FromOffsetOrRead(p.p3dData.off_objectListInitial, reader);
					p.p3dData.family.AddNewPhysicalList(ol);
					/*if (ol != null) {
						p.p3dData.family.AddNewPhysicalList(ol);
						ol.Gao.transform.SetParent(p.p3dData.family.Gao.transform);
					}*/
				}
                if (p.brain != null && p.brain.mind != null && p.brain.mind.AI_model != null
                    && !(CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.R3 && CPA_Settings.s.loadFromMemory)) { // Weird bug for R3 memory loading
                                                                                                          // Add physical objects tables hidden in scripts
                    AIModel ai = p.brain.mind.AI_model;
                    if (ai.behaviors_normal != null) {
                        for (int i = 0; i < ai.behaviors_normal.Length; i++) {
                            if (ai.behaviors_normal[i].scripts != null) {
                                for (int j = 0; j < ai.behaviors_normal[i].scripts.Length; j++) {
                                    List<ScriptNode> nodes = p.brain.mind.AI_model.behaviors_normal[i].scripts[j].scriptNodes;
                                    foreach (ScriptNode node in nodes) {
                                        if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ObjectTableRef) {
                                            ObjectList ol = ObjectList.FromOffsetOrRead(node.param_ptr, reader);
                                            ol.unknownFamilyName = p.p3dData.family.name;
                                            ol.AddToFamilyLists(p);
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
                                        if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ObjectTableRef) {
                                            ObjectList ol = ObjectList.FromOffsetOrRead(node.param_ptr, reader);
                                            ol.unknownFamilyName = p.p3dData.family.name;
                                            ol.AddToFamilyLists(p);
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
                                    if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ObjectTableRef) {
                                        ObjectList ol = ObjectList.FromOffsetOrRead(node.param_ptr, reader);
                                        ol.unknownFamilyName = p.p3dData.family.name;
                                        ol.AddToFamilyLists(p);
                                    }
                                }
                            }
                        }
                    }
                }
                if (p.p3dData.family.GetIndexOfPhysicalList(p.p3dData.off_objectList) != -1) {
                    p.p3dData.objectList = ObjectList.FromOffset(p.p3dData.off_objectList);
                }
            }

            Pointer.DoAt(ref reader, p.off_collSet, () => {
                p.collset = CollSet.Read(reader, p, p.off_collSet);
            });

            Pointer.DoAt(ref reader, p.off_sectInfo, () => {
                p.sectInfo = PersoSectorInfo.Read(reader, p.off_sectInfo);
            });

            return p;
        }

        public static Perso FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.persos.FirstOrDefault(f => f.offset == offset);
        }

        public IEnumerable<SearchableString> GetSearchableStrings()
        {
            var sl = new List<SearchableString>();

            var aiModel = brain?.mind?.AI_model;
            if (aiModel != null) {

                var behaviorNormalScripts = aiModel.behaviors_normal?.SelectMany(b => b.scripts) ?? new List<Script>();
                var behaviorReflexScripts = aiModel.behaviors_reflex?.SelectMany(b => b.scripts) ?? new List<Script>();
                var macroScripts = aiModel.macros?.Select(m => m.script) ?? new List<Script>();
                var scripts = behaviorNormalScripts.Concat(behaviorReflexScripts).Concat(macroScripts).Where(s=>s!=null);

                foreach (ScriptNode scriptNode in scripts.SelectMany(s=>s.scriptNodes)) {
                    var searchableString = scriptNode.GetSearchableString(this);
                    if (searchableString!=null) {
                        sl.Add(searchableString);
                    }
                }
                
                if (brain?.mind?.dsgMem != null) {

                    int dsgVarNum = 0;

                    var values = (brain.mind.dsgMem.values?.Length > 0) ? brain.mind.dsgMem.values : brain.mind.dsgMem.valuesInitial;

                    if (values != null) {
                        foreach (var value in values) {
                            var searchableString = value.GetSearchableString(this, dsgVarNum++);
                            if (searchableString != null) {
                                sl.AddRange(searchableString);
                            }
                        }
                    }
                }
            }

            return sl;

        }

        public void Write(Writer writer) {
            PersoBehaviour persoBehaviour = gao.GetComponent<PersoBehaviour>();
            if (p3dData != null && persoBehaviour != null && persoBehaviour.state!=null && persoBehaviour.state.offset!=null) {
                if (persoBehaviour.state.offset != p3dData.off_stateCurrent) {
                    p3dData.off_stateCurrent = persoBehaviour.state.offset;
                    p3dData.Write(writer);
                } /*else {
                    MapLoader.Loader.print("do not write state for perso " + fullName);
                }*/
            }

            if (persoBehaviour.clearTheBrain) {
                if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                   Pointer.Goto(ref writer, offset + 0x10);
                } else {
                    Pointer.Goto(ref writer, offset + 0xC);
                }
                Pointer.Write(writer, null);
                persoBehaviour.clearTheBrain = false;
            }

            CustomBitsComponent customBits = gao.GetComponent<CustomBitsComponent>();
            if (customBits != null && customBits.modified && stdGame != null) {
                stdGame.Write(writer);
            }
        }
    }
}
