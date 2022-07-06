namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_TemporaryMemoryBlock : BinarySerializable {
		public byte[] Data { get; set; } // Memory data

		public override void SerializeImpl(SerializerObject s) {
			Data = s.SerializeArray<byte>(Data, s.CurrentLength - s.CurrentFileOffset, name: nameof(Data));
		}
	}
}
