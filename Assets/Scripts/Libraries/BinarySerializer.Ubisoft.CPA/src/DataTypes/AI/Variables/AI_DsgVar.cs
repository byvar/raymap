namespace BinarySerializer.Ubisoft.CPA {
	public class AI_DsgVar : BinarySerializable {
		public Pointer DefaultMemoryInit { get; set; }
		public Pointer<AI_DsgVarInfo[]> Variables { get; set; }
		public uint BufferSize { get; set; } // Size of the buffer pointed to by DsgMemDefaultInit, and the buffers in the DsgMem structs that use this DsgVar
		public uint VariablesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DefaultMemoryInit = s.SerializePointer(DefaultMemoryInit, name: nameof(DefaultMemoryInit));
			Variables = s.SerializePointer<AI_DsgVarInfo[]>(Variables, name: nameof(Variables));
			if(s.GetCPASettings().EngineVersion == EngineVersion.Rayman2Revolution) {
				BufferSize = s.Serialize<ushort>((ushort)BufferSize, name: nameof(BufferSize));
				VariablesCount = s.Serialize<ushort>((ushort)VariablesCount, name: nameof(VariablesCount));
			} else {
				BufferSize = s.Serialize<uint>(BufferSize, name: nameof(BufferSize));
				VariablesCount = s.Serialize<byte>((byte)VariablesCount, name: nameof(VariablesCount));
			}

			Variables?.ResolveObjectArray(s, VariablesCount);
		}
	}
}
