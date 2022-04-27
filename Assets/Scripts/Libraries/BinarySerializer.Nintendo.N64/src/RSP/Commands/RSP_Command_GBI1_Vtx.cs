namespace BinarySerializer.Nintendo.N64 {
	public class RSP_Command_GBI1_Vtx : RSP_CommandData {
		public uint Address { get; set; } // Address of vertex in segment to load. Since each vertex is 0x10 bytes, this is a multiple of 0x10
		public byte Segment { get; set; } // RSP memory works with segments: http://n64.icequake.net/doc/n64intro/kantan/step1/1-4.html
		public ushort Length { get; set; } // Length of data to write in vertex buffer
		public byte N { get; set; } // Number of vertices to load into vertex buffer. max 32
		public byte V0 { get; set; } // Position in vertex buffer to write to

		public override void SerializeBits(BitSerializerObject b) {
			Address = b.SerializeBits<uint>(Address, 3 * 8, name: nameof(Address));
			Segment = b.SerializeBits<byte>(Segment, 8, name: nameof(Segment));
			Length = b.SerializeBits<ushort>(Length, 10, name: nameof(Length));
			N = b.SerializeBits<byte>(N, 6, name: nameof(N));
			b.SerializePadding(1, logIfNotNull: true);
			V0 = b.SerializeBits<byte>(V0, 7, name: nameof(V0));
		}
	}
}
