namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_TypeVariable : U64_Struct {
		public ushort VariableId { get; set; }
		public ushort Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VariableId = s.Serialize<ushort>(VariableId, name: nameof(VariableId));
			Type = s.Serialize<ushort>(Type, name: nameof(Type));
		}
	}
}
