namespace OpenSpace.Waypoints {
    public class MSWay {

        public Pointer offset;
        public Graph graph;
        public uint currentIndex;
        public byte someFlag;

        public Pointer off_graph;

        public MSWay(Pointer offset) // MicroStructure for Waypoint stuff, pointer to this is stored in Engine Object
        {
            this.offset = offset;
        }

        public static MSWay Read(Reader reader, Pointer offset) {
            MSWay msWay = new MSWay(offset);

            msWay.off_graph = Pointer.Read(reader);
            msWay.graph = Graph.FromOffsetOrRead(msWay.off_graph, reader);
            msWay.currentIndex = reader.ReadUInt32();
            msWay.someFlag = reader.ReadByte();

            return msWay;
        }
    }
}