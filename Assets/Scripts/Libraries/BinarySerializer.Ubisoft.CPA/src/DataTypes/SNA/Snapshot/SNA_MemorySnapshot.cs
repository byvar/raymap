namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_MemorySnapshot : BinarySerializable {
		public ushort TT_PathLength { get; set; }
		public string TT_Path { get; set; }
		public const byte TT_NameXORKey = 0xA5;

		public SNA_MemoryBlock[] Blocks { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersion == EngineVersion.TonicTrouble) {
				TT_PathLength = s.Serialize<ushort>(TT_PathLength, name: nameof(TT_PathLength));
				s.DoXOR(TT_NameXORKey, () => {
					TT_Path = s.SerializeString(TT_Path, length: TT_PathLength-1, name: nameof(TT_Path));
				});
			}
			Blocks = s.SerializeObjectArrayUntil<SNA_MemoryBlock>(Blocks,
				b => s.CurrentAbsoluteOffset >= s.CurrentLength,
				name: nameof(Blocks));
		}
	}
}
