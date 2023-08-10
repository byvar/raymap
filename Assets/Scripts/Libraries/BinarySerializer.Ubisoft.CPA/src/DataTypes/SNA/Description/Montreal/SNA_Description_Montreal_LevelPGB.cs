namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Montreal_LevelPGB : BinarySerializable {
		public uint VignetteNameLength { get; set; }
		public string VignetteName { get; set; }
		public uint UnknownLength { get; set; }
		public string UnknownName { get; set; }
		public int YMin { get; set; }
		public int YMax { get; set; }
		public int XMin { get; set; }
		public int XMax { get; set; }
		public GLI_FloatColor_RGBA BarOutlineColor { get; set; }
		public GLI_FloatColor_RGBA BarInsideColor { get; set; }
		public SNA_Description_Gradient BarColor { get; set; }
		public SNA_Description_Rectangle BarRectangle { get; set; }
		public uint MaxValueBar { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VignetteNameLength = s.Serialize<uint>(VignetteNameLength, name: nameof(VignetteNameLength));
			s.DoProcessed<SNA_MontrealXorProcessor>(new SNA_MontrealXorProcessor(3, -7), () => {
				VignetteName = s.SerializeString(VignetteName, length: VignetteNameLength, name: nameof(VignetteName));
			});
			UnknownLength = s.Serialize<uint>(UnknownLength, name: nameof(UnknownLength));
			s.DoProcessed<SNA_MontrealXorProcessor>(new SNA_MontrealXorProcessor(6, -11), () => {
				UnknownName = s.SerializeString(UnknownName, length: UnknownLength, name: nameof(UnknownName));
			});
			YMin = s.Serialize<int>(YMin, name: nameof(YMin));
			YMax = s.Serialize<int>(YMax, name: nameof(YMax));
			XMin = s.Serialize<int>(XMin, name: nameof(XMin));
			XMax = s.Serialize<int>(XMax, name: nameof(XMax));
			BarOutlineColor = s.SerializeObject<GLI_FloatColor_RGBA>(BarOutlineColor, name: nameof(BarOutlineColor));
			BarInsideColor = s.SerializeObject<GLI_FloatColor_RGBA>(BarInsideColor, name: nameof(BarInsideColor));
			BarColor = s.SerializeObject<SNA_Description_Gradient>(BarColor, name: nameof(BarColor));
			BarRectangle = s.SerializeObject<SNA_Description_Rectangle>(BarRectangle, name: nameof(BarRectangle));
			MaxValueBar = s.Serialize<uint>(MaxValueBar, name: nameof(MaxValueBar));

		}
	}
}
