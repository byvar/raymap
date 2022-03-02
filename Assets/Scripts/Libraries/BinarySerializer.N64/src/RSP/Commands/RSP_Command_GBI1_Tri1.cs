namespace BinarySerializer.N64 {
	public class RSP_Command_GBI1_Tri1 : RSP_CommandData {
		public byte V0 { get; set; }
		public byte V1 { get; set; }
		public byte V2 { get; set; }
		public byte Flag { get; set; } // Contains the index of the vertex that contains the normal/color for the face (for flat shading)

		public override void SerializeBits(BitSerializerObject b) {
			b.SerializePadding(1, logIfNotNull: true);
			V0 = b.SerializeBits<byte>(V0, 7, name: nameof(V0));
			b.SerializePadding(1, logIfNotNull: true);
			V1 = b.SerializeBits<byte>(V1, 7, name: nameof(V1));
			b.SerializePadding(1, logIfNotNull: true);
			V2 = b.SerializeBits<byte>(V2, 7, name: nameof(V2));

			Flag = b.SerializeBits<byte>(Flag, 8, name: nameof(Flag));
		}
	}
}
