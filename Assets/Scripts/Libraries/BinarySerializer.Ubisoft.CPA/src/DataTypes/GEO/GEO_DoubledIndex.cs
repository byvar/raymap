namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_DoubledIndex : BinarySerializable {
		public ushort Index0 { get; set; }
		public ushort Index1 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Index0 = s.Serialize<ushort>(Index0, name: nameof(Index0));
			Index1 = s.Serialize<ushort>(Index1, name: nameof(Index1));
		}
	}
}
