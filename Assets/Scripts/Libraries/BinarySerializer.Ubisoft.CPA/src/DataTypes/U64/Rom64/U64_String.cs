namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_String : BinarySerializable {
		public bool Pre_IsBinary { get; set; } = false;

		public ushort Length { get; set; }
		public U64_Reference<U64_StringChunk> Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Length = s.Serialize<ushort>(Length, name: nameof(Length));
			Value = s.SerializeObject<U64_Reference<U64_StringChunk>>(Value, name: nameof(Value));

			Value?.Resolve(s, onPreSerialize: (_,v) => {
				v.Pre_Length = Length;
				v.Pre_IsBinary = Pre_IsBinary;
			});
		}
	}
}
