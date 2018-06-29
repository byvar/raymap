namespace OpenSpace.Waypoints {
    public class Graph {

        public Pointer offset;
        public GraphNode firstNode;
        public GraphNode lastNode;
        public int nodeCount;

        public Pointer off_firstNode;
        public Pointer off_lastNode;

        public GraphNode[] nodeList;

        public Graph(Pointer offset) // MicroStructure for Waypoint stuff, pointer to this is stored in Engine Object
        {
            this.offset = offset;
        }

        public static Graph Read(EndianBinaryReader reader, Pointer offset) {
            Graph graph = new Graph(offset);

            graph.off_firstNode = Pointer.Read(reader);
            graph.off_lastNode = Pointer.Read(reader);
            graph.nodeCount = reader.ReadInt32();

            graph.nodeList = new GraphNode[graph.nodeCount];

            Pointer currentPointer = graph.off_firstNode;

            for (int i = 0; i < graph.nodeCount; i++) {
                Pointer.Goto(ref reader, currentPointer);

                GraphNode node = GraphNode.Read(reader, currentPointer);
                graph.nodeList[i] = node;
                currentPointer = node.off_nextNode;
                if (currentPointer == null) {
                    break;
                }
            }

            return graph;
        }
    }
}