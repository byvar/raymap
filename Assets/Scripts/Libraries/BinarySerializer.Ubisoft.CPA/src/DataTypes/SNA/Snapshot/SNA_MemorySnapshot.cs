namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_MemorySnapshot : BinarySerializable {
		public ushort TT_PathLength { get; set; }
		public string TT_Path { get; set; }
		public const byte TT_NameXORKey = 0xA5;

		/// <summary>
		/// Order in Rayman 2:
		/// 1. Infos of blocks that are not saved: TMP_0, SND_0
		/// 2. Infos of blocks saved in Fix: in source: GAM_0, (GEO_0,) GEO_1, IPT_0, AI_0, FON_0
		/// 3 (Fix). Data of blocks saved in Fix
		/// 3 (Level). Infos of blocks saved in Level: in source: GAM_1, GEO_2, AI_1
		/// 4 (Level). Data of blocks saved in Level
		/// 
		/// Order in Tonic Trouble (Retail):
		/// All blocks in the same order in both Fix and Level.
		/// Level blocks occur in Fix with size 0, Fix blocks occur in Level with size 0.
		/// Seems the same in the Montreal version.
		/// </summary>
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
			if (Blocks != null) {
				s.SystemLogger?.LogWarning("Blocks:");
				foreach (var b in Blocks) {
					s.SystemLogger?.LogWarning($"{b.BlockName} - {b.BlockSize}");
				}
			}
		}
	}
}
