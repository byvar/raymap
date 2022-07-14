namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_StackInfos : BinarySerializable {
		public uint PosAlloc { get; set; }
		public uint StackPos { get; set; }
		public uint MaxPos { get; set; }
		public uint ResetPos { get; set; }
		public uint FamilyPos { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			PosAlloc = s.Serialize<uint>(PosAlloc, name: nameof(PosAlloc));
			StackPos = s.Serialize<uint>(StackPos, name: nameof(StackPos));
			MaxPos = s.Serialize<uint>(MaxPos, name: nameof(MaxPos));
			ResetPos = s.Serialize<uint>(ResetPos, name: nameof(ResetPos));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanM)) {
				FamilyPos = s.Serialize<uint>(FamilyPos, name: nameof(FamilyPos));
			}
		}

		public uint Count(bool append = false) => StackPos - (append ? FamilyPos : 0);
		public uint MaxCount(bool append = false) => MaxPos;
		// TODO: Use MaxPos when serializing Fix. Maybe for level too.
	}
}
