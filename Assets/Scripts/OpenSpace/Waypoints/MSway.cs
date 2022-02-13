namespace OpenSpace.Waypoints {
    public class MSWay {

        public LegacyPointer offset;
        public Graph graph;
        public uint currentIndex;
        public byte someFlag;

        public LegacyPointer off_graph;

        public MSWay(LegacyPointer offset) // MicroStructure for Waypoint stuff, pointer to this is stored in Engine Object
        {
            this.offset = offset;
        }

        public static MSWay Read(Reader reader, LegacyPointer offset) {
            MSWay msWay = new MSWay(offset);

            msWay.off_graph = LegacyPointer.Read(reader);
            msWay.graph = Graph.FromOffsetOrRead(msWay.off_graph, reader);
            msWay.currentIndex = reader.ReadUInt32();
            msWay.someFlag = reader.ReadByte();

            return msWay;
        }
    }
}