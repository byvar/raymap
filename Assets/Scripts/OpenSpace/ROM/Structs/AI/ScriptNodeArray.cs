using OpenSpace.AI;
using OpenSpace.Loader;
using OpenSpace.ROM.RSP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using static OpenSpace.AI.ScriptNode;

namespace OpenSpace.ROM {
	public class ScriptNodeArray : ROMStruct {
		public ushort length;
		public ScriptNode[] nodes;


		protected override void ReadInternal(Reader reader) {
			//Loader.print("Script @ " + Pointer.Current(reader) + " - len: " + length);
			nodes = new ScriptNode[length];
			for (int i = 0; i < nodes.Length; i++) {
				nodes[i] = new ScriptNode(reader);
			}
		}


        public class ScriptNode {
			// size: 4
			public byte type;
			public byte indent;
			public ushort param;

			// Custom
            public NodeType nodeType;
            public LegacyPointer offset;

			// Parsed param
			public Reference<Scr_Int> paramInt;
			public Reference<Scr_Real> paramFloat;
			public Reference<Script> paramScript;
			public Reference<Comport> paramComport;
			public Reference<AIModel> paramModel;
			public Reference<GameMaterial> paramGameMaterial;
			public Reference<Perso> paramPerso;
			public Reference<SuperObject> paramSuperObject;
			public Reference<WayPoint> paramWayPoint;
			public Reference<Graph> paramGraph;
			public Reference<Family> paramFamily;
			public Reference<ObjectsTable> paramObjectTable;
			public Reference<Scr_String> paramString;
			public Reference<State> paramAction;
			public Reference<LightInfo> paramLightInfo;
			public Reference<Scr_Vector3> paramVector3;
			public Reference<EntryAction> paramButton;

            public ScriptNode(Reader reader) {
                offset = LegacyPointer.Current(reader);

				type = reader.ReadByte();
				indent = reader.ReadByte();
				param = reader.ReadUInt16();

                nodeType = NodeType.Unknown;
                if (Legacy_Settings.s.aiTypes != null) nodeType = Legacy_Settings.s.aiTypes.GetNodeType(type);

				switch (nodeType) {
					case NodeType.GraphRef:
						paramGraph = new Reference<Graph>(param, reader, true);
						break;
					case NodeType.ModelRef:
						paramModel = new Reference<AIModel>(param, reader, true);
						break;
					case NodeType.ObjectTableRef:
						paramObjectTable = new Reference<ObjectsTable>(param, reader, true);
						break;
					case NodeType.SubRoutine:
						paramScript = new Reference<Script>(param, reader, true);
						break;
					case NodeType.WayPointRef:
						paramWayPoint = new Reference<WayPoint>(param, reader, true);
						break;
					case NodeType.PersoRef:
						paramPerso = new Reference<Perso>(param, reader, true);
						break;
					case NodeType.Constant:
						paramInt = new Reference<Scr_Int>(param, reader, true);
						break;
					case NodeType.Real:
						paramFloat = new Reference<Scr_Real>(param, reader, true);
						break;
					case NodeType.GameMaterialRef:
						paramGameMaterial = new Reference<GameMaterial>(param, reader, true);
						break;
					case NodeType.SuperObjectRef:
						paramSuperObject = new Reference<SuperObject>(param, reader, true);
						break;
					case NodeType.String:
						paramString = new Reference<Scr_String>(param, reader, true);
						break;
					case NodeType.ActionRef:
						paramAction = new Reference<State>(param, reader, true);
						break;
					case NodeType.FamilyRef:
						paramFamily = new Reference<Family>(param, reader, true);
						break;
					case NodeType.LightInfoRef:
						paramLightInfo = new Reference<LightInfo>(param, reader, true);
						break;
					case NodeType.ConstantVector:
						paramVector3 = new Reference<Scr_Vector3>(param, reader, true);
						break;
					case NodeType.Button:
						paramButton = new Reference<EntryAction>(param, reader, true);
						break;
				}


                /*try {
                    paramAsInt = new Reference<Int32ROMStruct>(param, reader, true).Value.value;
                    paramAsFloat = new Reference<FloatROMStruct>(param, reader, true).Value.value;
                } catch (KeyNotFoundException) {
                    Debug.LogWarning("Param " + param + " is not a valid reference");
                }*/
            }

