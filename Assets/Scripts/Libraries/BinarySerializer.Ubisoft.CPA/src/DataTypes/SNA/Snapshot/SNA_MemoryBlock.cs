namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_MemoryBlock : BinarySerializable {
		public byte Module { get; set; }
		public byte Bloc { get; set; }
		public SNA_MemoryBlockType Type { get; set; } // Unused
		public uint BeginBlock { get; set; } // Absolute start position of the block
		public uint EndBlock { get; set; } // Absolute end position of the block
		public uint FirstFree { get; set; } // First free memory entry inside block
		public uint MaxMem { get; set; } // Last byte position that has occupied memory
		public bool TT_UnknownXORRelated { get; set; }

		public uint BlockSize { get; set; } // Memory data size. 0 if this is an "info", otherwise = MaxMem-BeginBlock+1+8 (8 for the BlockInfo)

		public byte[] Block { get; set; } // Memory data

		// For testing purposes: to read allocations separately
		public static bool TestAllocations = false;
		public MMG_HeaderBlockWithoutFree HeaderWithoutFree { get; set; }
		public MMG_Allocation[] Allocations { get; set; }

		public const uint InvalidBeginBlock = uint.MaxValue;

		public override void SerializeImpl(SerializerObject s) {
			Module = s.Serialize<byte>(Module, name: nameof(Module));
			Bloc = s.Serialize<byte>(Bloc, name: nameof(Bloc));

			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				Type = s.Serialize<SNA_MemoryBlockType>(Type, name: nameof(Type));

			BeginBlock = s.Serialize<uint>(BeginBlock, name: nameof(BeginBlock));
			if (BeginBlock != InvalidBeginBlock) {
				EndBlock = s.Serialize<uint>(EndBlock, name: nameof(EndBlock));
				FirstFree = s.Serialize<uint>(FirstFree, name: nameof(FirstFree));
				MaxMem = s.Serialize<uint>(MaxMem, name: nameof(MaxMem));
				BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));

				if(s.GetCPASettings().EngineVersion == EngineVersion.TonicTrouble)
					TT_UnknownXORRelated = s.Serialize<bool>(TT_UnknownXORRelated, name: nameof(TT_UnknownXORRelated));

				s.DoEncoded(SNA_LZOEncoder.GetIfRequired(s.GetCPASettings(), BlockSize), () => {
					var blockStart = s.CurrentAbsoluteOffset;
					if (!TestAllocations) {
						Block = s.SerializeArray<byte>(Block, BlockSize, name: nameof(Block));
					} else {
						if (BlockSize > 0) {
							if (FirstFree == 0xFFFFFFFF) {
								HeaderWithoutFree = s.SerializeObject<MMG_HeaderBlockWithoutFree>(HeaderWithoutFree, name: nameof(HeaderWithoutFree));
								Block = s.SerializeArray<byte>(Block, BlockSize - 4, name: nameof(Block));
							} else {
								Allocations = s.SerializeObjectArrayUntil<MMG_Allocation>(Allocations, a => s.CurrentAbsoluteOffset >= blockStart + BlockSize - 8, name: nameof(Allocations));
								Block = s.SerializeArray<byte>(Block, 8, name: nameof(Block));
							}
						}
					}
				});
			}
		}
	}
}
