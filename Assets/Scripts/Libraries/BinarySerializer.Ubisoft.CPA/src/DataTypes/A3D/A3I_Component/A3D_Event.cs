namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Event : BinarySerializable {
		public Pointer<A3D_EventInTable> EventInTBL { get; set; } // in object table
		public ushort EventIndexInTBL { get; set; }
		public ushort FrameIndex { get; set; }
		public ushort ChannelIndex { get; set; }
		public ushort IsLocalized { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Branch != EngineBranch.U64
				&& s.GetCPASettings().Platform != Platform.DC
				&& s.GetCPASettings().Platform != Platform.iOS
				&& !s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet)) {
				EventInTBL = s.SerializePointer<A3D_EventInTable>(EventInTBL, name: nameof(EventInTBL))?.ResolveObject(s);
			}

			EventIndexInTBL = s.Serialize<ushort>(EventIndexInTBL, name: nameof(EventIndexInTBL));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));
				ChannelIndex = s.Serialize<ushort>(ChannelIndex, name: nameof(ChannelIndex));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					IsLocalized = s.Serialize<ushort>(IsLocalized, name: nameof(IsLocalized));
				} else if(s.GetCPASettings().Platform != Platform.DC && s.GetCPASettings().Platform != Platform.iOS) {
					s.Align(4, Offset);
				}
			} else {
				FrameIndex = s.Serialize<byte>((byte)FrameIndex, name: nameof(FrameIndex));
				ChannelIndex = s.Serialize<byte>((byte)ChannelIndex, name: nameof(ChannelIndex));
			}
		}
	}
}