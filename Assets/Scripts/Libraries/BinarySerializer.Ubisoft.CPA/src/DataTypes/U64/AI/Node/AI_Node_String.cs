namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Node_String : U64_Struct {
		public ushort Length { get; set; }
		public U64_Reference<U64_StringChunk> Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Length = s.Serialize<ushort>(Length, name: nameof(Length));
			Value = s.SerializeObject<U64_Reference<U64_StringChunk>>(Value, name: nameof(Value));
		}
	}
}
