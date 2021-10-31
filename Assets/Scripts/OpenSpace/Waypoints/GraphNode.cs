using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.Waypoints {
    public class GraphNode : ILinkedListEntry {
        public Pointer offset;
        public WayPoint wayPoint;
        public ArcList arcList;

        public uint typeOfWP;
        public uint typeOfWPInit;

        public Pointer off_nextNode;
        public Pointer off_prevNode;
        public Pointer off_graph;
        public Pointer off_node;
        public Pointer off_wayPoint;

        public Pointer off_arcList;

        public Pointer NextEntry {
            get {
                return off_nextNode;
            }
        }

        public Pointer PreviousEntry {
            get {
                return off_prevNode;
            }
        }

        public GraphNode(Pointer offset) {
            this.offset = offset;
        }

        public static GraphNode FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.graphNodes.FirstOrDefault(g => g.offset == offset);
        }

        public static GraphNode FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            GraphNode g = FromOffset(offset);
            if (g == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    g = GraphNode.Read(reader, offset);
                });
            }
            return g;
        }

        public static GraphNode Read(Reader reader, Pointer offset) {

            GraphNode node = new GraphNode(offset);

            MapLoader.Loader.graphNodes.Add(node);

            node.off_nextNode = Pointer.Read(reader);
            node.off_prevNode = Pointer.Read(reader);

            node.off_graph = Pointer.Read(reader);
            node.off_wayPoint = Pointer.Read(reader);
            if (Settings.s.engineVersion != Settings.EngineVersion.Montreal) {
                node.typeOfWP = reader.ReadUInt32();
                node.typeOfWPInit = reader.ReadUInt32();
            }
            node.off_arcList = Pointer.Read(reader);

            //MapLoader.Loader.print("ArcList: "+node.off_arcList);

            /*Pointer start = Pointer.Goto(ref reader, node.off_node);
            node.node = GraphNode.Read(reader, node.off_node);
            Pointer.Goto(ref reader, start);*/

            node.wayPoint = WayPoint.FromOffsetOrRead(node.off_wayPoint, reader);
            if (node.wayPoint != null) node.wayPoint.containingGraphNodes.Add(node);
            Pointer.DoAt(ref reader, node.off_arcList, () => {
                node.arcList = ArcList.Read(reader, node.off_arcList);
            });

            return node;
        }
    }
}