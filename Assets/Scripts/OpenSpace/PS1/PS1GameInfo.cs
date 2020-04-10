using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {

	public class PS1GameInfo {
		public File[] files;
		public string[] maps;


		public class File {
			public string bigfile;
			public string extension;
			public MemoryBlock[] memoryBlocks;
			public uint baseLBA;

			public class MemoryBlock {
				public uint address; // Base address for level data
				public bool isPlayable;
				public LBA compressed;
				public LBA filetable;
				public LBA uncompressed;
				public LBA[] cutscenes;
				public bool inEngine;

				public MemoryBlock(uint address, bool isSomething, LBA compressed, LBA filetable, LBA uncompressed, LBA[] cutscenes = null, bool inEngine = true) {
					this.address = address;
					this.isPlayable = isSomething;
					this.compressed = compressed;
					this.filetable = filetable;
					this.uncompressed = uncompressed;
					if (cutscenes == null) {
						cutscenes = new File.LBA[0];
					}
					this.cutscenes = cutscenes;
					this.inEngine = inEngine;
				}
			}

			public class LBA {
				public uint lba;
				public uint size;

				public LBA(uint lba, uint size) {
					this.lba = lba;
					this.size = size;
				}
			}
		}


		public static PS1GameInfo R2_PS1_US = new PS1GameInfo() {
			maps = new string[] {
				"jail_10",
				"jail_20",
				"learn_10",
				"learn_30",
				"bast_20",
				"bast_22",
				"ski_10",
				"ski_60",
				"batam_10",
				"chase_10",
				"chase_22",
				"ly_10",
				"whale_00",
				"whale_10",
				"water_20",
				"poloc",
				"rodeo_40",
				"vulca_10",
				"vulca_15",
				"vulca_20",
				"vulca_25",
				"rodeo_60",
				"glob_30",
				"glob_10",
				"glob_20",
				"plum_00",
				"plum_20",
				"plum_30",
				"plum_10",
				"plum_15",
				"bast_10",
				"cask_10",
				"cask_30",
				"nave_10",
				"nave_15",
				"nave_20",
				"earth_20",
				"earth_30",
				"ly_20",
				"helic_10",
				"helic_20",
				"helic_30",
				"morb_10",
				"morb_15",
				"morb_20",
				"learn_40",
				"ball",
				"isle_10",
				"batam_20",
				"boat01",
				"boat02",
				"astro_10",
				"fan_10",
				"boss_1",
				"end",
				"mapmonde",
				"menu_st"
			},
			files = new File[] {
				new File() {
					bigfile = "COMBIN",
					extension = "DAT",
					baseLBA = 0x1F4,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(0x801091E0, false, new File.LBA(0x1F4, 0x1C3), new File.LBA(0x3B7, 0x36000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x423, 0x533800) }),
						new File.MemoryBlock(0x80104994, true, new File.LBA(0xE8A, 0x251), new File.LBA(0x10DB, 0x51080), new File.LBA(0x117E, 0xC080), new File.LBA[] { new File.LBA(0x1197, 0x56B000), new File.LBA(0x1C6D, 0xC2000) }),
						new File.MemoryBlock(0x800EA900, true, new File.LBA(0x1DF1, 0x2DB), new File.LBA(0x20CC, 0x39074), new File.LBA(0x213F, 0x3074), new File.LBA[] { new File.LBA(0x2146, 0xF3800), new File.LBA(0x232D, 0x292800), new File.LBA(0x2852, 0x61A800), new File.LBA(0x3487, 0x584000), new File.LBA(0x3F8F, 0xAF000), new File.LBA(0x40ED, 0x1EC800) }),
						new File.MemoryBlock(0x800CD5E4, true, new File.LBA(0x44C6, 0x342), new File.LBA(0x4808, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800DD108, true, new File.LBA(0x4874, 0x30A), new File.LBA(0x4B7E, 0x39074), new File.LBA(0x4BF1, 0x3074), new File.LBA[] { new File.LBA(0x4BF8, 0x34000) }),
						new File.MemoryBlock(0x800DC208, true, new File.LBA(0x4C60, 0x315), new File.LBA(0x4F75, 0x3ACC8), new File.LBA(0x4FEB, 0x4CC8), new File.LBA[] { new File.LBA(0x4FF5, 0x7C1000) }),
						new File.MemoryBlock(0x800F4320, true, new File.LBA(0x5F77, 0x2CF), new File.LBA(0x6246, 0x396A4), new File.LBA(0x62B9, 0x36A4), new File.LBA[] { new File.LBA(0x62C0, 0x3C3000) }),
						new File.MemoryBlock(0x800D0D5C, true, new File.LBA(0x6A46, 0x30D), new File.LBA(0x6D53, 0x36544), new File.LBA(0x6DC0, 0x544), new File.LBA[] { new File.LBA(0x6DC1, 0x121000) }),
						new File.MemoryBlock(0x80120720, false, new File.LBA(0x7003, 0x1B4), new File.LBA(0x71B7, 0x45000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x7241, 0x337800), new File.LBA(0x78B0, 0x35E800), new File.LBA(0x7F6D, 0x16F800), new File.LBA(0x824C, 0x15A000) }),
						new File.MemoryBlock(0x800D92BC, true, new File.LBA(0x8500, 0x319), new File.LBA(0x8819, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800CB2CC, true, new File.LBA(0x8885, 0x30F), new File.LBA(0x8B94, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800D4CA0, true, new File.LBA(0x8C00, 0x2D8), new File.LBA(0x8ED8, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800C5AF4, true, new File.LBA(0x8F44, 0x336), new File.LBA(0x927A, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800F70B8, true, new File.LBA(0x92E6, 0x28B), new File.LBA(0x9571, 0x36528), new File.LBA(0x95DE, 0x528), new File.LBA[] { new File.LBA(0x95DF, 0x108000) }),
						new File.MemoryBlock(0x80116E64, true, new File.LBA(0x97EF, 0x285), new File.LBA(0x9A74, 0x47988), new File.LBA(0x9B04, 0x2988), new File.LBA[] { new File.LBA(0x9B0A, 0x310000), new File.LBA(0xA12A, 0x1DB000) }),
						new File.MemoryBlock(0x80151A2C, false, new File.LBA(0xA4E0, 0x14A), new File.LBA(0xA62A, 0x36000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0xA696, 0x36C000), new File.LBA(0xAD6E, 0x1DE800), new File.LBA(0xB12B, 0x170000), new File.LBA(0xB40B, 0x5A3800) }),
						new File.MemoryBlock(0x800D6058, true, new File.LBA(0xBF52, 0x33A), new File.LBA(0xC28C, 0x39074), new File.LBA(0xC2FF, 0x3074), new File.LBA[] { new File.LBA(0xC306, 0x4E800), new File.LBA(0xC3A3, 0x43D800), new File.LBA(0xCC1E, 0x201800), new File.LBA(0xD021, 0x14E000) }),
						new File.MemoryBlock(0x800D9954, true, new File.LBA(0xD2BD, 0x2AD), new File.LBA(0xD56A, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800ED134, true, new File.LBA(0xD5D6, 0x26C), new File.LBA(0xD842, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800EFCFC, true, new File.LBA(0xD8AE, 0x256), new File.LBA(0xDB04, 0x3D2B4), new File.LBA(0xDB7F, 0x72B4), new File.LBA[] { new File.LBA(0xDB8E, 0x6E800), new File.LBA(0xDC6B, 0x3F800) }),
						new File.MemoryBlock(0x801045B4, true, new File.LBA(0xDCEA, 0x276), new File.LBA(0xDF60, 0x3A550), new File.LBA(0xDFD5, 0x4550), new File.LBA[] { new File.LBA(0xDFDE, 0x370000) }),
						new File.MemoryBlock(0x800F96C0, true, new File.LBA(0xE6BE, 0x2DA), new File.LBA(0xE998, 0x39160), new File.LBA(0xEA0B, 0x3160), new File.LBA[] { new File.LBA(0xEA12, 0xEA800) }),
						new File.MemoryBlock(0x80130984, true, new File.LBA(0xEBE7, 0x216), new File.LBA(0xEDFD, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800E6ABC, true, new File.LBA(0xEE69, 0x2FF), new File.LBA(0xF168, 0x37C3C), new File.LBA(0xF1D8, 0x1C3C), new File.LBA[] { new File.LBA(0xF1DC, 0x304800) }),
						new File.MemoryBlock(0x800DEA18, true, new File.LBA(0xF7E5, 0x2F2), new File.LBA(0xFAD7, 0x3A040), new File.LBA(0xFB4C, 0x4040), new File.LBA[] { new File.LBA(0xFB55, 0xD0000), new File.LBA(0xFCF5, 0xB8800) }),
						new File.MemoryBlock(0x800CA274, true, new File.LBA(0xFE66, 0x321), new File.LBA(0x10187, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800E7F3C, true, new File.LBA(0x101F3, 0x27C), new File.LBA(0x1046F, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800F406C, true, new File.LBA(0x104DB, 0x2A5), new File.LBA(0x10780, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800CDE24, true, new File.LBA(0x107EC, 0x2E0), new File.LBA(0x10ACC, 0x36AA0), new File.LBA(0x10B3A, 0xAA0), new File.LBA[] { new File.LBA(0x10B3C, 0x285800) }),
						new File.MemoryBlock(0x80127458, true, new File.LBA(0x11047, 0x22D), new File.LBA(0x11274, 0x38304), new File.LBA(0x112E5, 0x2304), new File.LBA[] { new File.LBA(0x112EA, 0x125000) }),
						new File.MemoryBlock(0x800D9CE4, true, new File.LBA(0x11534, 0x302), new File.LBA(0x11836, 0x3A6A4), new File.LBA(0x118AB, 0x46A4), new File.LBA[] { new File.LBA(0x118B4, 0x42000) }),
						new File.MemoryBlock(0x800F4C08, true, new File.LBA(0x11938, 0x2D4), new File.LBA(0x11C0C, 0x36000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x11C78, 0xC2800) }),
						new File.MemoryBlock(0x800D34D0, true, new File.LBA(0x11DFD, 0x312), new File.LBA(0x1210F, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800E3B20, true, new File.LBA(0x1217B, 0x29E), new File.LBA(0x12419, 0x36000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x12485, 0xF4000) }),
						new File.MemoryBlock(0x800EEB9C, true, new File.LBA(0x1266D, 0x2C0), new File.LBA(0x1292D, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800C614C, true, new File.LBA(0x12999, 0x309), new File.LBA(0x12CA2, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800D4250, true, new File.LBA(0x12D0E, 0x2E0), new File.LBA(0x12FEE, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800DCC98, true, new File.LBA(0x1305A, 0x2C9), new File.LBA(0x13323, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800DF308, true, new File.LBA(0x1338F, 0x294), new File.LBA(0x13623, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800D7298, true, new File.LBA(0x1368F, 0x2CD), new File.LBA(0x1395C, 0x3D19C), new File.LBA(0x139D7, 0x719C), new File.LBA[] { new File.LBA(0x139E6, 0x4BC000) }),
						new File.MemoryBlock(0x800CBC00, true, new File.LBA(0x1435E, 0x2D1), new File.LBA(0x1462F, 0x3E1F4), new File.LBA(0x146AC, 0x81F4), new File.LBA[] { new File.LBA(0x146BD, 0x123000) }),
						new File.MemoryBlock(0x8012CA50, true, new File.LBA(0x14903, 0x251), new File.LBA(0x14B54, 0x391AC), new File.LBA(0x14BC7, 0x31AC), new File.LBA[] { new File.LBA(0x14BCE, 0x24D800), new File.LBA(0x15069, 0x92000) }),
						new File.MemoryBlock(0x800CA9DC, true, new File.LBA(0x1518D, 0x2F9), new File.LBA(0x15486, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800D5028, true, new File.LBA(0x154F2, 0x317), new File.LBA(0x15809, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x8010D49C, true, new File.LBA(0x15875, 0x2AC), new File.LBA(0x15B21, 0x3BD8C), new File.LBA(0x15B99, 0x5D8C), new File.LBA[] { new File.LBA(0x15BA5, 0x1E1800), new File.LBA(0x15F68, 0xD1800) }),
						new File.MemoryBlock(0x800C6FA0, true, new File.LBA(0x1610B, 0x320), new File.LBA(0x1642B, 0x3AB20), new File.LBA(0x164A1, 0x4B20), new File.LBA[] { new File.LBA(0x164AB, 0x1F4000) }),
						new File.MemoryBlock(0x8015688C, false, new File.LBA(0x16893, 0x11C), new File.LBA(0x169AF, 0x36000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x16A1B, 0x6F000) }),
						new File.MemoryBlock(0x800E38E0, true, new File.LBA(0x16AF9, 0x2FC), new File.LBA(0x16DF5, 0x42300), new File.LBA(0x16E7A, 0xC300), new File.LBA[] { new File.LBA(0x16E93, 0x5A7800) }),
						new File.MemoryBlock(0x8012A49C, false, new File.LBA(0x179E2, 0x196), new File.LBA(0x17B78, 0x45000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x17C02, 0x4CF000) }),
						new File.MemoryBlock(0x800F780C, true, new File.LBA(0x185A0, 0x280), new File.LBA(0x18820, 0x36000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x1888C, 0x9D800) }),
						new File.MemoryBlock(0x800EF9B0, true, new File.LBA(0x189C7, 0x280), new File.LBA(0x18C47, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x800C964C, true, new File.LBA(0x18CB3, 0x250), new File.LBA(0x18F03, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x80120CCC, false, new File.LBA(0x18F6F, 0x1C5), new File.LBA(0x19134, 0x45000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x191BE, 0x5D8800) }),
						new File.MemoryBlock(0x800EF51C, true, new File.LBA(0x19D6F, 0x2FA), new File.LBA(0x1A069, 0x58864), new File.LBA(0x1A11B, 0x13864), new File.LBA[] { new File.LBA(0x1A143, 0x325000), new File.LBA(0x1A78D, 0x591800), new File.LBA(0x1B2B0, 0x18B000) }),
						new File.MemoryBlock(0x8011D040, false, new File.LBA(0x1B5C6, 0x181), new File.LBA(0x1B747, 0x45000), new File.LBA(0, 0), new File.LBA[] { new File.LBA(0x1B7D1, 0x556800) }),
						new File.MemoryBlock(0x800BF514, true, new File.LBA(0x1C27E, 0x2E1), new File.LBA(0x1C55F, 0x463A4), new File.LBA(0x1C5EC, 0x13A4), new File.LBA[] { new File.LBA(0x1C5EF, 0x6F5800), new File.LBA(0x1D3DA, 0x405800), new File.LBA(0x1DBE5, 0x6B800), new File.LBA(0x1DCBC, 0x3BD000), new File.LBA(0x1E436, 0x7000), new File.LBA(0x1E444, 0x466000), new File.LBA(0x1ED10, 0x2D4800), new File.LBA(0x1F2B9, 0x173800), new File.LBA(0x1F5A0, 0x30F000), new File.LBA(0x1FBBE, 0x7800), new File.LBA(0x1FBCD, 0x58C800), new File.LBA(0x206E6, 0x2FE800), new File.LBA(0x20CE3, 0x11C800), new File.LBA(0x20F1C, 0x357000), new File.LBA(0x215CA, 0x7000), new File.LBA(0x215D8, 0x674800), new File.LBA(0x222C1, 0x3CD800), new File.LBA(0x22A5C, 0xE8800), new File.LBA(0x22C2D, 0x353000), new File.LBA(0x232D3, 0x7800) }),
						new File.MemoryBlock(0x80146184, true, new File.LBA(0x232E2, 0x1B3), new File.LBA(0x23495, 0x36000), new File.LBA(0, 0)),
						new File.MemoryBlock(0x80127458, true, new File.LBA(0x23501, 0x9D), new File.LBA(0x23495, 0x36000), new File.LBA(0, 0), inEngine: false)
					}
				}
			}
		};

		public static Dictionary<Settings.Mode, PS1GameInfo> Games = new Dictionary<Settings.Mode, PS1GameInfo>() {
			{ Settings.Mode.Rayman2PS1, R2_PS1_US },
		};
	}
}
