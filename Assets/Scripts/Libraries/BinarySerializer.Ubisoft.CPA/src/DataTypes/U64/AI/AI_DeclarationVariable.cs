namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_DeclarationVariable : U64_Struct {
		public ushort Type { get; set; }
		public ushort Value { get; set; }
		public ushort OffsetInBuffer { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<ushort>(Type, name: nameof(Type));
			Value = s.Serialize<ushort>(Value, name: nameof(Value));
			OffsetInBuffer = s.Serialize<ushort>(OffsetInBuffer, name: nameof(OffsetInBuffer));
		}
	}
}
