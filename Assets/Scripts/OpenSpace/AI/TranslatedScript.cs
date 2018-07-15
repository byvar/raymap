using OpenSpace.EngineObject;
using System;
using System.Collections.Generic;

namespace OpenSpace.AI
{
    public class TranslatedScript
    {
        public Script originalScript;
        public Node[] nodes;

        public Perso perso;

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

            public string ToString()
            {
                if (scriptNode != null)
                {
                    string firstChildNode = (this.children.Count>0 && this.children[0]!=null) ? this.children[0].ToString() : "null";
                    string secondChildNode = (this.children.Count > 1 && this.children[1] != null) ? this.children[1].ToString() : "null";

                    switch (scriptNode.nodeType)
                    {
                        case ScriptNode.NodeType.KeyWord:
                            switch (scriptNode.param)
                            {
                                // If keywords
                                case 0: return "if ({condition})".Replace("{condition}", firstChildNode);
                                case 1: return "if (!({condition}))".Replace("{condition}", firstChildNode);
                                case 2: return "if (globalRandomizer % 2 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case 3: return "if (globalRandomizer % 4 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case 4: return "if (globalRandomizer % 8 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case 5: return "if (globalRandomizer % 16 == 0 && ({condition}))".Replace("{condition}", firstChildNode);
                                case 6: return "if (debug && {condition})".Replace("{condition}", firstChildNode);
                                case 7: return "if (!u64)\n{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                // Then
                                case 8: return "{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                // Else
                                case 9: return " else\n{\n{childNodes}\n}\n".Replace("{childNodes}", string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())));
                                default: return R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso); ;
                            }
                        case ScriptNode.NodeType.Condition:
                            switch (scriptNode.param)
                            {
                                // Boolean conditions:
                                case 0: return firstChildNode + " && " + secondChildNode;
                                case 1: return firstChildNode + " || " + secondChildNode;
                                case 2: return "!" + "(" + firstChildNode + ")";
                                case 3: return firstChildNode + " != " + secondChildNode; // XOR
                                // Real (float) comparisons:
                                case 4: return firstChildNode + " == " + secondChildNode;
                                case 5: return firstChildNode + " != " + secondChildNode;
                                case 6: return firstChildNode + " < " + secondChildNode;
                                case 7: return firstChildNode + " > " + secondChildNode;
                                case 8: return firstChildNode + " <= " + secondChildNode;
                                case 9: return firstChildNode + " >= " + secondChildNode;
                                default: return R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
                            }

                        case ScriptNode.NodeType.Function:

                            string func = R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
                            return func + "(" + string.Join(",", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";

                        case ScriptNode.NodeType.Procedure:

                            switch (scriptNode.param) {
                                default:
                                string proc = R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
                                return proc + "(" + string.Join(",", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";
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
                                default:
                                    string proc = "("+scriptNode.param+")"+R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
                                    return proc + "(" + string.Join(",", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString())) + ")";
                            }


                        default: return R2AITypes.readableFunctionSubTypeBasic(this.scriptNode, this.ts.perso);
                    }
                }
                else
                {
                    return string.Join("\n", Array.ConvertAll<Node, String>(this.children.ToArray(), x => x.ToString()));
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
            return nodes[0].ToString();
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