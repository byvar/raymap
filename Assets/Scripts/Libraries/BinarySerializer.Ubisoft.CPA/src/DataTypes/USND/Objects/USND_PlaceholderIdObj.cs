namespace BinarySerializer.Ubisoft.CPA {
	public class USND_PlaceholderIdObj : USND_IdObj {
		public long Pre_Length { get; set; }
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			Data = s.SerializeArray<byte>(Data, Pre_Length, name: nameof(Data));
		}
	}
}