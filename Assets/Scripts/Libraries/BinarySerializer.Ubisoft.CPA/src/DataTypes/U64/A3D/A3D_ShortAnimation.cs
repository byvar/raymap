namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class A3D_ShortAnimation : BinarySerializable {
        public ushort FramesCount { get; set; }
        public ushort FrameRate { get; set; }
        public ushort MaxElementsCount { get; set; }
        public ushort EventsCount { get; set; }
        public ushort MorphDataCount { get; set; }
        public ushort Align { get; set; }
        public CPA_Vector3D Translation { get; set; }
        public U64_ShortQuaternion Rotation { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
            FrameRate = s.Serialize<ushort>(FrameRate, name: nameof(FrameRate));
            MaxElementsCount = s.Serialize<ushort>(MaxElementsCount, name: nameof(MaxElementsCount));
            EventsCount = s.Serialize<ushort>(EventsCount, name: nameof(EventsCount));
            MorphDataCount = s.Serialize<ushort>(MorphDataCount, name: nameof(MorphDataCount));
            Align = s.Serialize<ushort>(Align, name: nameof(Align));
            Translation = s.SerializeObject<CPA_Vector3D>(Translation, name: nameof(Translation));
            Rotation = s.SerializeObject<U64_ShortQuaternion>(Rotation, name: nameof(Rotation));
        }
    }
}