using Newtonsoft.Json;
using OpenSpace.Object;

namespace OpenSpace.Waypoints {
    public class Graph : IReferenceable {

        public LegacyPointer offset;
        public LinkedList<GraphNode> nodes;
        public string name = null;
        public LegacyPointer off_name;
        public LegacyPointer off_wayPointName;

        [JsonIgnore]
        public ReferenceFields References { get; set; } = new ReferenceFields();

        public Graph(LegacyPointer offset) // MicroStructure for Waypoint stuff, pointer to this is stored in Engine Object
        {
            this.offset = offset;
        }

        public static Graph Read(Reader reader, LegacyPointer offset) {
            Graph graph = new Graph(offset);

            graph.nodes = LinkedList<GraphNode>.Read(ref reader, LegacyPointer.Current(reader), (off_element) => {
                return GraphNode.FromOffsetOrRead(off_element, reader);
            }, flags: LinkedList.Flags.HasHeaderPointers, type: LinkedList.Type.Double);
            if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3
                && CPA_Settings.s.platform != CPA_Settings.Platform.DC
                && CPA_Settings.s.platform != CPA_Settings.Platform.PS2) {
                graph.off_name = LegacyPointer.Read(reader);
                graph.off_wayPointName = LegacyPointer.Read(reader);
                LegacyPointer.DoAt(ref reader, graph.off_name, () => {
                    graph.name = reader.ReadNullDelimitedString();
                });
            }
            return graph;
        }

        public static Graph FromOffsetOrRead(LegacyPointer offset, Reader reader) {
            if (offset == null) return null;
            Graph g = FromOffset(offset);
            if (g == null) {
                LegacyPointer.DoAt(ref reader, offset, () => {
                    g = Graph.Read(reader, offset);
                    MapLoader.Loader.graphs.Add(g);
                });
            }
            return g;
        }

        public static Graph FromOffset(LegacyPointer offset) {
            MapLoader l = MapLoader.Loader;
            for (int i = 0; i < l.graphs.Count; i++) {
                if (offset == l.graphs[i].offset) return l.graphs[i];
            }
            return null;
        }
    }
}