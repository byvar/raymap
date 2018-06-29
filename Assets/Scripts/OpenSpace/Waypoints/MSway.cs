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

        public static MSWay Read(EndianBinaryReader reader, Pointer offset) {
            MSWay msWay = new MSWay(offset);

            msWay.off_graph = Pointer.Read(reader);
            if (msWay.off_graph != null) {
                Pointer original = Pointer.Goto(ref reader, msWay.off_graph);
                msWay.graph = Graph.Read(reader, msWay.off_graph);
                Pointer.Goto(ref reader, original);
            }

            msWay.currentIndex = reader.ReadUInt32();
            msWay.someFlag = reader.ReadByte();

            return msWay;
        }
    }
}