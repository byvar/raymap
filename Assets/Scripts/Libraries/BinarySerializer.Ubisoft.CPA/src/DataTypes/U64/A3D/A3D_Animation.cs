namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class A3D_Animation : BinarySerializable {
        public long Pre_DataSize { get; set; }

        public A3D_General Header { get; set; }
        public A3D_Vector[] Vectors { get; set; }
        public A3D_Quaternion[] Quaternions { get; set; }
        public A3D_Hierarchy[] Hierarchies { get; set; }
        public A3D_NTTO[] NTTOs { get; set; }
        public A3D_Frame[] SavedFrames { get; set; }
        public A3D_OnlyFrame[] OnlyFrames { get; set; }
        public A3D_Channel[] Channels { get; set; }
        public A3D_KeyFrame[] KeyFrames { get; set; }
        public A3D_Event[] Events { get; set; }
        public A3D_MorphData[] MorphData { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Header = s.SerializeObject<A3D_General>(Header, name: nameof(Header));
            Vectors = s.SerializeObjectArray<A3D_Vector>(Vectors, Header.VectorsCount, name: nameof(Vectors));
            Quaternions = s.SerializeObjectArray<A3D_Quaternion>(Quaternions, Header.QuaternionsCount, name: nameof(Quaternions));
            Hierarchies = s.SerializeObjectArray<A3D_Hierarchy>(Hierarchies, Header.HierarchiesCount, name: nameof(Hierarchies));
            NTTOs = s.SerializeObjectArray<A3D_NTTO>(NTTOs, Header.NTTOCount, name: nameof(NTTOs));
            OnlyFrames = s.SerializeObjectArray<A3D_OnlyFrame>(OnlyFrames, Header.EndFrame - Header.StartFrame, name: nameof(OnlyFrames));
            Channels = s.SerializeObjectArray<A3D_Channel>(Channels, Header.ChannelsCount, name: nameof(Channels));
            SavedFrames = s.SerializeObjectArray<A3D_Frame>(SavedFrames, Header.ChannelsCount * Header.SavedFramesCount, name: nameof(SavedFrames));
            KeyFrames = s.SerializeObjectArray<A3D_KeyFrame>(KeyFrames, Header.KeyFramesCount, name: nameof(KeyFrames));
            s.Align(4, Offset);
            Events = s.SerializeObjectArray<A3D_Event>(Events, Header.EventsCount, name: nameof(Events));
            MorphData = s.SerializeObjectArray<A3D_MorphData>(MorphData, Header.MorphDataCount, name: nameof(MorphData));

            if (s.CurrentAbsoluteOffset != Offset.AbsoluteOffset + Pre_DataSize) {
                long readSize = s.CurrentAbsoluteOffset - Offset.AbsoluteOffset;
                throw new BinarySerializableException(this, $"Animation was not fully read. Filesize: {Pre_DataSize} vs. read: {readSize}");
            }
        }
    }
}