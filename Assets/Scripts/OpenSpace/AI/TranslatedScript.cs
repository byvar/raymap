using OpenSpace.EngineObject;
using OpenSpace.Input;
using System;
using System.Collections.Generic;

namespace OpenSpace.AI
{
    public class TranslatedScript
    {
        public Script originalScript;
        public Node[] nodes;

        public Perso perso;
        public bool printAddresses = false;

        public class Node
        {
            public int index;
            public int indent;
            public ScriptNode scriptNode;
            public List<Node> children;
            public TranslatedScript ts;

            public Node(int index, int indent, ScriptNode scriptNode, TranslatedScript ts)
            {
                this.index = index;
                this.indent = indent;
                this.scriptNode = scriptNode;
                this.children = new List<Node>();
                this.ts = ts;
            }

            public override string ToString() {
                if (scriptNode != null)
                {
                    string firstChildNode  = (this.children.Count > 0 && this.children[0] != null) ? this.children[0].ToString() : "null";
                    string secondChildNode = (this.children.Count > 1 && this.children[1] != null) ? this.children[1].ToString() : "null";
                    string prefix = (ts.printAddresses ? "{0x" + scriptNode.offset.offset.ToString("X8")  + "}" : "");

                    switch (scriptNode.nodeType)
                    {
                        case ScriptNode.NodeType.KeyWord:
                            switch (scriptNode.param)
                            {
                                // If keywords
                                case 0: return prefix+"if ({condition})".Replace("{condition}", firstChildNode);
                                case 1: return prefix + "if (!({condition}))".Replace("{condition}", firstChildNode);
                                case 2: return prefix + "if (globalRandomizer % 2 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case 3: return prefix + "if (globalRandomizer % 4 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case 4: return prefix + "if (globalRandomizer % 8 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case 5: return prefix + "if (globalRandomizer % 16 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case 6: return prefix + "if (debug && {condition})".Replace("{condition}", firstChildNode);
                                case 7: return prefix + "if (!u64)\n{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                // Then
                                case 8: return prefix + "{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                // Else
                                case 9: return prefix + "else\n{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                default: return prefix + R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso); ;
                            }
                        case ScriptNode.NodeType.Condition:
                            switch (scriptNode.param)
                            {
                                // Boolean conditions:
                                case 0: return prefix + firstChildNode + " && " + secondChildNode;
                                case 1: return prefix + firstChildNode + " || " + secondChildNode;
                                case 2: return prefix + "!" + "(" + firstChildNode + ")";
                                case 3: return firstChildNode + " != " + secondChildNode; // XOR
                                // Real (float) comparisons:
                                case 4: return prefix + firstChildNode + " == " + secondChildNode;
                                case 5: return prefix + firstChildNode + " != " + secondChildNode;
                                case 6: return prefix + firstChildNode + " < " + secondChildNode;
                                case 7: return prefix + firstChildNode + " > " + secondChildNode;
                                case 8: return prefix + firstChildNode + " <= " + secondChildNode;
                                case 9: return prefix + firstChildNode + " >= " + secondChildNode;
                                // Button condition:
                                case 44:
                                case 45:
                                case 46:
                                case 47:
                                    return prefix + firstChildNode;

                                default:
                                    if (firstChildNode!=null)
                                        return prefix + R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso) + "("+string.Join(",", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString()))+")";
                                    else
                                        return prefix + R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso)+"()";


                            }

                        case ScriptNode.NodeType.Function:

                            string func = R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
                            return prefix + func + "(" + string.Join(",", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";

                        case ScriptNode.NodeType.Procedure:

                            switch (scriptNode.param) {
                                default:
                                string proc = R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
                                return prefix + proc + "(" + string.Join(",", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";
                            }

                        case ScriptNode.NodeType.Operator:

                            switch (scriptNode.param)
                            {
                                // scalar:
                                case 0: return "(" + firstChildNode + "+" + secondChildNode + ")";
                                case 1: case 4: return "(" + firstChildNode + "-" + secondChildNode + ")";
                                case 2: return "(" + firstChildNode + "*" + secondChildNode + ")";
                                case 3: return "(" + firstChildNode + "/" + secondChildNode + ")";
                                // affect:
                                case 5: case 9: return firstChildNode + "+=" + secondChildNode;
                                case 6: case 10: return firstChildNode + "-=" + secondChildNode;
                                case 7: return firstChildNode + "*=" + secondChildNode;
                                case 8: return firstChildNode + "/=" + secondChildNode;
                                case 11: return firstChildNode + "=" + secondChildNode;
                                case 12: return firstChildNode + "." + secondChildNode;
                                case 13: return firstChildNode + ".x"; // vector
                                case 14: return firstChildNode + ".y"; // vector
                                case 15: return firstChildNode + ".z"; // vector
                                case 24: return firstChildNode + ".execute('\n{code}\n')".Replace("{code}", secondChildNode);
                                default:
                                    string proc = "("+scriptNode.param+")"+R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
                                    return prefix + proc + "(" + string.Join(",", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";
                            }

                        case ScriptNode.NodeType.Field:
                            if (firstChildNode != null)
                                return prefix + R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso) + "(" + string.Join(",", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";
                            else
                                return prefix + R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso) + "()";
                        default:
                            return prefix + R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
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

        public string ToString()
        {
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