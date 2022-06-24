namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_RelocationTable : BinarySerializable {
		public byte BlocksCount { get; set; }
		public uint Checksum { get; set; }
		public SNA_RelocationTableBlock[] Blocks { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BlocksCount = s.Serialize<byte>(BlocksCount, name: nameof(BlocksCount));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2))
				Checksum = s.Serialize<uint>(Checksum, name: nameof(Checksum));

			Blocks = s.SerializeObjectArray<SNA_RelocationTableBlock>(Blocks, BlocksCount, name: nameof(Blocks));
		}

		public static string GetExtension(SNA_RelocationType type) {
			return type switch {
				SNA_RelocationType.SNA => "rtb",
				SNA_RelocationType.GlobalPointers => "rtp",
				SNA_RelocationType.Sound => "rts",
				SNA_RelocationType.Textures => "rtt",
				SNA_RelocationType.LipsSync => "rtl",
				SNA_RelocationType.Dialog => "rtd",
				SNA_RelocationType.RTG => "rtg",
				SNA_RelocationType.Video => "rtv",
				_ => null
			};
		}
	}
}
