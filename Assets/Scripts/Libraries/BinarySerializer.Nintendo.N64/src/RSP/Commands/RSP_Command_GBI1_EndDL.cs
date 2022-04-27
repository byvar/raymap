namespace BinarySerializer.Nintendo.N64 {
	public class RSP_Command_GBI1_EndDL: RSP_CommandData {

		public override void SerializeBits(BitSerializerObject b) {
			b.SerializePadding(7 * 8, logIfNotNull: true);
		}
	}
}
