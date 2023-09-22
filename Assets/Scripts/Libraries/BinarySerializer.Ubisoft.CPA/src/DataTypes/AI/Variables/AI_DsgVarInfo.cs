namespace BinarySerializer.Ubisoft.CPA {
	public class AI_DsgVarInfo : BinarySerializable {
		public uint OffsetInMemory { get; set; }
		public uint DsgVarType { get; set; }
		public SAI_Flags SaveType { get; set; }
		public AI_ObjectTreeInit InitType { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				throw new System.NotImplementedException();
			} else {
				OffsetInMemory = s.Serialize<uint>(OffsetInMemory, name: nameof(OffsetInMemory));
				DsgVarType = s.Serialize<uint>(DsgVarType, name: nameof(DsgVarType));
				SaveType = s.Serialize<SAI_Flags>(SaveType, name: nameof(SaveType));
				s.Align(4, Offset);
				InitType = s.Serialize<AI_ObjectTreeInit>(InitType, name: nameof(InitType));
			}

			s?.Log($"Linked Type: {s.GetCPASettings().AITypes?.GetDsgVarType(DsgVarType)}");
		}
	}
}
