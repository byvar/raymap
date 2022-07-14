namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_FrameKF : BinarySerializable {
		public uint KeyFrameIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (IsAligned(s.Context)) {
				KeyFrameIndex = s.Serialize<uint>(KeyFrameIndex, name: nameof(KeyFrameIndex));
			} else {
				KeyFrameIndex = s.Serialize<ushort>((ushort)KeyFrameIndex, name: nameof(KeyFrameIndex));
			}
		}

		public static bool IsAligned(Context c) =>
			c.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2)
			&& c.GetCPASettings().EngineVersion != EngineVersion.Rayman2Revolution;

		public override bool UseShortLog => true;
		public override string ToString() => $"FrameKF(KeyFrame: {KeyFrameIndex})";
	}
}