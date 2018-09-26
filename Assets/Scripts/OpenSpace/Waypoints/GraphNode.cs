
using System.Linq;

namespace OpenSpace.Waypoints {
    public class GraphNode {
        public Pointer offset;
        public GraphNode nextNode;
        public GraphNode previousNode;
        public GraphNode node;
        public WayPoint wayPoint;
        public ArcList arcList;

        public Pointer off_nextNode;
        public Pointer off_prevNode;
        public Pointer off_node;
        public Pointer off_wayPoint;

        public Pointer off_arcList;

        public GraphNode(Pointer offset) {
            this.offset = offset;
        }

        public static GraphNode FromOffset(Pointer offset)
        {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.graphNodes.FirstOrDefault(g => g.offset == offset);
        }

        public static GraphNode FromOffsetOrRead(Pointer offset, Reader reader)
        {
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

            //node.off_node = Pointer.Read(reader);
            reader.ReadUInt32();

            node.off_wayPoint = Pointer.Read(reader);
            reader.ReadUInt32();
            reader.ReadUInt32();
            node.off_arcList = Pointer.Read(reader);

            //MapLoader.Loader.print("ArcList: "+node.off_arcList);

            /*Pointer start = Pointer.Goto(ref reader, node.off_node);
            node.node = GraphNode.Read(reader, node.off_node);
            Pointer.Goto(ref reader, start);*/

            Pointer start = Pointer.Goto(ref reader, node.off_wayPoint);
            node.wayPoint = WayPoint.Read(reader, node.off_wayPoint);
            Pointer.Goto(ref reader, start);

            start = Pointer.Goto(ref reader, node.off_arcList);
            node.arcList = ArcList.Read(reader, node.off_arcList);
            Pointer.Goto(ref reader, start);

            return node;
        }
    }
}