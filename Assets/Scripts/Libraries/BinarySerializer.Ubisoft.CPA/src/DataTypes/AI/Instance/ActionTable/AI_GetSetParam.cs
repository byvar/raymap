namespace BinarySerializer.Ubisoft.CPA {
	public class AI_GetSetParam : BinarySerializable {
		// This struct is a big union of various different value and pointer types.
		// The biggest one is a vector, so the size of this struct is 12 bytes
		// TODO: Maybe parse this properly someday
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Data = s.SerializeArray<byte>(Data, 12, name: nameof(Data));
		}
	}
}
