namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Declaration : U64_Struct {
		public ushort BlockSize { get; set; } // Size of Buffer
		public U64_ArrayReference<AI_DeclarationVariable> Variables { get; set; }
		public U64_ArrayReference<LST_ReferenceElement<AI_TypeVariable>> SaveTypes { get; set; }
		public U64_ArrayReference<LST_ReferenceElement<AI_TypeVariable>> InitTypes { get; set; }
		public ushort VariablesCountInBuffer { get; set; } // In buffer
		public ushort VariablesCount { get; set; }
		public ushort SaveTypesCount { get; set; }
		public ushort InitTypesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BlockSize = s.Serialize<ushort>(BlockSize, name: nameof(BlockSize));
			Variables = s.SerializeObject<U64_ArrayReference<AI_DeclarationVariable>>(Variables, name: nameof(Variables));
			SaveTypes = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<AI_TypeVariable>>>(SaveTypes, name: nameof(SaveTypes));
			InitTypes = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<AI_TypeVariable>>>(InitTypes, name: nameof(InitTypes));
			VariablesCountInBuffer = s.Serialize<ushort>(VariablesCountInBuffer, name: nameof(VariablesCountInBuffer));
			VariablesCount = s.Serialize<ushort>(VariablesCount, name: nameof(VariablesCount));
			SaveTypesCount = s.Serialize<ushort>(SaveTypesCount, name: nameof(SaveTypesCount));
			InitTypesCount = s.Serialize<ushort>(InitTypesCount, name: nameof(InitTypesCount));

			Variables?.Resolve(s, VariablesCount);
			SaveTypes?.Resolve(s, SaveTypesCount);
			InitTypes?.Resolve(s, InitTypesCount);
		}
	}
}
