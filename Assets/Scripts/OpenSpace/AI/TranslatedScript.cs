using OpenSpace.Object;
using OpenSpace.Input;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenSpace.AI {
    public class TranslatedScript {
        public Script originalScript;
        public Node[] nodes;

        public Perso perso;
        public bool printAddresses = false;
        public bool expandMacros = false;
        public bool expandEntryActions = true;

        public class Node {
            public int index;
            public int indent;
            public ScriptNode scriptNode;
            public List<Node> children;
            public TranslatedScript ts;

            public Node(int index, int indent, ScriptNode scriptNode, TranslatedScript ts) {
                this.index = index;
                this.indent = indent;
                this.scriptNode = scriptNode;
                this.children = new List<Node>();
                this.ts = ts;
            }

            public override string ToString()
            {
                return this.ToString(this.ts.perso);
            }

            public string ToString(Perso perso) {
                if (scriptNode != null)
                {
                    string firstChildNode  = (this.children.Count > 0 && this.children[0] != null) ? this.children[0].ToString() : "null";
                    string secondChildNode = (this.children.Count > 1 && this.children[1] != null) ? this.children[1].ToString() : "null";
                    string prefix = (ts.printAddresses ? "{" + scriptNode.offset.ToString()  + "}" : "");

                    AITypes aiTypes = Settings.s.aiTypes;
                    uint param = scriptNode.param;

                    switch (scriptNode.nodeType)
                    {
                        case ScriptNode.NodeType.KeyWord:
                            string keyword = param < aiTypes.keywordTable.Length ? aiTypes.keywordTable[param] : "";
                            switch (keyword)
                            {
                                // If keywords
                                case "If":       return prefix + "if ({condition})".Replace("{condition}", firstChildNode);
                                case "IfNot":    return prefix + "if (!({condition}))".Replace("{condition}", firstChildNode);
                                case "If2":      return prefix + "if (globalRandomizer % 2 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "If4":      return prefix + "if (globalRandomizer % 4 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "If8":      return prefix + "if (globalRandomizer % 8 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "If16":     return prefix + "if (globalRandomizer % 16 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "If32":     return prefix + "if (globalRandomizer % 32 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "If64":     return prefix + "if (globalRandomizer % 64 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "IfNot2":   return prefix + "if (globalRandomizer % 2 != 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "IfNot4":   return prefix + "if (globalRandomizer % 4 != 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "IfNot8":   return prefix + "if (globalRandomizer % 8 != 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "IfNot16":  return prefix + "if (globalRandomizer % 16 != 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "IfNot32":  return prefix + "if (globalRandomizer % 32 != 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "IfNot64":  return prefix + "if (globalRandomizer % 64 != 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case "IfDebug":  return prefix + "if (debug && {condition})".Replace("{condition}", firstChildNode);
                                case "IfNotU64": return prefix + "if (!u64)\n{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                // Then
                                case "Then":     return prefix + "{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                // Else
                                case "Else":     return prefix + "else\n{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                default:         return prefix + scriptNode.ToString(perso);
                            }
                        case ScriptNode.NodeType.Condition:
                            string cond = param < aiTypes.conditionTable.Length ? aiTypes.conditionTable[param] : "";
                            switch (cond)
                            {
                                // Boolean conditions:
                                case "Cond_And":            return prefix + firstChildNode + " && " + secondChildNode;
                                case "Cond_Or":             return prefix + firstChildNode + " || " + secondChildNode;
                                case "Cond_Not":            return prefix + "!" + "(" + firstChildNode + ")";
                                case "Cond_XOR":            return prefix + firstChildNode + " != " + secondChildNode; // XOR
                                // Real (float) comparisons:
                                case "Cond_Equal":          return prefix + firstChildNode + " == " + secondChildNode;
                                case "Cond_Different":      return prefix + firstChildNode + " != " + secondChildNode;
                                case "Cond_Lesser":         return prefix + firstChildNode + " < " + secondChildNode;
                                case "Cond_Greater":        return prefix + firstChildNode + " > " + secondChildNode;
                                case "Cond_LesserOrEqual":  return prefix + firstChildNode + " <= " + secondChildNode;
                                case "Cond_GreaterOrEqual": return prefix + firstChildNode + " >= " + secondChildNode;
                                // Button condition:
                                /*case 44:
                                case 45:
                                case 46:
                                case 47:
                                    return prefix + firstChildNode;*/

                                default:
                                    if (firstChildNode!=null)
                                        return prefix + scriptNode.ToString(perso) + "("+string.Join(", ", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString()))+")";
                                    else
                                        return prefix + scriptNode.ToString(perso)+"()";


                            }

                        case ScriptNode.NodeType.Function:
                            string function = param < aiTypes.functionTable.Length ? aiTypes.functionTable[param] : "";
                            string ternaryCheck = "";

                            switch (function) {
                                // Ternary real operators (e.g. x > y ? true : false)
                                case "Func_TernInf":
                                case "Func_TernSup":
                                case "Func_TernEq":
                                case "Func_TernInfEq":
                                case "Func_TernSupEq":
                                    switch (function) {
                                        case "Func_TernInf":   ternaryCheck = " < "; break;
                                        case "Func_TernSup":   ternaryCheck = " > "; break;
                                        case "Func_TernEq":    ternaryCheck = " == "; break;
                                        case "Func_TernInfEq": ternaryCheck = " <= "; break;
                                        case "Func_TernSupEq": ternaryCheck = " >= "; break;
                                    }
                                    if (this.children.Count >= 4)
                                        return prefix + "((" + this.children[0] + ternaryCheck + this.children[1] + ") ? " + this.children[2] + " : " + this.children[3] + ")";
                                    else
                                        return "ERROR";

                                case "Func_TernOp": // conditional ternary operator (cond ? true : false)
                                    return prefix + "((" + this.children[0] + ") ? " + this.children[1] + " : " + this.children [2]+")";

                                default:
                                    string func = scriptNode.ToString(perso);
                                    return prefix + func + "(" + string.Join(", ", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";
                            }

                            

                        case ScriptNode.NodeType.Procedure:
                            string procedure = param < aiTypes.procedureTable.Length ? aiTypes.procedureTable[param] : "";
                            switch (procedure) {
                                case "Proc_Loop":    return prefix + "for(int i = 0; i < " + firstChildNode + "; i++)\n{";
                                case "Proc_EndLoop": return prefix + "}\n";
                                case "Proc_Break":   return prefix + "break;\n";

                                default:
                                    string proc = scriptNode.ToString(perso);
                                    return prefix + proc + "(" + string.Join(", ", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ");";

                            }

                        case ScriptNode.NodeType.Operator:
                            string op = param < aiTypes.operatorTable.Length ? aiTypes.operatorTable[param] : "";
                            Pointer persoPtr = null;

                            switch (op) {
                                // scalar:
                                case "Operator_Plus": return "(" + firstChildNode + (children[1].scriptNode.param>=0 ? " + " : "") + secondChildNode + ")";
                                case "Operator_Minus":
                                case "Operator_UnaryMinus":
                                    if (children.Count > 1) {
                                        return "(" + firstChildNode + " - " + secondChildNode + ")";
                                    } else {
                                        return "-" + firstChildNode;
                                    }
                                case "Operator_Mul": return "(" + firstChildNode + " * " + secondChildNode + ")";
                                case "Operator_Div": return "(" + firstChildNode + " / " + secondChildNode + ")";
                                case "Operator_Mod": return "(" + firstChildNode + " % " + secondChildNode + ")";
                                // affect:
                                case "Operator_PlusAffect":
                                case "Operator_PlusPlusAffect":
                                    return children.Count > 1 ? (firstChildNode + " += " + secondChildNode + ";") : firstChildNode + "++" + ";";
                                case "Operator_MinusAffect":
                                case "Operator_MinusMinusAffect":
                                    return children.Count > 1 ? (firstChildNode + " -= " + secondChildNode + ";") : firstChildNode + "--" + ";";
                                case "Operator_MulAffect": return firstChildNode + " *= " + secondChildNode + ";";
                                case "Operator_DivAffect": return firstChildNode + " /= " + secondChildNode + ";";
                                case "Operator_Affect": return firstChildNode + " = " + secondChildNode + ";";
                                case "Operator_Dot": // dot operator
                                    persoPtr = this.children[0].scriptNode.param_ptr;
                                    if (persoPtr != null) {
                                        Perso firstNodePerso = Perso.FromOffset(persoPtr);
                                        /*if (firstNodePerso != null) {
                                            string secondChildNodeWithDifferentContext = (this.children.Count > 1 && this.children[1] != null) ? this.children[1].ToString(firstNodePerso) : "null";
                                            return firstChildNode + "." + secondChildNodeWithDifferentContext;
                                        } else {*/
                                            return firstChildNode + "." + this.children[1].ToString(null);
                                        //}
                                    }

                                    return firstChildNode + "." + secondChildNode;
                                case ".X": return firstChildNode + ".x"; // vector
                                case ".Y": return firstChildNode + ".y"; // vector
                                case ".Z": return firstChildNode + ".z"; // vector
                                case "Operator_VectorPlusVector": return firstChildNode + " + " + secondChildNode;
                                case "Operator_VectorMinusVector": return firstChildNode + " - " + secondChildNode;
                                case "Operator_VectorMulScalar": return firstChildNode + " * " + secondChildNode;
                                case "Operator_VectorDivScalar": return firstChildNode + " / " + secondChildNode;
                                case "Operator_VectorUnaryMinus": return "-"+firstChildNode;
                                case ".X:=": return firstChildNode + ".x = " + secondChildNode; // vector
                                case ".Y:=": return firstChildNode + ".y = " + secondChildNode; // vector
                                case ".Z:=": return firstChildNode + ".z = " + secondChildNode; // vector
                                case "Operator_Ultra": // Ultra operator (execute code for different object)
                                    return firstChildNode + ".{code}".Replace("{code}", secondChildNode);
                                case "Operator_ModelCast": return "((" + firstChildNode + ")(" + secondChildNode + "))";
                                case "Operator_Array": return firstChildNode + "[" + secondChildNode + "]";

                                default:
                                    string proc = "("+scriptNode.param+")"+ scriptNode.ToString(perso);
                                    return prefix + proc + "(" + string.Join(", ", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ");";
                            }

                        case ScriptNode.NodeType.Field:
                            if (firstChildNode != null)
                                return prefix + scriptNode.ToString(perso) + "(" + string.Join(", ", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";
                            else
                                return prefix + scriptNode.ToString(perso);

                        case ScriptNode.NodeType.Vector:
                        case ScriptNode.NodeType.ConstantVector:

                            return prefix + scriptNode.ToString(perso) + "(" + string.Join(", ", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";

                        case ScriptNode.NodeType.MetaAction:

                            return prefix + scriptNode.ToString(perso) + "(" + string.Join(", ", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ");";

                        case ScriptNode.NodeType.SubRoutine:

                            if (ts.expandMacros) {

                                Macro macro = Macro.FromOffset(scriptNode.param_ptr);

                                if (macro != null) {
                                    string macroString = "// evalMacro(" + macro.ShortName + ");";
                                    macroString += Environment.NewLine;
                                    TranslatedScript macroScript = new TranslatedScript(macro.script, perso);
                                    macroScript.expandMacros = true;
                                    macroString += macroScript.ToString();
                                    macroString += Environment.NewLine + "// end macro";
                                    return macroString;
                                }
                            }

                            return prefix + scriptNode.ToString(perso);

                        default:
                            return prefix + scriptNode.ToString(perso, expandEntryActions: ts.expandEntryActions);
                    }
                }
                else // Root node returns all children concatenated
                {
                    string result = "";
                    foreach(Node child in this.children)
                    {
                        result += child.ToString() + '\n';
                    }
                    return result;
                }
            }
        }

        public TranslatedScript(Script originalScript, Perso perso)
        {
            if(originalScript==null) {
                return;
            }
            this.originalScript = originalScript;
            this.perso = perso;
            nodes = new Node[originalScript.scriptNodes.Count + 1];
            int index = 1;
            foreach(ScriptNode scriptNode in originalScript.scriptNodes)
            {
                nodes[index] = new Node(index, scriptNode.indent, scriptNode, this);
                index++;
            }

            Node rootNode = new Node(0, 0, null, this);
            nodes[0] = rootNode;
            AssignNodeChildren(nodes, nodes[0]);
        }
        
        public string ToCSharpString_R2()
        {
            string str = this.ToString();

            Regex macroRegex = new Regex("evalMacro\\([a-zA-Z0-9_]*\\.Macro\\[([0-9]+)\\]\\)");
            str = macroRegex.Replace(str, "await Macro_${1}()");

            Regex myRuleChangeRegex = new Regex("Proc_ChangeMyComport\\([a-zA-Z0-9_]+\\.Rule\\[([0-9]+)\\]\\[\\\"([^\"]+)\\\"\\]\\)");
            str = myRuleChangeRegex.Replace(str, "smRule.SetState(Rule_$1_$2)");

            Regex myReflexChangeRegex = new Regex("Proc_ChangeMyComportReflex\\([a-zA-Z0-9_]+\\.Reflex\\[([0-9]+)\\]\\[\\\"([^\"]+)\\\"\\]\\)");
            str = myReflexChangeRegex.Replace(str, "smReflex.SetState(Reflex_$1_$2)");

            Regex myRuleAndReflexChangeRegex = new Regex("Proc_ChangeMyComportAndMyReflex\\([a-zA-Z0-9_]+\\.Rule\\[([0-9]+)\\]\\[\"([^\"]+)\"\\], [a-zA-Z0-9_]+\\.Reflex\\[([0-9]+)\\]\\[\"([^\"]+)\"\\]\\)");
            str = myRuleAndReflexChangeRegex.Replace(str, "smRule.SetState(Rule_$1_$2);"+Environment.NewLine+"smReflex.SetState(Reflex_$3_$4)");

            Regex ruleChangeRegex = new Regex("Proc_ChangeComport\\(([^,]+), ([^.]+).Rule\\[([0-9]+)\\]\\[\\\"([^\"]+)\\\"]\\);");
            str = ruleChangeRegex.Replace(str, "Proc_ChangeComport($1, $2.Rule_$3_$4);");

            Regex reflexChangeRegex = new Regex("Proc_ChangeComportReflex\\(([^,]+), ([^.]+).Reflex\\[([0-9]+)\\]\\[\\\"([^\"]+)\\\"]\\);");
            str = reflexChangeRegex.Replace(str, "Proc_ChangeComportReflex($1, $2.Rule_$3_$4);");

            foreach (string metaAction in AITypes.R2.metaActionTable) {
                str = str.Replace(metaAction, "await "+metaAction);
            }

            Regex generateObjectRegex = new Regex("Func_GenerateObject\\(\\(\\(([^)]+)\\)[^,]+, ([^)]+)\\)");
            str = generateObjectRegex.Replace(str, "Func_GenerateObject(typeof($1), $2)");

            str = str.Replace(" Me", " this");
            str = str.Replace("(Me", "(this");

            str = str.Replace(" (1)", " (true)");
            str = str.Replace("((1)", "((true)");
            str = str.Replace(" (0)", " (false)");
            str = str.Replace("((0)", "((false)");

            str = str.Replace("Nobody", "null");

            Regex soundEventRegex = new Regex("SoundEvent\\(([0-9]+)\\)");
            str = soundEventRegex.Replace(str, "SoundEvent.GetByID($1)");   

            return str;
        }

        public override string ToString() {
            string result = nodes[0].ToString();
            // Trim down duplicate newlines
            result = result.Replace("\n\n", "\n");
            string[] resultLines = result.Split('\n');
            int currentIndent = 0;
            string formattedResult = "";
            foreach(string line in resultLines)
            {
                if (line == "}")
                {
                    currentIndent--;
                    if (currentIndent < 0)
                    {
                        currentIndent = 0;
                    }
                }
                formattedResult += new string(' ', currentIndent * 2) + line + '\n';
                if (line == "{")
                {
                    currentIndent++;
                }
            }
            return formattedResult.Substring(0, formattedResult.Length - 1); // remove last newline character
        }

        void AssignNodeChildren(Node[] nodes, Node node)
        {
            int nodeIndex = node.index;
            int indent = node.indent;
            int endIndex = nodes.Length;
            for (int i = nodeIndex + 1; i < nodes.Length; i++)
            {
                if (nodes[i].indent <= indent)
                {
                    break;
                }
                else if (nodes[i].indent == indent + 1)
                {
                    node.children.Add(nodes[i]);
                }
            }
            foreach (Node child in node.children)
            {
                AssignNodeChildren(nodes, child);
            }
        }
    }
}