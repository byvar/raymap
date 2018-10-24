using OpenSpace.Input;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
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
            if (Settings.s.platform == Settings.Platform.DC) reader.ReadUInt32();

            if (Settings.s.mode == Settings.Mode.Rayman3GC) {
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
            if (Settings.s.aiTypes != null) sn.nodeType = Settings.s.aiTypes.GetNodeType(sn.type);

            if (sn.param_ptr != null && sn.nodeType != NodeType.Unknown) {
                if (sn.nodeType == NodeType.WayPointRef) {
                    WayPoint waypoint = WayPoint.FromOffsetOrRead(sn.param_ptr, reader);
                } else if (sn.nodeType == NodeType.String) {
                    Pointer off_currentNode = Pointer.Goto(ref reader, sn.param_ptr);
                    string str = reader.ReadNullDelimitedString();
                    l.strings[sn.param_ptr] = str;
                    Pointer.Goto(ref reader, off_currentNode);
                } else if (sn.nodeType == NodeType.ObjectTableRef) {
                    // In R2 some objects have object tables that aren't listed normally, but are referenced through scripts.
                } else if (sn.nodeType == NodeType.Button) {
                    EntryAction.FromOffsetOrRead(sn.param_ptr, reader);
                }
            }
            return sn;
        }

        public string ToString(Perso perso, bool advanced = false) {
            MapLoader l = MapLoader.Loader;
            short mask = 0;

            AITypes aiTypes = Settings.s.aiTypes;

            Vector3 vector3 = new Vector3 { x = 0, y = 0, z = 0 };
            switch (nodeType) {
                case ScriptNode.NodeType.KeyWord: // KeyWordFunctionPtr
                    if (param < aiTypes.keywordTable.Length) { return aiTypes.keywordTable[param]; }
                    return "Unknown Keyword (" + param + ")";
                case ScriptNode.NodeType.Condition: // GetConditionFunctionPtr
                    if (param < aiTypes.conditionTable.Length) { return aiTypes.conditionTable[param]; }
                    return "Unknown Condition (" + param + ")";
                case ScriptNode.NodeType.Operator: // GetOperatorFunctionPtr
                    if (advanced) {
                        if (param < aiTypes.operatorTable.Length) { return aiTypes.operatorTable[param] + " (" + param + ")"; }
                    }
                    if (param < aiTypes.operatorTable.Length) { return aiTypes.operatorTable[param]; }
                    return "Unknown Operator (" + param + ")";
                case ScriptNode.NodeType.Function: // GetFunctionFunctionPtr
                    if (param < aiTypes.functionTable.Length) { return aiTypes.functionTable[param]; }
                    return "Unknown Function (" + param + ")";
                case ScriptNode.NodeType.Procedure: // ProcedureFunctionReturn
                    if (param < aiTypes.procedureTable.Length) { return aiTypes.procedureTable[param]; }
                    return "Unknown Procedure (" + param + ")";
                case ScriptNode.NodeType.MetaAction: // meta action
                    if (param < aiTypes.metaActionTable.Length) { return aiTypes.metaActionTable[param]; }
                    return "Unknown Meta Action (" + param + ")";
                case ScriptNode.NodeType.BeginMacro:
                    return "Begin Macro";
                case ScriptNode.NodeType.EndMacro:
                    return "End Macro";
                case ScriptNode.NodeType.Field:
                    if (param < aiTypes.fieldTable.Length) { return aiTypes.fieldTable[param]; }
                    return "Unknown Field (" + param + ")";
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
                    if (advanced) return "Constant: " + param;
                    return param.ToString();
                case ScriptNode.NodeType.Real:
                    if (advanced) return "Real: " + BitConverter.ToSingle(BitConverter.GetBytes(param), 0);
                    return BitConverter.ToSingle(BitConverter.GetBytes(param), 0).ToString();
                case ScriptNode.NodeType.Button: // Button/entryaction
                    EntryAction ea = EntryAction.FromOffset(param_ptr);
                    string eaName = ea == null ? "ERR_ENTRYACTION_NOTFOUND" : (advanced ? ea.ToString() : ea.ToBasicString());
                    if (advanced) return "Button: " + eaName + "(" + param_ptr + ")";
                    return eaName;
                case ScriptNode.NodeType.ConstantVector:
                    return "Constant Vector: " + "0x" + param.ToString("x8"); // TODO: get from address
                case ScriptNode.NodeType.Vector:
                    return "new Vector3"; // TODO: same
                case ScriptNode.NodeType.Mask:
                    mask = (short)param; // TODO: as short
                    return "Mask: " + (mask).ToString("x4");
                case ScriptNode.NodeType.ModuleRef:
                    return "ModuleRef: " + "0x" + (param).ToString("x8");
                case ScriptNode.NodeType.DsgVarId:
                    return "DsgVarId: " + "0x" + (param).ToString("x8");
                case ScriptNode.NodeType.String:
                    string str = "ERR_STRING_NOTFOUND";
                    if (l.strings.ContainsKey(param_ptr)) str = l.strings[param_ptr];
                    if (advanced) return "String: " + param_ptr + " (" + str + ")";
                    return "\"" + str + "\"";
                case ScriptNode.NodeType.LipsSynchroRef:
                    return "LipsSynchroRef: " + param_ptr;
                case ScriptNode.NodeType.FamilyRef:
                    return "FamilyRef: " + param_ptr;
                case ScriptNode.NodeType.PersoRef:
                    Perso argPerso = Perso.FromOffset(param_ptr);
                    if (argPerso != null && argPerso.offset == perso.offset) {
                        if (advanced) return "PersoRef: this";
                        return "this";
                    }
                    string persoName = argPerso == null ? "ERR_PERSO_NOTFOUND" : argPerso.fullName;
                    if (advanced) return "PersoRef: " + param_ptr + " (" + persoName + ")";
                    return "getPersoByName(\"" + argPerso.namePerso + "\")";
                case ScriptNode.NodeType.ActionRef:
                    State state = State.FromOffset(param_ptr);
                    string stateName = state == null ? "ERR_STATE_NOTFOUND" : state.name;
                    int stateIndex = state.index;
                    if (advanced) return "ActionRef: " + param_ptr + " [" + stateIndex + "](" + stateName + ")";
                    return "action[" + stateIndex + "]";
                case ScriptNode.NodeType.SuperObjectRef:
                    return "SuperObjectRef: " + param_ptr;
                case ScriptNode.NodeType.WayPointRef:
                    return "WayPointRef: " + param_ptr;
                case ScriptNode.NodeType.TextRef:
                    if (l.fontStruct == null) return "TextRef";
                    if (advanced) return "TextRef: " + param + " (" + l.fontStruct.GetTextForHandleAndLanguageID((int)param, 0) + ")";
                    return "TextRef: " + l.fontStruct.GetTextForHandleAndLanguageID((int)param, 0); // Preview in english
                case ScriptNode.NodeType.ComportRef:
                    Behavior comportRef = script.behaviorOrMacro.aiModel.GetBehaviorByOffset(param_ptr);
                    if (comportRef == null) {
                        if (advanced) return "ComportRef: " + param_ptr + " (null)";
                        return "null";
                    } else {
                        string type = comportRef.type == Behavior.BehaviorType.Normal ? "normalBehavior" : "reflexBehavior";
                        return type + "[" + comportRef.number + "]";
                    }
                case ScriptNode.NodeType.SoundEventRef:
                    return "SoundEventRef: " + param_ptr;
                case ScriptNode.NodeType.ObjectTableRef:
                    return "ObjectTableRef: " + param_ptr;
                case ScriptNode.NodeType.GameMaterialRef:
                    return "GameMaterialRef: " + param_ptr;
                case ScriptNode.NodeType.ParticleGenerator:
                    return "ParticleGenerator: " + "0x" + (param).ToString("x8");
                case ScriptNode.NodeType.VisualMaterial:
                    return "VisualMaterial: " + param_ptr;
                case ScriptNode.NodeType.ModelRef: // ModelCast
                    if (advanced) return "AIModel: " + param_ptr;
                    AIModel model = AIModel.FromOffset(param_ptr);
                    return model != null ? model.name : "null";
                case ScriptNode.NodeType.DataType42:
                    return "EvalDataType42: " + "0x" + (param).ToString("x8");
                case ScriptNode.NodeType.CustomBits:
                    return "CustomBits: " + "0x" + (param).ToString("x8");
                case ScriptNode.NodeType.Caps:
                    return "Caps: " + "0x" + (param).ToString("x8");
                case ScriptNode.NodeType.SubRoutine:
                    if (advanced) return "Eval SubRoutine: " + param_ptr;
                    Macro macro = script.behaviorOrMacro.aiModel.GetMacroByOffset(param_ptr);
                    if (macro == null) {
                        return "null";
                    }
                    return "evalMacro(" + macro.number + ");";
                case ScriptNode.NodeType.Null:
                    return "null";
                case ScriptNode.NodeType.GraphRef:
                    return "Graph: " + "0x" + (param).ToString("x8");
            }

            return "unknown";
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
            Module // Different from ModuleRef
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
