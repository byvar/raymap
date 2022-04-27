namespace BinarySerializer.Nintendo.N64 {
	public abstract class RSP_CommandData : BinarySerializable {
		public RSP_CommandType Command { get; set; }

		public abstract void SerializeBits(BitSerializerObject b);

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<long>(b => {
				SerializeBits(b);

				b.Position = 7 * 8; // Command is the most significant byte
				Command = b.SerializeBits<RSP_CommandType>(Command, 8, name: nameof(Command));
			});
		}
	}
}
