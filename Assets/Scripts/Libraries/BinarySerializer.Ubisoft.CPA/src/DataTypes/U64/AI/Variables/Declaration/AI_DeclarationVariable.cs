namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_DeclarationVariable : U64_Struct {
		public AI_Variable_Value Value { get; set; }
		public ushort OffsetInBuffer { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeObject<AI_Variable_Value>(Value, name: nameof(Value));
			OffsetInBuffer = s.Serialize<ushort>(OffsetInBuffer, name: nameof(OffsetInBuffer));
		}
	}
}
