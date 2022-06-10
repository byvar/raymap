namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_InitializationVariable : U64_Struct {
		public ushort VariableId { get; set; } // Index in Declaration
		public AI_Variable_Value Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VariableId = s.Serialize<ushort>(VariableId, name: nameof(VariableId));
			Value = s.SerializeObject<AI_Variable_Value>(Value, name: nameof(Value));
		}
	}
}
