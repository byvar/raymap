namespace BinarySerializer.N64 {
	public class RSP_Command_GBI1_Tri2 : RSP_Command_GBI1_Tri1 {
		public byte V3 { get; set; }
		public byte V4 { get; set; }
		public byte V5 { get; set; }

		public override void SerializeBits(BitSerializerObject b) {
			base.SerializeBits(b);
			b.SerializePadding(1, logIfNotNull: true);
			V3 = b.SerializeBits<byte>(V3, 7, name: nameof(V3));
			b.SerializePadding(1, logIfNotNull: true);
			V4 = b.SerializeBits<byte>(V4, 7, name: nameof(V4));
			b.SerializePadding(1, logIfNotNull: true);
			V5 = b.SerializeBits<byte>(V5, 7, name: nameof(V5));
		}
	}
}