            public bool ContentEquals(ScriptNode sn)
            {
                if (sn == null) return false;
                if (param != sn.param) return false;
                if (type != sn.type || indent != sn.indent) return false;
                return true;
            }

            private string ParamString => string.Format("0x{0:X4}", param);

            public string ToString(Perso perso, TranslatedROMScript.TranslationSettings ts, bool advanced = false)
            {
                R2ROMLoader l = Loader;
                short mask = 0;

                AITypes aiTypes = Legacy_Settings.s.aiTypes;

                Vector3 vector3 = new Vector3 { x = 0, y = 0, z = 0 };
                switch (nodeType) {
                    case NodeType.KeyWord: // KeyWordFunctionPtr
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
                    case NodeType.Condition: // GetConditionFunctionPtr
                        if (param < aiTypes.conditionTable.Length) { return aiTypes.conditionTable[param]; }
                        return "UnknownCondition_" + param;
                    case NodeType.Operator: // GetOperatorFunctionPtr
                        if (advanced) {
                            if (param < aiTypes.operatorTable.Length) { return aiTypes.operatorTable[param] + " (" + param + ")"; }
                        }
                        if (param < aiTypes.operatorTable.Length) { return aiTypes.operatorTable[param]; }
                        return "UnknownOperator_" + param;
                    case NodeType.Function: // GetFunctionFunctionPtr
                        if (param < aiTypes.functionTable.Length) { return aiTypes.functionTable[param]; }
                        return "UnknownFunction_" + param;
                    case NodeType.Procedure: // ProcedureFunctionReturn
                        if (param < aiTypes.procedureTable.Length) { return aiTypes.procedureTable[param]; }
                        return "UnknownProcedure_" + param;
                    case NodeType.MetaAction: // meta action
                        if (param < aiTypes.metaActionTable.Length) { return aiTypes.metaActionTable[param]; }
                        return "UnknownMetaAction_" + param;
                    case NodeType.BeginMacro:
                        return "BeginMacro";
                    case NodeType.EndMacro:
                        return "EndMacro";
                    case NodeType.Field:
                        if (param < aiTypes.fieldTable.Length) { return aiTypes.fieldTable[param]; }
                        return "UnknownField_" + param;
                    case NodeType.DsgVarRef: // Dsg Var
                        /*if (perso != null && perso.brain != null && perso.brain.mind != null) {
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
                        }*/
                        return "dsgVar_" + param;
                    case NodeType.Constant:
                        if (advanced) return "Constant: " + paramInt.Value?.value;
                        return paramInt.Value?.value + "";
                    case NodeType.Real:
                        NumberFormatInfo nfi = new NumberFormatInfo()
                        {
                            NumberDecimalSeparator = "."
                        };
                        return paramFloat.Value?.value.ToString(nfi) + "f";
                    case NodeType.Button: // Button/entryaction
						EntryAction ea = paramButton.Value;

                        if (ea == null) {
                            return "ERR_ENTRYACTION_NOTFOUND";
                        }

                        string eaName = (advanced ? ea.ToString() : ea.ToScriptString());
                        if (advanced) return "Button: " + eaName + "(" + ea.Offset + ")";
                        
                        if (!ts.expandEntryActions && ea != null) {
                            return "\"" + eaName + "\"";
                        }
						return eaName;
					case NodeType.ConstantVector:
						return "Constant Vector: " + paramVector3.Value?.value.ToString();
                    case NodeType.Vector:
                        return "new Vector3"; // TODO: same
                    case NodeType.Mask:
                        mask = (short)param; // TODO: as short
                        if (advanced) return "Mask: " + (mask).ToString("x4");
                        if (ts.exportMode) {
                            return "\"" + (mask).ToString("x4") + "\"";
                        }
                        return "Mask(" + (mask).ToString("x4") + ")";
                    case NodeType.ModuleRef:
                        if (advanced) return "ModuleRef: " + "0x" + (param).ToString("x8");
                        return "GetModule(" + (int)param + ")";
                    case NodeType.DsgVarId:
                        if (advanced) return "DsgVarId: " + "0x" + (param).ToString("x8");
                        return "DsgVarId(" + param + ")";
                    case NodeType.String:
                        return paramString.Value.ToString();
                    case NodeType.LipsSynchroRef:
                        return "LipsSynchroRef: " + param;
                    case NodeType.FamilyRef:
                        
                        return "Family.FromIndex(" + ParamString + ")";
                    case NodeType.PersoRef:
                        return "Perso.FromIndex(" + ParamString + ")";
                    case NodeType.ActionRef:
                        return "GetAction(" + ParamString + ")";
                    case NodeType.SuperObjectRef:
                        return "SuperObject.FromIndex(" + ParamString + ")";
                    case NodeType.WayPointRef:
                        return "WayPoint.FromIndex(" + ParamString + ")";
                    case NodeType.TextRef:
                        if (param == 0xFFFF || l.localizationROM == null) return "TextRef.Null";
                        /*if (advanced) return "TextRef: " + param + " (" + l.localizationROM.GetTextForHandleAndLanguageID((int)param, 0) + ")";
                        if (ts.expandStrings) {
                            return "\"" + l.localizationROM[0].GetTextForHandleAndLanguageID((int)param, 0) + "\""; // Preview in english
                        } else {
                            return "new TextReference(" + (int)param + ")";
                        }*/
                        int txtIndex = param;
                        string result = l.localizationROM.Lookup(txtIndex);
                        if (result != null) {
                            return "\"" + result + "\"";
                        } else {
                            return "TextRef_" + param;
                        }
                    case NodeType.ComportRef:

                        return "Comport.FromIndex(" + ParamString + ")";
                    case NodeType.SoundEventRef:
                        if (advanced) return "SoundEventRef: " + (int)param;
                        return "SoundEvent.FromID(0x" + ((int)param).ToString("X8") + ")";
                    case NodeType.ObjectTableRef:
                        

                        return "ObjectTable.FromIndex(" + ParamString + ")";
                    case NodeType.GameMaterialRef:
           
                        return "GameMaterial.FromIndex(" + ParamString + ")";
                    case NodeType.ParticleGenerator:
                        return "ParticleGenerator: " + "0x" + (param).ToString("x8");
                    case NodeType.VisualMaterial:
                        return "VisualMaterial.FromIndex(" + ParamString + ")";
                    case NodeType.ModelRef: // ModelCast

                        return "AIModel.FromIndex(" + ParamString + ")";
                    case NodeType.DataType42:
                        if (advanced) return "EvalDataType42: " + "0x" + (param).ToString("x8");
                        return "EvalDataType42(" + "0x" + (param).ToString("x8") + ")";
                    case NodeType.CustomBits:
                        if (advanced) return "CustomBits: " + "0x" + (param).ToString("x8");
                        if (ts.exportMode) {
                            return "0x" + (param).ToString("x8");
                        }
                        return "CustomBits(" + "0x" + (param).ToString("x8") + ")";
                    case NodeType.Caps:
                        if (advanced) return "Caps: " + "0x" + (param).ToString("x8");
                        if (ts.exportMode) {
                            return "0x" + (param).ToString("x8");
                        }
                        return "Caps(" + "0x" + (param).ToString("x8") + ")";
                    case NodeType.SubRoutine:
                        string macroString = "/* Subroutine */";
                        macroString += Environment.NewLine;
                        TranslatedROMScript macroScript = new TranslatedROMScript(paramScript.Value, perso);
                        macroString += macroScript.ToString();
                        macroString += Environment.NewLine + "/* End Subroutine */";
                        return macroString;
                    case NodeType.Null:
                        return "null";
                    case NodeType.GraphRef:
                        if (advanced) return "Graph: " + "0x" + (param).ToString("x8");
                        return "Graph.FromIndex(" + ParamString + ")";
                }

                return "unknown";
            }


            

            internal static bool IsNodeTypeVariable(NodeType nodeType)
            {
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
}
