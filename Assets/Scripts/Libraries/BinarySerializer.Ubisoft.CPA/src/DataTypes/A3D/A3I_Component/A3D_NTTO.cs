namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_NTTO : BinarySerializable {
		public ushort TypeOfObject { get; set; }
		public ushort IndexInTable { get; set; }
		public byte Transparency { get; set; }

		// Parsed
		public A3D_NTTO_ElementType Type { get; set; }
		public A3D_NTTO_ElementTypeFlags TypeFlags { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				TypeOfObject = s.Serialize<ushort>(TypeOfObject, name: nameof(TypeOfObject));
				s.DoAt(Offset, () => {
					s.DoBits<ushort>(b => {
						Type = b.SerializeBits<A3D_NTTO_ElementType>(Type, 8, name: nameof(Type));
						TypeFlags = b.SerializeBits<A3D_NTTO_ElementTypeFlags>(TypeFlags, 8, name: nameof(TypeFlags));
					});
				});
				if (s.GetCPASettings().Branch == EngineBranch.U64) {
					IndexInTable = s.Serialize<byte>((byte)IndexInTable, name: nameof(IndexInTable));
				} else {
					IndexInTable = s.Serialize<ushort>(IndexInTable, name: nameof(IndexInTable));
				}
			} else {
				IndexInTable = s.Serialize<ushort>(IndexInTable, name: nameof(IndexInTable));
				TypeOfObject = s.Serialize<ushort>(TypeOfObject, name: nameof(TypeOfObject));
				s.DoAt(Offset + 2, () => {
					s.DoBits<ushort>(b => {
						Type = b.SerializeBits<A3D_NTTO_ElementType>(Type, 8, name: nameof(Type));
						TypeFlags = b.SerializeBits<A3D_NTTO_ElementTypeFlags>(TypeFlags, 8, name: nameof(TypeFlags));
					});
				});
			}
			Transparency = s.Serialize<byte>(Transparency, name: nameof(Transparency));
			s.Align(2, Offset);
		}
	}
}