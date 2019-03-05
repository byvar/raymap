using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.Exporter {
    public class SerializedGraphData {

        public Dictionary<string, SerializedGraphData.EGraph> Graphs;
        public Dictionary<string, SerializedGraphData.EGraphNode> GraphNodes;
        public Dictionary<string, SerializedGraphData.EWayPoint> Waypoints;

        public struct EGraphNode {
            public string wayPointReference;
            public EArc[] arcs;
        }

        public struct EArc {
            public uint capabilities;
            public int weight;
            public string targetGraphNodeReference;
        }

        public struct EWayPoint {
            public Vector3 position;
            public float radius;
        }

        public struct EGraph {
            public string[] graphNodeReferences;
        }

        public SerializedGraphData(List<Graph> graphs, List<GraphNode> graphNodes, List<WayPoint> wayPoints)
        {
            Graphs = new Dictionary<string, SerializedGraphData.EGraph>();
            GraphNodes = new Dictionary<string, SerializedGraphData.EGraphNode>();
            Waypoints = new Dictionary<string, SerializedGraphData.EWayPoint>();

            foreach (Graph graph in graphs) {
                EGraph exportableGraph = new EGraph();

                var graphNodeReferences = new List<string>();
                foreach (GraphNode gn in graph.nodes) {
                    graphNodeReferences.Add(gn.offset.ToString());
                }
                exportableGraph.graphNodeReferences = graphNodeReferences.ToArray();
                Graphs.Add(graph.offset.ToString(), exportableGraph);
            }

            foreach (GraphNode graphNode in graphNodes) {
                EGraphNode exportableGraphNode = new EGraphNode();
                exportableGraphNode.wayPointReference = graphNode.wayPoint.offset.ToString();
                var exportableArcs = new List<EArc>();

                foreach (Arc arc in graphNode.arcList.list) {
                    exportableArcs.Add(new EArc()
                    {
                        capabilities = arc.capabilities,
                        targetGraphNodeReference = arc.graphNode.offset.ToString(),
                        weight = arc.weight
                    });
                }

                exportableGraphNode.arcs = exportableArcs.ToArray();

                GraphNodes.Add(graphNode.offset.ToString(), exportableGraphNode);
            }

            foreach (WayPoint wp in wayPoints) {
                EWayPoint exportableWayPoint = new EWayPoint();
                exportableWayPoint.position = wp.position;
                exportableWayPoint.radius = wp.radius;
                Waypoints.Add(wp.offset.ToString(), exportableWayPoint);
            }
        }
    }
}
