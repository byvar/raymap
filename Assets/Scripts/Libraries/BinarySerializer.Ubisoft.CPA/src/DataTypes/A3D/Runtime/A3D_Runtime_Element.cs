namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Runtime_Element : BinarySerializable { // NTTO
		public ushort IndexInTable { get; set; }
		public bool IsActive { get; set; }
		public byte ChannelIndex { get; set; }
		public A3D_NTTO_ElementType Type { get; set; }
		public byte Transparency { get; set; }
		public byte AnimLevel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<ushort>(b => {
				IndexInTable = b.SerializeBits<ushort>(IndexInTable, 15, name: nameof(IndexInTable));
				IsActive = b.SerializeBits<bool>(IsActive, 1, name: nameof(IsActive));
			});
			ChannelIndex = s.Serialize<byte>(ChannelIndex, name: nameof(ChannelIndex));
			Type = s.Serialize<A3D_NTTO_ElementType>(Type, name: nameof(Type));
			Transparency = s.Serialize<byte>(Transparency, name: nameof(Transparency));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				AnimLevel = s.Serialize<byte>(AnimLevel, name: nameof(AnimLevel));
			}
			s.Align(2, Offset);
		}
	}
}