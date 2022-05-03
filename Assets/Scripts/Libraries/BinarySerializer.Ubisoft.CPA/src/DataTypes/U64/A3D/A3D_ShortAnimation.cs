namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class A3D_ShortAnimation : BinarySerializable {
		public ushort FramesCount { get; set; }
		public ushort FrameRate { get; set; }
		public ushort MaxElementsCount { get; set; }
		public ushort EventsCount { get; set; }
		public ushort MorphDataCount { get; set; }
		public ushort Align { get; set; }
		public CPA.MTH3D_Vector Translation { get; set; }
		public MTH4D_ShortQuaternion Rotation { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			FrameRate = s.Serialize<ushort>(FrameRate, name: nameof(FrameRate));
			MaxElementsCount = s.Serialize<ushort>(MaxElementsCount, name: nameof(MaxElementsCount));
			EventsCount = s.Serialize<ushort>(EventsCount, name: nameof(EventsCount));
			MorphDataCount = s.Serialize<ushort>(MorphDataCount, name: nameof(MorphDataCount));
			Align = s.Serialize<ushort>(Align, name: nameof(Align));
			Translation = s.SerializeObject<CPA.MTH3D_Vector>(Translation, name: nameof(Translation));
			Rotation = s.SerializeObject<MTH4D_ShortQuaternion>(Rotation, name: nameof(Rotation));
		}
	}
}