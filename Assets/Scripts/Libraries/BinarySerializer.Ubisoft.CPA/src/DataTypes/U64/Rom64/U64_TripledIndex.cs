namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_TripledIndex : U64_Struct, ISerializerShortLog {
		public ushort Index0 { get; set; }
		public ushort Index1 { get; set; }
		public ushort Index2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Index0 = s.Serialize<ushort>(Index0, name: nameof(Index0));
			Index1 = s.Serialize<ushort>(Index1, name: nameof(Index1));
			Index2 = s.Serialize<ushort>(Index2, name: nameof(Index2));
		}
		public string ShortLog => $"TripledIndex({Index0:X4}, {Index1:X4}, {Index2:X4})";
	}
}
