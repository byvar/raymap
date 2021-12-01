using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Editor;
using UnityEditor;
using OpenSpace.Waypoints;

[CustomEditor(typeof(GraphBehaviour))]
public class GraphBehaviourEditor : Editor {
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GraphBehaviour graphBehaviour = (GraphBehaviour)target;

        if (GUILayout.Button("Copy offsets to Cheat Engine table")) {
            //string text = "";

            CETableBuilder cb = new CETableBuilder();

            int nodeCount = 0;

            var nodes = new List<CETableBuilder.TableEntry>();

            foreach (var node in graphBehaviour.graph.nodes) {

                var nodeProperties = new List<CETableBuilder.TableEntry>();

                int arcCount = 0;

                foreach (var arc in node.arcList.list) {
                    var arcProperties = new List<CETableBuilder.TableEntry>
                    {
                        new CETableBuilder.TableEntry("Capabilities", arc.offset + 16,
                            CETableBuilder.CEVarType._4_Bytes, true),
                        new CETableBuilder.TableEntry("Weight", arc.offset + 20,
                            CETableBuilder.CEVarType._4_Bytes, true)
                    };

                    nodeProperties.Add(new CETableBuilder.TableEntry()
                    {
                        Address = arc.offset,
                        Name = $"Arc {arcCount}",
                        Children = arcProperties,
                        Type = CETableBuilder.CEVarType._4_Bytes
                    });

                    arcCount++;
                }

                nodeProperties.Add(new CETableBuilder.TableEntry("Waypoint", node.offset + 12,
                    CETableBuilder.CEVarType._4_Bytes, true)
                {
                    Children = new List<CETableBuilder.TableEntry>()
                    {
                        new CETableBuilder.TableEntry("X", node.wayPoint.offset + 0, CETableBuilder.CEVarType.Float, false),
                        new CETableBuilder.TableEntry("Y", node.wayPoint.offset + 4 , CETableBuilder.CEVarType.Float, false),
                        new CETableBuilder.TableEntry("Z", node.wayPoint.offset + 8, CETableBuilder.CEVarType.Float, false),
                        new CETableBuilder.TableEntry("Radius", node.wayPoint.offset + 12, CETableBuilder.CEVarType.Float, false),
                    }
                });
                nodeProperties.Add(new CETableBuilder.TableEntry("Type of WP", node.offset + 16,
                    CETableBuilder.CEVarType._4_Bytes, true));
                nodeProperties.Add(new CETableBuilder.TableEntry("Type of WP Init", node.offset + 20,
                    CETableBuilder.CEVarType._4_Bytes, true));

                nodes.Add(new CETableBuilder.TableEntry
                {
                    Address = node.offset,
                    Name = $"Node {nodeCount}",
                    Type = CETableBuilder.CEVarType._4_Bytes,
                    Children = nodeProperties,
                });

                nodeCount++;
            }

            cb.Entries = nodes;
            EditorGUIUtility.systemCopyBuffer = cb.Build();

        }
        
    }
}