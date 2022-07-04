namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_MemoryBlock : BinarySerializable {
		public virtual byte Module { get; set; }
		public virtual byte Block { get; set; }
		public SNA_MemoryBlockType Type { get; set; } // Unused
		public uint BeginBlock { get; set; } // Absolute start position of the block
		public uint EndBlock { get; set; } // Absolute end position of the block
		public uint FirstFree { get; set; } // First free memory entry inside block
		public uint MaxMem { get; set; } // Last byte position that has occupied memory
		public bool TT_UnknownXORRelated { get; set; }

		public uint BlockSize { get; set; } // Memory data size. 0 if this is an "info", otherwise = MaxMem-BeginBlock+1+8 (8 for the BlockInfo)

		public byte[] Data { get; set; } // Memory data

		// For testing purposes: to read allocations separately
		public static bool TestAllocations = false;
		public MMG_HeaderBlockWithoutFree HeaderWithoutFree { get; set; }
		public MMG_Allocation[] Allocations { get; set; }

		public const uint InvalidBeginBlock = uint.MaxValue;

		public virtual SNA_Module? ModuleTranslation {
			get => Context.GetCPASettings().SNATypes?.GetModule(Module);
			set {
				if(!value.HasValue) return;
				var val = value.Value;
				var newModule = Context.GetCPASettings().SNATypes?.GetModuleInt(val);
				if(newModule.HasValue) Module = (byte)newModule.Value;
			}
		}

		public virtual string BlockName => $"{ModuleTranslation?.ToString() ?? Module.ToString()}_{Block}";

		public override void SerializeImpl(SerializerObject s) {
			Module = s.Serialize<byte>(Module, name: nameof(Module));
			if (ModuleTranslation != null) s.Log($"Module: {ModuleTranslation}");

			Block = s.Serialize<byte>(Block, name: nameof(Block));

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
						Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
					} else {
						if (BlockSize > 0) {
							if (FirstFree == 0xFFFFFFFF) {
								HeaderWithoutFree = s.SerializeObject<MMG_HeaderBlockWithoutFree>(HeaderWithoutFree, name: nameof(HeaderWithoutFree));
								Data = s.SerializeArray<byte>(Data, BlockSize - 4, name: nameof(Data));
							} else {
								Allocations = s.SerializeObjectArrayUntil<MMG_Allocation>(Allocations, a => s.CurrentAbsoluteOffset >= blockStart + BlockSize - 8, name: nameof(Allocations));
								Data = s.SerializeArray<byte>(Data, 8, name: nameof(Data));
							}
						}
					}
				});
			}
		}
	}
}
