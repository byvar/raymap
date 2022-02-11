using Newtonsoft.Json;
using OpenSpace.Exporter;
using OpenSpace.Input;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using OpenSpace.Visual;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class ScriptNode {
        public Pointer offset;
        public uint param;
        public byte type;
        public byte indent;

        // derived fields
        public Pointer param_ptr;
        public NodeType nodeType;

        public Script script;

        public ScriptNode(Pointer offset) {
            this.offset = offset;
        }

        public static ScriptNode Read(Reader reader, Pointer offset, Script script) {
            MapLoader l = MapLoader.Loader;
            ScriptNode sn = new ScriptNode(offset);

            sn.script = script;
            sn.param = reader.ReadUInt32();
            sn.param_ptr = Pointer.GetPointerAtOffset(offset); // if parameter is pointer
            if (CPA_Settings.s.platform == CPA_Settings.Platform.DC) reader.ReadUInt32();

            if (CPA_Settings.s.mode == CPA_Settings.Mode.Rayman3GC) {
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                sn.type = reader.ReadByte();

                reader.ReadByte();
                reader.ReadByte();
                sn.indent = reader.ReadByte();
                reader.ReadByte();
            } else {
                reader.ReadByte();
                reader.ReadByte();
                sn.indent = reader.ReadByte();
                sn.type = reader.ReadByte();
            }
            sn.nodeType = NodeType.Unknown;
            if (CPA_Settings.s.aiTypes != null) sn.nodeType = CPA_Settings.s.aiTypes.GetNodeType(sn.type);
			
            if (sn.param_ptr != null && sn.nodeType != NodeType.Unknown) {
                //l.print("ScriptNode " + offset + " - " + sn.nodeType + " (" + sn.type + ") - " + sn.param_ptr);
                if (sn.nodeType == NodeType.WayPointRef) {
                    WayPoint waypoint = WayPoint.FromOffsetOrRead(sn.param_ptr, reader);
                    waypoint.References.referencedByNodes.Add(sn);
                } else if (sn.nodeType == NodeType.String) {
                    Pointer.DoAt(ref reader, sn.param_ptr, () => {
                        string str = reader.ReadNullDelimitedString();
                        l.strings[sn.param_ptr] = str;
                    });
                } else if (sn.nodeType == NodeType.ObjectTableRef) {
                    // In R2 some objects have object tables that aren't listed normally, but are referenced through scripts.
                } else if (sn.nodeType == NodeType.Button) {
                    EntryAction.FromOffsetOrRead(sn.param_ptr, reader);
                } else if (sn.nodeType == NodeType.GameMaterialRef) {
                    GameMaterial.FromOffsetOrRead(sn.param_ptr, reader);
                } else if (sn.nodeType == NodeType.VisualMaterial) {
                    VisualMaterial.FromOffsetOrRead(sn.param_ptr, reader);
                } else if (sn.nodeType == NodeType.ComportRef) {
                    Behavior comportRef = l.FromOffsetOrRead<Behavior>(reader, sn.param_ptr);
                    comportRef.referencedBy.Add(script);
                } else if (sn.nodeType == NodeType.GraphRef) {
                    Graph.FromOffsetOrRead(sn.param_ptr, reader);
                }
            }

            l.onPostLoad.Add(sn.InitPostLoad);

            return sn;
        }

        public void InitPostLoad()
        {
            switch(nodeType) {
                case NodeType.GraphRef:
                    Graph graphRef = Graph.FromOffset(param_ptr);
                    if (graphRef != null) {
                        graphRef.References.referencedByNodes.Add(this);
                    } else {
                        Debug.LogWarning("Couldn't add ScriptNode reference to Graph offset " + param_ptr);
                    }
                    break;
                case NodeType.PersoRef:
                    Perso persoRef = Perso.FromOffset(param_ptr);
                    if (persoRef != null) {
                        persoRef.References.referencedByNodes.Add(this);
                    } else {
                        Debug.LogWarning("Couldn't add ScriptNode reference to Perso offset " + param_ptr);
                    }
                    break;
                case NodeType.SectorRef:
                    Sector sectorRef = Sector.FromOffset(param_ptr);
                    if (sectorRef != null) {
                        sectorRef.References.referencedByNodes.Add(this);
                    } else {
                        Debug.LogWarning("Couldn't add ScriptNode reference to Sector offset " + param_ptr);
                    }
                    break;
                case NodeType.SuperObjectRef:
                    SuperObject superObjectRef = SuperObject.FromOffset(param_ptr);
                    if (superObjectRef != null) {
                        superObjectRef.References.referencedByNodes.Add(this);
                    } else {
                        Debug.LogWarning("Couldn't add ScriptNode reference to SuperObject offset " + param_ptr);
                    }
                    break;
                default: return;
            }
        }

        public bool ContentEquals(ScriptNode sn) {
			if (sn == null) return false;
			if (param != sn.param || param_ptr != sn.param_ptr) return false;
			if (type != sn.type || indent != sn.indent) return false;
			return true;
		}

        public string ToString(Perso perso, TranslatedScript.TranslationSettings ts, bool advanced = false) {
            MapLoader l = MapLoader.Loader;
            short mask = 0;

            AITypes aiTypes = CPA_Settings.s.aiTypes;

            Vector3 vector3 = new Vector3 { x = 0, y = 0, z = 0 };
			switch (nodeType) {
				case ScriptNode.NodeType.KeyWord: // KeyWordFunctionPtr
					if (param < aiTypes.keywordTable.Length) {

                        if (ts.exportMode) {
                            if (aiTypes.keywordTable[param] == "Me") {
                                return "this";
                            }
                            if (aiTypes.keywordTable[param] == "MainActor") {
                                return "Controller.MainActor";
                            }
                            if (aiTypes.keywordTable[param] == "Nobody" || aiTypes.keywordTable[param] == "NoInput" || aiTypes.keywordTable[param] == "Nowhere" || aiTypes.keywordTable[param] == "NoGraph" || aiTypes.keywordTable[param] == "NoAction" || aiTypes.keywordTable[param] == "CapsNull") {
                                return "null";
                            }
                        }

                        return aiTypes.keywordTable[param];
                    }
					return "UnknownKeyword_" + param;
				case ScriptNode.NodeType.Condition: // GetConditionFunctionPtr
					if (param < aiTypes.conditionTable.Length) { return aiTypes.conditionTable[param]; }
					return "UnknownCondition_" + param;
				case ScriptNode.NodeType.Operator: // GetOperatorFunctionPtr
					if (advanced) {
						if (param < aiTypes.operatorTable.Length) { return aiTypes.operatorTable[param] + " (" + param + ")"; }
					}
					if (param < aiTypes.operatorTable.Length) { return aiTypes.operatorTable[param]; }
					return "UnknownOperator_" + param;
				case ScriptNode.NodeType.Function: // GetFunctionFunctionPtr
					if (param < aiTypes.functionTable.Length) { return aiTypes.functionTable[param]; }
					return "UnknownFunction_" + param;
				case ScriptNode.NodeType.Procedure: // ProcedureFunctionReturn
					if (param < aiTypes.procedureTable.Length) { return aiTypes.procedureTable[param]; }
					return "UnknownProcedure_" + param;
				case ScriptNode.NodeType.MetaAction: // meta action
					if (param < aiTypes.metaActionTable.Length) { return aiTypes.metaActionTable[param]; }
					return "UnknownMetaAction_" + param;
				case ScriptNode.NodeType.BeginMacro:
					return "BeginMacro";
				case ScriptNode.NodeType.EndMacro:
					return "EndMacro";
				case ScriptNode.NodeType.Field:
					if (param < aiTypes.fieldTable.Length) { return aiTypes.fieldTable[param]; }
					return "UnknownField_" + param;
				case ScriptNode.NodeType.DsgVarRef: // Dsg Var
					if (perso != null && perso.brain != null && perso.brain.mind != null) {
						Mind mind = perso.brain.mind;
						if (mind.dsgMem != null && mind.dsgMem.dsgVar != null) {
							if (param < mind.dsgMem.dsgVar.dsgVarInfos.Length) {
								return mind.dsgMem.dsgVar.dsgVarInfos[param].NiceVariableName;
							}
						} else if (mind.AI_model != null && mind.AI_model.dsgVar != null) {
							if (param < mind.AI_model.dsgVar.dsgVarInfos.Length) {
								return mind.AI_model.dsgVar.dsgVarInfos[param].NiceVariableName;
							}
						}
					}
					return "dsgVar_" + param;
				case ScriptNode.NodeType.Constant:
					if (advanced) return "Constant: " + BitConverter.ToInt32(BitConverter.GetBytes(param), 0);
					return BitConverter.ToInt32(BitConverter.GetBytes(param), 0).ToString();
				case ScriptNode.NodeType.Real:
					NumberFormatInfo nfi = new NumberFormatInfo() {
						NumberDecimalSeparator = "."
					};
					if (advanced) return "Real: " + BitConverter.ToSingle(BitConverter.GetBytes(param), 0).ToString(nfi);
                    return BitConverter.ToSingle(BitConverter.GetBytes(param), 0).ToString(nfi)+"f";
                case ScriptNode.NodeType.Button: // Button/entryaction
                    EntryAction ea = EntryAction.FromOffset(param_ptr);

                    if (ea == null) {
                        return "ERR_ENTRYACTION_NOTFOUND";
                    }

                    string eaName = (advanced ? ea.ToString() : ea.ToBasicString());
                    if (advanced) return "Button: " + eaName + "(" + param_ptr + ")";

                    if (!ts.expandEntryActions && ea!=null) {
                        return "\""+ea.ExportName+"\"";
                    }
                    return eaName;
                case ScriptNode.NodeType.ConstantVector:
                    return "Constant Vector: " + "0x" + param.ToString("x8"); // TODO: get from address
                case ScriptNode.NodeType.Vector:
                    return "new Vector3"; // TODO: same
                case ScriptNode.NodeType.Mask:
                    mask = (short)param; // TODO: as short
                    if(advanced) return "Mask: " + (mask).ToString("x4");
                    if (ts.exportMode) {
                        return "\"" + (mask).ToString("x4") + "\"";
                    }
                    return "Mask(" + (mask).ToString("x4") + ")";
                case ScriptNode.NodeType.ModuleRef:
                    if(advanced) return "ModuleRef: " + "0x" + (param).ToString("x8");
					return "GetModule(" + (int)param + ")";
                case ScriptNode.NodeType.DsgVarId:
                    if(advanced) return "DsgVarId: " + "0x" + (param).ToString("x8");
					return "DsgVarId(" + param + ")";
                case ScriptNode.NodeType.String:
                    string str = "ERR_STRING_NOTFOUND";
                    if (l.strings.ContainsKey(param_ptr)) str = l.strings[param_ptr];
                    if (advanced) return "String: " + param_ptr + " (" + str + ")";
                    return "\"" + str + "\"";
                case ScriptNode.NodeType.LipsSynchroRef:
                    return "LipsSynchroRef: " + param_ptr;
                case ScriptNode.NodeType.FamilyRef:
                    if(advanced) return "FamilyRef: " + param_ptr;
					Family f = Family.FromOffset(param_ptr);
					if (f != null) {
						return "GetFamily(\"" + f.name + "\")";
					} else {
						return "Family.FromOffset(\"" + param_ptr + "\")";
					}
                case ScriptNode.NodeType.PersoRef:
                    Perso argPerso = Perso.FromOffset(param_ptr);
                    if (argPerso != null && perso!=null && argPerso.offset == perso.offset) {
                        if (advanced) return "PersoRef: this";
                        return "this";
                    }
                    string persoName = argPerso == null ? "ERR_PERSO_NOTFOUND" : argPerso.fullName;
                    if (advanced) return "PersoRef: " + param_ptr + " (" + persoName + ")";
                    if (argPerso?.brain?.mind?.AI_model!=null ) {
                        AIModel aiModel = argPerso.brain.mind.AI_model;
                        // Make sure to add a cast in case the AI Model is accessed
                        return "(("+aiModel.name+")GetPerso(\"" + argPerso.namePerso + "\"))";
                    }
                    return "GetPerso(\"" + argPerso.namePerso + "\")";
                case ScriptNode.NodeType.ActionRef:
                    State state = State.FromOffset(param_ptr);
                    string stateName = state == null ? "ERR_STATE_NOTFOUND" : state.ShortName;
                    if (advanced) return "ActionRef: " + param_ptr + " " + stateName;
                    if (ts.useStateIndex) {
                        return "GetAction("+state.index.ToString()+")";
                    }
					return stateName;
                case ScriptNode.NodeType.SuperObjectRef:
                    if(advanced) return "SuperObjectRef: " + param_ptr;
					SuperObject so = SuperObject.FromOffset(param_ptr);
					if (so != null) {
						return "GetSuperObject(\"" + so.Gao.name + "\")";
					} else {
						return "SuperObject.FromOffset(\"" + param_ptr + "\")";
					}
				case ScriptNode.NodeType.WayPointRef:
                    if(advanced) return "WayPointRef: " + param_ptr;
					return "WayPoint.FromOffset(\"" + param_ptr + "\")";
                case ScriptNode.NodeType.TextRef:
                    if (l.localization == null) return "TextRef";
                    if (advanced) return "TextRef: " + param + " (" + l.localization.GetTextForHandleAndLanguageID((int)param, 0) + ")";
                    if (ts.expandStrings) {
                        return "\"" + l.localization.GetTextForHandleAndLanguageID((int)param, 0) + "\""; // Preview in english
                    } else {
                        return "new TextReference(" + (int)param + ")";
                    }
                case ScriptNode.NodeType.ComportRef:
					Behavior comportRef = l.FromOffset<Behavior>(param_ptr);

                    if (comportRef == null) {
                        if (advanced) return "ComportRef: " + param_ptr + " (null)";
                        return "null";
                    } else {
						return comportRef.ShortName;
                        //string type = comportRef.type == Behavior.BehaviorType.Normal ? "normalBehavior" : "reflexBehavior";
                        //return type + "[" + script.behaviorOrMacro.aiModel.GetBehaviorIndex(comportRef) + "]";
                    }
                case ScriptNode.NodeType.SoundEventRef:
                    if(advanced) return "SoundEventRef: " + (int)param;
					return "SoundEvent.FromID(0x" + ((int)param).ToString("X8") + ")";
                case ScriptNode.NodeType.ObjectTableRef:
					if (advanced) return "ObjectTableRef: " + param_ptr;

                    if (ts.useHashIdentifiers) {
                        string objectListJson = ObjectList.FromOffset(param_ptr).ToJSON();

                        string objectListHash = HashUtils.MD5Hash(objectListJson);
                        return "ObjectList.FromHash(\"" + objectListHash + "\")";
                    }

                    return "ObjectTable.FromOffset(\"" + param_ptr + "\")";
				case ScriptNode.NodeType.GameMaterialRef:
					if (advanced) return "GameMaterialRef: " + param_ptr;
                    if (ts.useHashIdentifiers) {
                        string gmtJson = GameMaterial.FromOffset(param_ptr).ToJSON();

                        string gmtHash = HashUtils.MD5Hash(gmtJson);
                        return "GameMaterial.FromHash(\"" + gmtHash + "\")";
                    }
					return "GameMaterial.FromOffset(\"" + param_ptr + "\")";
				case ScriptNode.NodeType.ParticleGenerator:
					return "ParticleGenerator: " + "0x" + (param).ToString("x8");
                case ScriptNode.NodeType.VisualMaterial:
					if (advanced) return "VisualMaterial: " + param_ptr;

                    if (ts.useHashIdentifiers) {
                        string vmtJson = VisualMaterial.FromOffset(param_ptr).ToJSON();

                        string vmtHash = HashUtils.MD5Hash(vmtJson);
                        return "VisualMaterial.FromHash(\"" + vmtHash + "\")";
                    }

                    return "VisualMaterial.FromOffset(\"" + param_ptr + "\")";
				case ScriptNode.NodeType.ModelRef: // ModelCast
                    if (advanced) return "AIModel: " + param_ptr;
                    AIModel model = l.FromOffset<AIModel>(param_ptr);
                    return model != null ? model.name : "null";
                case ScriptNode.NodeType.DataType42:
                    if(advanced) return "EvalDataType42: " + "0x" + (param).ToString("x8");
					return "EvalDataType42(" + "0x" + (param).ToString("x8") + ")";
				case ScriptNode.NodeType.CustomBits:
                    if(advanced) return "CustomBits: " + "0x" + (param).ToString("x8");
                    if (ts.exportMode) {
                        return "0x"+(param).ToString("x8");
                    }
					return "CustomBits(" + "0x" + (param).ToString("x8") + ")";
				case ScriptNode.NodeType.Caps:
                    if(advanced) return "Caps: " + "0x" + (param).ToString("x8");
                    if (ts.exportMode) {
                        return "0x" + (param).ToString("x8");
                    }
                    return "Caps(" + "0x" + (param).ToString("x8") + ")";
				case ScriptNode.NodeType.SubRoutine:
                    if (advanced) return "Eval SubRoutine: " + param_ptr;
                    Macro macro = l.FromOffset<Macro>(param_ptr);
                    if (macro == null) {
                        return "null";
                    }
                    return "evalMacro(" + macro.ShortName + ");";
                case ScriptNode.NodeType.Null:
                    return "null";
                case ScriptNode.NodeType.GraphRef:
                    if(advanced) return "Graph: " + "0x" + (param).ToString("x8");
					return "Graph.FromOffset(\"" + param_ptr + "\")";
			}

            return "unknown";
        }

        public SearchableString GetSearchableString(Perso p)
        {
            GameObject gao = (p.Gao.GetComponentsInChildren<ScriptComponent>().Where(c => c.script == script).FirstOrDefault()?.gameObject) ?? p.Gao;
            string str = "";
            string display = script.behaviorOrMacro.ToString();
            switch (nodeType) {
                case NodeType.String:
                    str = MapLoader.Loader.strings[param_ptr];
                    return new SearchableString(str, gao, display);
                case NodeType.TextRef:
                    str = MapLoader.Loader.localization?.GetTextForHandleAndLanguageID((int)param, 0);
                    return new SearchableString(str??"", gao, display);
                default:
                    return null;
            }
        }

        public enum NodeType {
            Unknown,
            KeyWord,
            Condition,
            Operator,
            Function,
            Procedure,
            MetaAction,
            BeginMacro,
            EndMacro,
            Field,
            DsgVarRef,
            Constant,
            Real,
            Button,
            ConstantVector,
            Vector,
            Mask,
            ModuleRef,
            DsgVarId,
            String,
            LipsSynchroRef,
            FamilyRef,
            PersoRef,
            ActionRef,
            SuperObjectRef,
            WayPointRef,
            TextRef,
            ComportRef,
            SoundEventRef,
            ObjectTableRef,
            GameMaterialRef,
            ParticleGenerator,
            VisualMaterial,
            ModelRef,
            DataType42,
            CustomBits,
            Caps,
            SubRoutine,
            Null,
            GraphRef,
            // Types below here added for engineversions < R2
            ConstantRef,
            RealRef,
            SurfaceRef,
            Way,
            DsgVar,
            SectorRef,
            EnvironmentRef,
            FontRef,
            Color,
            Module, // Different from ModuleRef
			LightInfoRef,

            // R3:
            EndTree,
            SOLinksRef,
            Light,
            Graph,
            CineRef
        };

        internal static bool IsNodeTypeVariable(NodeType nodeType) {
            switch (nodeType) {
                case NodeType.Unknown: return false;
                case NodeType.KeyWord: return false;
                case NodeType.Condition: return false;
                case NodeType.Operator: return false;
                case NodeType.Function: return false;
                case NodeType.Procedure: return false;
                case NodeType.MetaAction: return false;
                case NodeType.BeginMacro: return false;
                case NodeType.EndMacro: return false;
                case NodeType.SubRoutine: return false;
            }

            return true;
        }
    }
}
