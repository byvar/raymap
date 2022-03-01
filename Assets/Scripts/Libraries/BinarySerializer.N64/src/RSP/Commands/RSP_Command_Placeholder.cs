namespace BinarySerializer.N64 {
	public class RSP_Command_Placeholder : RSP_CommandData {

		public override void SerializeBits(BitSerializerObject b) {
			b.SerializerObject.LogWarning("{0}: Unparsed RSP Command: {1}", Offset, Command);
			b.SerializePadding(7 * 8);
		}
	}
}
