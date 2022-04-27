namespace BinarySerializer.Nintendo.N64 {
	public class RSP_Command_GBI1_ModifyVtx : RSP_CommandData {
		public Type ModifyCommand { get; set; }
		public ushort Vertex { get; set; }

		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public byte A { get; set; }

		public ushort U { get; set; } // TODO: Check if these can be negative
		public ushort V { get; set; }

		public ushort X { get; set; }
		public ushort Y { get; set; }
		public ushort Z { get; set; }


		public override void SerializeBits(BitSerializerObject b) {
			b.Position = 4*8;
			b.SerializePadding(1, logIfNotNull: true);
			Vertex = b.SerializeBits<ushort>(Vertex, 15, name: nameof(Vertex));
			ModifyCommand = b.SerializeBits<Type>(ModifyCommand, 8, name: nameof(ModifyCommand));
			b.Position = 0;

			switch (ModifyCommand) {
				case Type.RSP_MV_WORD_OFFSET_POINT_RGBA:
					R = b.SerializeBits<byte>(R, 8, name: nameof(R));
					G = b.SerializeBits<byte>(G, 8, name: nameof(G));
					B = b.SerializeBits<byte>(B, 8, name: nameof(B));
					A = b.SerializeBits<byte>(A, 8, name: nameof(A));
					break;
				case Type.RSP_MV_WORD_OFFSET_POINT_ST:
					V = b.SerializeBits<ushort>(V, 16, name: nameof(V));
					U = b.SerializeBits<ushort>(U, 16, name: nameof(U));
					break;
				case Type.RSP_MV_WORD_OFFSET_POINT_XYSCREEN:
					b.SerializePadding(2, logIfNotNull: true);
					X = b.SerializeBits<ushort>(X, 14, name: nameof(X));
					b.SerializePadding(2, logIfNotNull: true);
					Y = b.SerializeBits<ushort>(Y, 14, name: nameof(Y));
					break;
				case Type.RSP_MV_WORD_OFFSET_POINT_ZSCREEN:
					Z = b.SerializeBits<ushort>(Z, 16, name: nameof(Z));
					b.SerializePadding(16, logIfNotNull: true);
					break;
				default:
					throw new BinarySerializableException(this, $"Unparsed command {ModifyCommand}");
			}

		}
		public enum Type : byte {
			RSP_MV_WORD_OFFSET_POINT_RGBA = 0x10,
			RSP_MV_WORD_OFFSET_POINT_ST = 0x14,
			RSP_MV_WORD_OFFSET_POINT_XYSCREEN = 0x18,
			RSP_MV_WORD_OFFSET_POINT_ZSCREEN = 0x1C
		}
	}
}
