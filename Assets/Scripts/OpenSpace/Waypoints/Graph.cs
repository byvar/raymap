using Newtonsoft.Json;
using OpenSpace.Object;

namespace OpenSpace.Waypoints {
    public class Graph : IReferenceable {

        public Pointer offset;
        public LinkedList<GraphNode> nodes;
        public string name = null;

        [JsonIgnore]
        public ReferenceFields References { get; set; } = new ReferenceFields();

        public Graph(Pointer offset) // MicroStructure for Waypoint stuff, pointer to this is stored in Engine Object
        {
            this.offset = offset;
        }

        public static Graph Read(Reader reader, Pointer offset) {
            Graph graph = new Graph(offset);

            graph.nodes = LinkedList<GraphNode>.Read(ref reader, Pointer.Current(reader), (off_element) => {
                return GraphNode.FromOffsetOrRead(off_element, reader);
            }, flags: LinkedList.Flags.HasHeaderPointers, type: LinkedList.Type.Double);

            return graph;
        }

        public static Graph FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            Graph g = FromOffset(offset);
            if (g == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    g = Graph.Read(reader, offset);
                    MapLoader.Loader.graphs.Add(g);
                });
            }
            return g;
        }

        public static Graph FromOffset(Pointer offset) {
            MapLoader l = MapLoader.Loader;
            for (int i = 0; i < l.graphs.Count; i++) {
                if (offset == l.graphs[i].offset) return l.graphs[i];
            }
            return null;
        }
    }
}