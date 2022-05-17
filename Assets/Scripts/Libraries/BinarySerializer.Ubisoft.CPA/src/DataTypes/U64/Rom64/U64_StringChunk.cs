namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_StringChunk : U64_Struct {
		public ushort Pre_Length { get; set; }
		public string StringValue { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// A StringChunk is actually only 2 characters. They're serialized as ArrayReference<StringChunk>.
			// We just serialize the full string instead.
			StringValue = s.SerializeString(StringValue, length: Pre_Length + (Pre_Length % 2), name: nameof(StringValue));
		}
	}
}
