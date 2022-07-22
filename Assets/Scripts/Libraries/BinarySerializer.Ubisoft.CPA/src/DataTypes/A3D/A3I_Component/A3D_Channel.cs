namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Channel : BinarySerializable {
		public ushort KeysCount { get; set; }
		public ushort ChannelID { get; set; }
		public ushort LocalPivotPositionVector { get; set; }

		public ushort FramesStart { get; set; }
		public uint FramesKFStart { get; set; }
		public uint KeyFramesStart { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			KeysCount = s.Serialize<ushort>(KeysCount, name: nameof(KeysCount));
			ChannelID = s.Serialize<ushort>(ChannelID, name: nameof(ChannelID));
			LocalPivotPositionVector = s.Serialize<ushort>(LocalPivotPositionVector, name: nameof(LocalPivotPositionVector));

			if (s.GetCPASettings().Branch == EngineBranch.U64) {
				s.SerializePadding(2, logIfNotNull: true);
			} else {
				FramesStart = s.Serialize<ushort>(FramesStart, name: nameof(FramesStart));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)
					&& s.GetCPASettings().EngineVersion != EngineVersion.Rayman2Revolution) {
					FramesKFStart = s.Serialize<uint>(FramesKFStart, name: nameof(FramesKFStart));
					if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
						KeyFramesStart = s.Serialize<uint>(KeyFramesStart, name: nameof(KeyFramesStart));
					} else {
						KeyFramesStart = s.Serialize<ushort>((ushort)KeyFramesStart, name: nameof(KeyFramesStart));
						s.Align(4, Offset);
					}
				} else {
					FramesKFStart = s.Serialize<ushort>((ushort)FramesKFStart, name: nameof(FramesKFStart));
					KeyFramesStart = s.Serialize<ushort>((ushort)KeyFramesStart, name: nameof(KeyFramesStart));
				}
			}
		}
	}
}