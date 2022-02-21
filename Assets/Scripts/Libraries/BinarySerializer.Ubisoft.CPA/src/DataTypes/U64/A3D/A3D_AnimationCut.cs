namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class A3D_AnimationCut : BinarySerializable {
		public ushort StartFrame { get; set; }
		public ushort EndFrame { get; set; }
		public ushort PreviousAnimation { get; set; }
		public ushort NextAnimation { get; set; }
		public ushort MaxElementsCount { get; set; }
		public ushort Align { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			StartFrame = s.Serialize<ushort>(StartFrame, name: nameof(StartFrame));
			EndFrame = s.Serialize<ushort>(EndFrame, name: nameof(EndFrame));
			PreviousAnimation = s.Serialize<ushort>(PreviousAnimation, name: nameof(PreviousAnimation));
			NextAnimation = s.Serialize<ushort>(NextAnimation, name: nameof(NextAnimation));
			MaxElementsCount = s.Serialize<ushort>(MaxElementsCount, name: nameof(MaxElementsCount));
			Align = s.Serialize<ushort>(Align, name: nameof(Align));
		}
	}
}