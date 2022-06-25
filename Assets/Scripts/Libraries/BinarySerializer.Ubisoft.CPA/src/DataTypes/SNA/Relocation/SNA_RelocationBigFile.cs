using System;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	// PC Protection
	public class SNA_RelocationBigFile : BinarySerializable {
		public static uint MainHeaderSize => 8*4;

		public uint RelocationTablesCount { get; set; } // = MapsCount * 4
		public uint OccurCount { get; set; }
		public uint HeaderStep { get; set; }
		public uint FirstHeaderSize { get; set; }
		public uint FileStep { get; set; }
		public uint SectorSize { get; set; }
		public uint SectorsCount { get; set; }
		public uint ConnectedFilesCount { get; set; }

		public Pointer<OffsetTable>[] Occurs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoEncoded(new PTC_BigFileEncoder(MainHeaderSize), () => {
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2)) {
					// Different order in R2
					HeaderStep = s.Serialize<uint>(HeaderStep, name: nameof(HeaderStep)); // Unclear. Could be FileStep too
					SectorsCount = s.Serialize<uint>(SectorsCount, name: nameof(SectorsCount));
					RelocationTablesCount = s.Serialize<uint>(RelocationTablesCount, name: nameof(RelocationTablesCount));
					FileStep = s.Serialize<uint>(FileStep, name: nameof(FileStep)); // Unclear. Could be HeaderStep too
					OccurCount = s.Serialize<uint>(OccurCount, name: nameof(OccurCount));
					ConnectedFilesCount = s.Serialize<uint>(ConnectedFilesCount, name: nameof(ConnectedFilesCount));
					FirstHeaderSize = s.Serialize<uint>(FirstHeaderSize, name: nameof(FirstHeaderSize));
					SectorSize = s.Serialize<uint>(SectorSize, name: nameof(SectorSize));
				} else {
					RelocationTablesCount = s.Serialize<uint>(RelocationTablesCount, name: nameof(RelocationTablesCount));
					OccurCount = s.Serialize<uint>(OccurCount, name: nameof(OccurCount));
					HeaderStep = s.Serialize<uint>(HeaderStep, name: nameof(HeaderStep));
					FirstHeaderSize = s.Serialize<uint>(FirstHeaderSize, name: nameof(FirstHeaderSize));
					FileStep = s.Serialize<uint>(FileStep, name: nameof(FileStep));
					SectorSize = s.Serialize<uint>(SectorSize, name: nameof(SectorSize));
					SectorsCount = s.Serialize<uint>(SectorsCount, name: nameof(SectorsCount));
					ConnectedFilesCount = s.Serialize<uint>(ConnectedFilesCount, name: nameof(ConnectedFilesCount));
				}
			});
		}

		public async Task SerializeOccur(SerializerObject s, int i) {
			if(Occurs == null) Occurs = new Pointer<OffsetTable>[OccurCount];
			Occurs[i] = GetOccurOffset(i);
			Pointer curPos = s.CurrentPointer;
			s.Goto(Occurs[i]);
			await s.FillCacheForReadAsync(RelocationTablesCount * 4 + PTC_BigFileEncoder.KeysSize);
			s.Goto(curPos);
			Occurs[i].Resolve(s, onPreSerialize: o => o.Pre_OffsetsCount = RelocationTablesCount);
		}

		public async Task<SNA_RelocationTable> SerializeRelocationTable(SerializerObject s, SNA_RelocationTable table, int occur, int mapNumber, SNA_RelocationType type) {
			if((int)type < 0 || (int)type > 3)
				throw new Exception($"Invalid relocation type: {type}");

			uint GetKey() {
				long key = 0;
				key = BitHelpers.SetBits64(key, mapNumber, 8, 0);
				key = BitHelpers.SetBits64(key, (int)type, 8, 8);
				key = BitHelpers.SetBits64(key, occur, 8, 16);
				key = BitHelpers.SetBits64(key, ~occur, 8, 24);
				return (uint)key;
			}

			if(Occurs == null || Occurs[occur] == null) await SerializeOccur(s, occur);

			var off = Occurs[occur].Value.Entries[mapNumber * 4 + (int)type];
			var key = GetKey();
			uint ProtectionKey = SNA_XORCalculator.GetProtectionKey(key);
			var oldProtectionKey = ProtectionKey;
			SNA_RelocationTable RelocationTable = table;

			Pointer curOff = s.CurrentPointer;
			try {
				s.Goto(off);
				await s.FillCacheForReadAsync(1024 * 1024); // 1 MB cache
				s.DoXOR(new SNA_XORCalculator(SNA_XORCalculator.GetCryptKey(key)), () => {
					ProtectionKey = s.Serialize<uint>(ProtectionKey, name: nameof(ProtectionKey));
					if (oldProtectionKey != ProtectionKey) {
						s.LogWarning($"SNA: Incorrect protection key algorithm. Calculated {oldProtectionKey:X8}, but read {ProtectionKey}");
					}
					RelocationTable = s.SerializeObject<SNA_RelocationTable>(RelocationTable, name: nameof(RelocationTable));
				});
			} finally {
				s.Goto(curOff);
			}

			return RelocationTable;
		}

		public Pointer<OffsetTable> GetOccurOffset(int occur) {
			occur = occur % (int)OccurCount;

			// Sinus header
			double a = 0.69314;
			double b = 1.69314;
			double c = 0.52658;
			double n = 1.06913;

			for (int i = 0; i < occur; i++) {
				n = n + a * Math.Abs(Math.Sin(b * i * i)) + c;
			}
			double value = Math.Floor((n % 1.0) * 1000000.0) / 1000000.0;
			return new Pointer<OffsetTable>(Offset + SectorSize * (uint)Math.Floor(value * SectorsCount) + FirstHeaderSize);
		}

		public class OffsetTable : BinarySerializable {
			public uint Pre_OffsetsCount { get; set; }

			public Pointer[] Entries { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.DoEncoded(new PTC_BigFileEncoder(Pre_OffsetsCount * 4), () => {
					s.DoWithDefaults(new SerializerDefaults() {
						PointerFile = Offset.File
					}, () => {
						Entries = s.SerializePointerArray(Entries, Pre_OffsetsCount, name: nameof(Entries));
					});
				});
			}
		}
	}
}
