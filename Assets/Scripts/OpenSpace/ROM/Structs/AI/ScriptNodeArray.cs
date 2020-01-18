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
			Loader.print("Script @ " + Pointer.Current(reader) + " - len: " + length);
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
            public NodeType nodeType;
            public long offset;

            public int paramAsInt;
            public float paramAsFloat;

            public ScriptNode(Reader reader) {

                offset = reader.BaseStream.Position;

				type = reader.ReadByte();
				indent = reader.ReadByte();
				param = reader.ReadUInt16();

                nodeType = NodeType.Unknown;
                if (Settings.s.aiTypes != null) nodeType = Settings.s.aiTypes.GetNodeType(type);

                try {
                    paramAsInt = new Reference<Int32ROMStruct>(param, reader, true).Value.value;
                    paramAsFloat = new Reference<FloatROMStruct>(param, reader, true).Value.value;
                } catch (KeyNotFoundException e) {
                    Debug.LogWarning("Param " + param + " is not a valid reference");
                }
            }

            public bool ContentEquals(ScriptNode sn)
            {
                if (sn == null) return false;
                if (param != sn.param) return false;
                if (type != sn.type || indent != sn.indent) return false;
                return true;
            }

            public string ToString(Perso perso, TranslatedROMScript.TranslationSettings ts, bool advanced = false)
            {
                MapLoader l = MapLoader.Loader;
                short mask = 0;

                AITypes aiTypes = Settings.s.aiTypes;

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
                        if (advanced) return "Constant: " + BitConverter.ToInt32(BitConverter.GetBytes(paramAsInt), 0);
                        var bytes = BitConverter.GetBytes(paramAsInt);
                        return BitConverter.ToInt32(bytes, 0).ToString()+" ("+param+")";
                    case NodeType.Real:
                        NumberFormatInfo nfi = new NumberFormatInfo()
                        {
                            NumberDecimalSeparator = "."
                        };
                        return BitConverter.ToSingle(BitConverter.GetBytes(paramAsFloat), 0).ToString(nfi) + "f" + " (" + param + ")";
                    case NodeType.Button: // Button/entryaction
                        /*EntryAction ea = EntryAction.FromOffset(param_ptr);

                        if (ea == null) {
                            return "ERR_ENTRYACTION_NOTFOUND";
                        }

                        string eaName = (advanced ? ea.ToString() : ea.ToBasicString());
                        if (advanced) return "Button: " + eaName + "(" + param_ptr + ")";
                        
                        if (!ts.expandEntryActions && ea != null) {
                            return "\"" + ea.ExportName + "\"";
                        }
                        */
                        return "entryaction_"+param;
                    case NodeType.ConstantVector:
                        return "Constant Vector: " + "0x" + param.ToString("x8"); // TODO: get from address
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
                        return "\"str_" + param+ "\"";
                    case NodeType.LipsSynchroRef:
                        return "LipsSynchroRef: " + param;
                    case NodeType.FamilyRef:
                        
                        return "Family.FromOffset(\"" + param + "\")";
                    case NodeType.PersoRef:
                        return "Perso.FromOffset(\"" + param + "\")";
                    case NodeType.ActionRef:
                        return "GetAction(" + param + ")";
                    case NodeType.SuperObjectRef:
                        return "SuperObject.FromOffset(\"" + param + "\")";
                    case NodeType.WayPointRef:
                        return "WayPoint.FromOffset(\"" + param + "\")";
                    case NodeType.TextRef:
                        if (l.fontStruct == null) return "TextRef";
                        if (advanced) return "TextRef: " + param + " (" + l.fontStruct.GetTextForHandleAndLanguageID((int)param, 0) + ")";
                        if (ts.expandStrings) {
                            return "\"" + l.fontStruct.GetTextForHandleAndLanguageID((int)param, 0) + "\""; // Preview in english
                        } else {
                            return "new TextReference(" + (int)param + ")";
                        }
                    case NodeType.ComportRef:

                        return "Comport.FromOffset(\"" + param + "\")";
                    case NodeType.SoundEventRef:
                        if (advanced) return "SoundEventRef: " + (int)param;
                        return "SoundEvent.FromID(0x" + ((int)param).ToString("X8") + ")";
                    case NodeType.ObjectTableRef:
                        

                        return "ObjectTable.FromOffset(\"" + param + "\")";
                    case NodeType.GameMaterialRef:
           
                        return "GameMaterial.FromOffset(\"" + param + "\")";
                    case NodeType.ParticleGenerator:
                        return "ParticleGenerator: " + "0x" + (param).ToString("x8");
                    case NodeType.VisualMaterial:
                        return "VisualMaterial.FromOffset(\"" + param + "\")";
                    case NodeType.ModelRef: // ModelCast

                        return "AIModel.FromOffset(\"" + param + "\")";
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
                        return "Macro";
                    case NodeType.Null:
                        return "null";
                    case NodeType.GraphRef:
                        if (advanced) return "Graph: " + "0x" + (param).ToString("x8");
                        return "Graph.FromOffset(\"" + param + "\")";
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
