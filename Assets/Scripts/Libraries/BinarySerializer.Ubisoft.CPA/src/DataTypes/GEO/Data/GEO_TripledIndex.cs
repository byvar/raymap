namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_TripledIndex : BinarySerializable {
		public ushort Index0 { get; set; }
		public ushort Index1 { get; set; }
		public ushort Index2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Index0 = s.Serialize<ushort>(Index0, name: nameof(Index0));
			Index1 = s.Serialize<ushort>(Index1, name: nameof(Index1));
			Index2 = s.Serialize<ushort>(Index2, name: nameof(Index2));
		}
		public override bool UseShortLog => true;
		public override string ToString() => $"{GetType().Name}({Index0}, {Index1}, {Index2})";
	}
}
