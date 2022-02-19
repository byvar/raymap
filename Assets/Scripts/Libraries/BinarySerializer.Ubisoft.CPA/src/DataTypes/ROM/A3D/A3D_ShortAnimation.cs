namespace BinarySerializer.Ubisoft.CPA.ROM {
	public class A3D_ShortAnimation : BinarySerializable {
		public ushort FramesCount { get; set; }
		public ushort FrameRate { get; set; }
		public ushort MaxElementsCount { get; set; }
		public ushort EventsCount { get; set; }
		public ushort MorphDataCount { get; set; }
		public ushort Align { get; set; }
		public CPA_Vector Translation { get; set; }
		public CPA_ROM_ShortQuaternion Rotation { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			FrameRate = s.Serialize<ushort>(FrameRate, name: nameof(FrameRate));
			MaxElementsCount = s.Serialize<ushort>(MaxElementsCount, name: nameof(MaxElementsCount));
			EventsCount = s.Serialize<ushort>(EventsCount, name: nameof(EventsCount));
			MorphDataCount = s.Serialize<ushort>(MorphDataCount, name: nameof(MorphDataCount));
			Align = s.Serialize<ushort>(Align, name: nameof(Align));
			Translation = s.SerializeObject<CPA_Vector>(Translation, name: nameof(Translation));
			Rotation = s.SerializeObject<CPA_ROM_ShortQuaternion>(Rotation, name: nameof(Rotation));
		}
	}
}