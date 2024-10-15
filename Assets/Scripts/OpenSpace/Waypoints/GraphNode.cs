using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.Waypoints {
    public class GraphNode : ILinkedListEntry {
        public LegacyPointer offset;
        public WayPoint wayPoint;
        public ArcList arcList;

        public uint typeOfWP;
        public uint typeOfWPInit;

        public LegacyPointer off_nextNode;
        public LegacyPointer off_prevNode;
        public LegacyPointer off_graph;
        public LegacyPointer off_node;
        public LegacyPointer off_wayPoint;

        public LegacyPointer off_arcList;

        public LegacyPointer NextEntry {
            get {
                return off_nextNode;
            }
        }

        public LegacyPointer PreviousEntry {
            get {
                return off_prevNode;
            }
        }

        public GraphNode(LegacyPointer offset) {
            this.offset = offset;
        }

        public static GraphNode FromOffset(LegacyPointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.graphNodes.FirstOrDefault(g => g.offset == offset);
        }

        public static GraphNode FromOffsetOrRead(LegacyPointer offset, Reader reader) {
            if (offset == null) return null;
            GraphNode g = FromOffset(offset);
            if (g == null) {
                LegacyPointer.DoAt(ref reader, offset, () => {
                    g = GraphNode.Read(reader, offset);
                });
            }
            return g;
        }

        public static GraphNode Read(Reader reader, LegacyPointer offset) {

            GraphNode node = new GraphNode(offset);

            MapLoader.Loader.graphNodes.Add(node);

            node.off_nextNode = LegacyPointer.Read(reader);
            node.off_prevNode = LegacyPointer.Read(reader);

            node.off_graph = LegacyPointer.Read(reader);
            node.off_wayPoint = LegacyPointer.Read(reader);
            if (Legacy_Settings.s.engineVersion != Legacy_Settings.EngineVersion.Montreal) {
                node.typeOfWP = reader.ReadUInt32();
				if (Legacy_Settings.s.game != Legacy_Settings.Game.R2Beta) {
					node.typeOfWPInit = reader.ReadUInt32();
				}
            }
			node.off_arcList = LegacyPointer.Read(reader);

            //MapLoader.Loader.print("ArcList: "+node.off_arcList);

            /*Pointer start = Pointer.Goto(ref reader, node.off_node);
            node.node = GraphNode.Read(reader, node.off_node);
            Pointer.Goto(ref reader, start);*/

            node.wayPoint = WayPoint.FromOffsetOrRead(node.off_wayPoint, reader);
            if (node.wayPoint != null) node.wayPoint.containingGraphNodes.Add(node);
            LegacyPointer.DoAt(ref reader, node.off_arcList, () => {
                node.arcList = ArcList.Read(reader, node.off_arcList);
            });

            return node;
        }
    }
}