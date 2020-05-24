using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {

	public class PS1GameInfo {
		public File[] files;
		public string[] maps;
		public Dictionary<string, string[]> cines;
		public string mainFile;


		public class File {
			public enum Type {
				Map,
				Actor,
				Sound,
				Menu,
				Single
			}
			public int fileID;
			public string bigfile;
			public string extension;
			public MemoryBlock[] memoryBlocks;
			public uint baseLBA;
			public Type type = Type.Map;

			public class MemoryBlock {
				public uint address; // Base address for level data
				public bool hasSoundEffects;
				public LBA main_compressed;
				public LBA overlay_game;
				public LBA overlay_cine;
				public LBA[] cutscenes;
				public bool inEngine;
				public bool exeOnly = false;

				public MemoryBlock(uint address, bool hasSoundEffects, LBA main_compressed, LBA overlay_game, LBA overlay_cine, LBA[] cutscenes = null, bool inEngine = true) {
					this.address = address;
					this.hasSoundEffects = hasSoundEffects;
					this.main_compressed = main_compressed;
					this.overlay_game = overlay_game;
					this.overlay_cine = overlay_cine;
					if (cutscenes == null) {
						cutscenes = new File.LBA[0];
					}
					this.cutscenes = cutscenes;
					this.inEngine = inEngine;
				}

				public MemoryBlock(uint address, bool hasSoundEffects, LBA main_compressed, bool inEngine = true) {
					this.address = address;
					this.hasSoundEffects = hasSoundEffects;
					this.main_compressed = main_compressed;
					this.overlay_cine = new LBA(0, 0);
					this.overlay_game = new LBA(0, 0);
					this.cutscenes = new File.LBA[0];
					this.inEngine = inEngine;
				}

				public MemoryBlock(LBA main_compressed) {
					this.address = 0;
					this.hasSoundEffects = false;
					this.main_compressed = main_compressed;
					this.overlay_cine = new LBA(0, 0);
					this.overlay_game = new LBA(0, 0);
					this.cutscenes = new File.LBA[0];
					this.inEngine = false;
				}
			}

			public class LBA {
				public uint lba;
				public uint size;
				public string name;

				public LBA(uint lba, uint size, string name = null) {
					this.lba = lba;
					this.size = size;
					this.name = name;
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
				"boss_10",
				"end",
				"mapmonde",
				"menu_st"
			},
			cines = new Dictionary<string, string[]>() {
				{ "jail_10", new string[] { "jail10" } },
				{ "jail_20", new string[] { "jail201", "jail202", } },
				{ "learn_10", new string[] { "learn10a", "learn10b", "learn10c", "learn10d", "learn10e", "learn10f" } },
				{ "learn_30", new string[] { "learn30" } },
				{ "bast_22", new string[] { "bast22" } },
				{ "ski_10", new string[] { "ski10" } },
				{ "ski_60", new string[] { "ski60" } },
				{ "batam_10", new string[] { "batam10a", "batam10b", "batam10c", "batam10d" } },
				{ "whale_10", new string[] { "whale10" } },
				{ "water_20", new string[] { "water20a", "water20b" } },
				{ "poloc", new string[] { "poloc10", "poloc20", "poloc30", "poloc40" } },
				{ "rodeo_40", new string[] { "rodeo40a", "rodeo40b", "rodeo40c", "rodeo40d" } },
				{ "vulca_20", new string[] { "vulca20a", "vulca20b" } },
				{ "vulca_25", new string[] { "vulca25" } },
				{ "rodeo_60", new string[] { "rodeo60a" } },
				{ "glob_10", new string[] { "glob10" } },
				{ "glob_20", new string[] { "glob20", "glob20b" } },
				{ "plum_10", new string[] { "plum10" } },
				{ "plum_15", new string[] { "plum15" } },
				{ "bast_10", new string[] { "bast10" } },
				{ "helic_10", new string[] { "helic10" } },
				{ "helic_20", new string[] { "helic20" } },
				{ "helic_30", new string[] { "helic30a", "helic30b" } },
				{ "morb_20", new string[] { "morb201", "morb202" } },
				{ "learn_40", new string[] { "learn40" } },
				{ "ball", new string[] { "ball10" } },
				{ "isle_10", new string[] { "isle10a", "isle10b" } },
				{ "batam_20", new string[] { "batam20" } },
				{ "fan_10", new string[] { "fan10" } },
				{ "boss_10", new string[] { "boss10a", "boss10b", "boss10c" } },
				{ "end", new string[] { "end10" } },
				{ "mapmonde", new string[] {
					"nego10a", "nego10b", "nego10c", "nego10d", "nego11",
					"nego20a", "nego20b", "nego20c", "nego20d", "nego21",
					"nego30a", "nego30b", "nego30c", "nego30d", "nego31",
					"nego40a", "nego40b", "nego40c", "nego40d", "nego41"
				} },
				{ "cask_10", new string[] { "cask10" } },
				{ "boat01", new string[] { "boat10" } },
				{ "nave_10", new string[] { "nave10" } },
			},
			files = new File[] {
				new File() {
					fileID = 0,
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

		public static PS1GameInfo DD_PS1_US = new PS1GameInfo() {
			maps = new string[] {
				"warproom",
				"forjp",
				"forem",
				"for2d",
				"forbonus",
				"cityjp",
				"cityem",
				"city2d",
				"citybonus",
				"housejp",
				"housem",
				"house2d",
				"housebonus",
				"incajp",
				"incaem",
				"inca2d",
				"incabonus",
				"forboss",
				"cityboss",
				"houseboss",
				"incaboss",
				"forch",
				"citych",
				"housech",
				"incach",
				"housejp_2",
				"incabonus_2",
				"menu",
			},
			files = new File[] {
				new File() {
					fileID = 0,
					bigfile = "COMBIN",
					extension = "DAT",
					baseLBA = 0x1F4,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x1F4, 0x2DA)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x4CE, 0x2E8)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x7B6, 0x2DE)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0xA94, 0x2D5)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0xD69, 0x2EA)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x1053, 0x2F4)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x1347, 0x301)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x1648, 0x2CE)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x1916, 0x2D3)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x1BE9, 0x2E2)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x1ECB, 0x30D)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x21D8, 0x30A)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x24E2, 0x2C2)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x27A4, 0x2EB)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x2A8F, 0x2E6)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x2D75, 0x2EC)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x3061, 0x2BE)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x331F, 0x287)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x35A6, 0x269)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x380F, 0x22D)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x3A3C, 0x217)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x3C53, 0x2EB)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x3F3E, 0x2BF)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x41FD, 0x2DE)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x44DB, 0x293)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x476E, 0x2E9)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x4A57, 0x2C7)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x4D1E, 0x14B)),
						new File.MemoryBlock(0x800b8e00, true, new File.LBA(0x4E69, 0x1B), inEngine: false) { exeOnly = true },
					}
				}
			}
		};

		public static PS1GameInfo VIP_PS1_US = new PS1GameInfo() {
			maps = new string[] {
				"l1m1s1",
				"l1m1s2",
				"l1m1s3",
				"l1m1s4",
				"l1m1s5",
				"l1m2s1",
				"l1m2s2",
				"l1m2s3",
				"l1m2s4",
				"l1m2s5",
				"l1m3s1",
				"l2m1s1",
				"l2m1s2",
				"l2m1s4",
				"l2m2s1",
				"l2m2s2",
				"l2m2s3",
				"l2m2s4",
				"l2m2s5",
				"l2m2s6",
				"l2m2s7",
				"l2m2s8",
				"l3m2s1",
				"l3m2s2",
				"l3m3s1",
				"l3m4s1",
				"l3m5s1",
				"l3m5s2",
				"l3m5s3",
				"l3m7s1",
				"l3m8s1",
				"l3m8s2",
				"l3m8s3",
				"l3m8s4",
				"l3m8s5",
				"l4m1s1",
				"l4m1s2",
				"l4m1s3",
				"l4m1s5",
				"l4m2s1",
				"l4m2s2",
				"l4m3s1",
				"l4m3s2",
				"l4m3s3",
				"l4m3s4",
				"l4m4s1",
				"l4m4s2",
				"l4m4s3",
				"l4m5s1",
				"l4m5s2",
				"l4m6s1",
				"l4m6s2",
				"menu_st",
				"menu_sp",
			},
			files = new File[] {
				new File() {
					fileID = 0,
					bigfile = "MAPS",
					extension = "DAT",
					baseLBA = 0x1F4,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(0x800B8B78, true, new File.LBA(0x1F4, 0x2AB)),
						new File.MemoryBlock(0x800C6B34, true, new File.LBA(0x49F, 0x294)),
						new File.MemoryBlock(0x800CBEC8, true, new File.LBA(0x733, 0x26A)),
						new File.MemoryBlock(0x80116084, true, new File.LBA(0x99D, 0x231)),
						new File.MemoryBlock(0x800C1314, true, new File.LBA(0xBCE, 0x262)),
						new File.MemoryBlock(0x800BFBE4, true, new File.LBA(0xE30, 0x252)),
						new File.MemoryBlock(0x800B6A20, true, new File.LBA(0x1082, 0x25C)),
						new File.MemoryBlock(0x800BF818, true, new File.LBA(0x12DE, 0x26B)),
						new File.MemoryBlock(0x800E6328, true, new File.LBA(0x1549, 0x229)),
						new File.MemoryBlock(0x800C6F1C, true, new File.LBA(0x1772, 0x253)),
						new File.MemoryBlock(0x800EC8F0, true, new File.LBA(0x19C5, 0x200)),
						new File.MemoryBlock(0x80102D60, true, new File.LBA(0x1BC5, 0x223)),
						new File.MemoryBlock(0x8016C6B4, true, new File.LBA(0x1DE8, 0xC9)),
						new File.MemoryBlock(0x800C6384, true, new File.LBA(0x1EB1, 0x262)),
						new File.MemoryBlock(0x801179BC, true, new File.LBA(0x2113, 0x1D4)),
						new File.MemoryBlock(0x8010E970, true, new File.LBA(0x22E7, 0x23E)),
						new File.MemoryBlock(0x800DC9D0, true, new File.LBA(0x2525, 0x250)),
						new File.MemoryBlock(0x800CAAFC, true, new File.LBA(0x2775, 0x271)),
						new File.MemoryBlock(0x800C6924, true, new File.LBA(0x29E6, 0x25D)),
						new File.MemoryBlock(0x800C42D8, true, new File.LBA(0x2C43, 0x262)),
						new File.MemoryBlock(0x800FE71C, true, new File.LBA(0x2EA5, 0x216)),
						new File.MemoryBlock(0x800B5AF4, true, new File.LBA(0x30BB, 0x272)),
						new File.MemoryBlock(0x800F50EC, true, new File.LBA(0x332D, 0x214)),
						new File.MemoryBlock(0x800C54EC, true, new File.LBA(0x3541, 0x22F)),
						new File.MemoryBlock(0x800CA410, true, new File.LBA(0x3770, 0x279)),
						new File.MemoryBlock(0x8010D0D0, true, new File.LBA(0x39E9, 0x1C1)),
						new File.MemoryBlock(0x800F7278, true, new File.LBA(0x3BAA, 0x23E)),
						new File.MemoryBlock(0x800BE9B8, true, new File.LBA(0x3DE8, 0x24F)),
						new File.MemoryBlock(0x800E1CC4, true, new File.LBA(0x4037, 0x240)),
						new File.MemoryBlock(0x800E1A40, true, new File.LBA(0x4277, 0x235)),
						new File.MemoryBlock(0x800B86F8, true, new File.LBA(0x44AC, 0x28A)),
						new File.MemoryBlock(0x800C4A9C, true, new File.LBA(0x4736, 0x274)),
						new File.MemoryBlock(0x800EAC64, true, new File.LBA(0x49AA, 0x25E)),
						new File.MemoryBlock(0x800CD850, true, new File.LBA(0x4C08, 0x25E)),
						new File.MemoryBlock(0x8016341C, true, new File.LBA(0x4E66, 0x155)),
						new File.MemoryBlock(0x800AD5E0, true, new File.LBA(0x4FBB, 0x269)),
						new File.MemoryBlock(0x800CA060, true, new File.LBA(0x5224, 0x254)),
						new File.MemoryBlock(0x800DA9B4, true, new File.LBA(0x5478, 0x237)),
						new File.MemoryBlock(0x800BB530, true, new File.LBA(0x56AF, 0x243)),
						new File.MemoryBlock(0x800BE648, true, new File.LBA(0x58F2, 0x239)),
						new File.MemoryBlock(0x800FD4EC, true, new File.LBA(0x5B2B, 0x223)),
						new File.MemoryBlock(0x800C23B0, true, new File.LBA(0x5D4E, 0x270)),
						new File.MemoryBlock(0x800CE38C, true, new File.LBA(0x5FBE, 0x235)),
						new File.MemoryBlock(0x800DB1F8, true, new File.LBA(0x61F3, 0x275)),
						new File.MemoryBlock(0x800EFFAC, true, new File.LBA(0x6468, 0x24C)),
						new File.MemoryBlock(0x800D40FC, true, new File.LBA(0x66B4, 0x231)),
						new File.MemoryBlock(0x800C2D84, true, new File.LBA(0x68E5, 0x241)),
						new File.MemoryBlock(0x800ADC94, true, new File.LBA(0x6B26, 0x277)),
						new File.MemoryBlock(0x80129E30, true, new File.LBA(0x6D9D, 0x1CA)),
						new File.MemoryBlock(0x800BBBF8, true, new File.LBA(0x6F67, 0x299)),
						new File.MemoryBlock(0x800D1D7C, true, new File.LBA(0x7200, 0x2A6)),
						new File.MemoryBlock(0x800EA5AC, true, new File.LBA(0x74A6, 0x1E3)),
						new File.MemoryBlock(0x8016BAB8, true, new File.LBA(0x7689, 0x121)),
						new File.MemoryBlock(0x8015B698, true, new File.LBA(0x77AA, 0x13F)),
					}
				}
			}
		};

		public static PS1GameInfo JB_PS1_US = new PS1GameInfo() {
			maps = new string[] {
				"map0",
				"map1",
				"map2",
				"map3",
				"map4",
				"map5",
				"map6",
				"map7",
				"map8",
				"map9",
				"map10",
				"map11",
				"map12",
				"map13",
				"map14",
				"map15",
				"map16",
				"map17",
				"map18",
				"map19"
			},
			files = new File[] {
				new File() {
					fileID = 0,
					bigfile = "MAPS",
					extension = "DAT",
					baseLBA = 0x1F4,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(0x800C1648, false, new File.LBA(0x1F4, 0x115)),
						new File.MemoryBlock(0x800C98F0, false, new File.LBA(0x309, 0x113)),
						new File.MemoryBlock(0x800C1DEC, false, new File.LBA(0x41C, 0x111)),
						new File.MemoryBlock(0x800C8F98, false, new File.LBA(0x52D, 0x10E)),
						new File.MemoryBlock(0x800BFE60, false, new File.LBA(0x63B, 0x144)),
						new File.MemoryBlock(0x800C58F4, false, new File.LBA(0x77F, 0x121)),
						new File.MemoryBlock(0x800C1D68, false, new File.LBA(0x8A0, 0xF6)),
						new File.MemoryBlock(0x800C2080, false, new File.LBA(0x996, 0xF1)),
						new File.MemoryBlock(0x800BCEDC, false, new File.LBA(0xA87, 0x11A)),
						new File.MemoryBlock(0x800C15C0, false, new File.LBA(0xBA1, 0x11C)),
						new File.MemoryBlock(0x800C9868, false, new File.LBA(0xCBD, 0x11D)),
						new File.MemoryBlock(0x800C1D64, false, new File.LBA(0xDDA, 0x116)),
						new File.MemoryBlock(0x800C8F10, false, new File.LBA(0xEF0, 0x118)),
						new File.MemoryBlock(0x800BFDD8, false, new File.LBA(0x1008, 0x14E)),
						new File.MemoryBlock(0x800C586C, false, new File.LBA(0x1156, 0x12A)),
						new File.MemoryBlock(0x800C1CE0, false, new File.LBA(0x1280, 0xFF)),
						new File.MemoryBlock(0x800C1FF8, false, new File.LBA(0x137F, 0xF9)),
						new File.MemoryBlock(0x800BF488, false, new File.LBA(0x1478, 0x116)),
						new File.MemoryBlock(0x800D072C, false, new File.LBA(0x158E, 0x100)),
						new File.MemoryBlock(0x800BFE60, false, new File.LBA(0x168E, 0x13F))
					}
				},
				new File() {
					fileID = 1,
					type = File.Type.Actor,
					bigfile = "ACTOR1",
					extension = "DAT",
					baseLBA = 0x1B198,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(new File.LBA(0x1B198, 0xA1)),
						new File.MemoryBlock(new File.LBA(0x1B2DE, 0xAA)),
						new File.MemoryBlock(new File.LBA(0x1B388, 0x8E)),
						new File.MemoryBlock(new File.LBA(0x1B416, 0x95)),
						new File.MemoryBlock(new File.LBA(0x1B4AB, 0xC0)),
						new File.MemoryBlock(new File.LBA(0x1B56B, 0x9F)),
						new File.MemoryBlock(new File.LBA(0x1B60A, 0x97)),
						new File.MemoryBlock(new File.LBA(0x1B6A1, 0x95)),
						new File.MemoryBlock(new File.LBA(0x1B736, 0x80)),
						new File.MemoryBlock(new File.LBA(0x1B7B6, 0x84)),
						new File.MemoryBlock(new File.LBA(0x1B83A, 0x81)),
						new File.MemoryBlock(new File.LBA(0x1B8BB, 0x7D)),
						new File.MemoryBlock(new File.LBA(0x1B938, 0x7E)),
						new File.MemoryBlock(new File.LBA(0x1B9B6, 0x82)),
						new File.MemoryBlock(new File.LBA(0x1BA38, 0x81)),
						new File.MemoryBlock(new File.LBA(0x1BAB9, 0x80)),
						new File.MemoryBlock(new File.LBA(0x1BB39, 0x80)),
						new File.MemoryBlock(new File.LBA(0x1BBB9, 0x86)),
						new File.MemoryBlock(new File.LBA(0x1BC3F, 0x82)),
						new File.MemoryBlock(new File.LBA(0x1BCC1, 0x7F)),
						new File.MemoryBlock(new File.LBA(0x1BD40, 0x7F)),
						new File.MemoryBlock(new File.LBA(0x1BDBF, 0x7C)),
						new File.MemoryBlock(new File.LBA(0x1BE3B, 0x72)),
						new File.MemoryBlock(new File.LBA(0x1BEAD, 0x7F)),
						new File.MemoryBlock(new File.LBA(0x1BF2C, 0x86)),
						new File.MemoryBlock(new File.LBA(0x1BFB2, 0x85))
					}
				},
				new File() {
					fileID = 2,
					type = File.Type.Actor,
					bigfile = "ACTOR2",
					extension = "DAT",
					baseLBA = 0x1C520,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(new File.LBA(0x1C520, 0x65)),
						new File.MemoryBlock(new File.LBA(0x1C585, 0x6C)),
						new File.MemoryBlock(new File.LBA(0x1C5F1, 0x6E)),
						new File.MemoryBlock(new File.LBA(0x1C65F, 0x79)),
						new File.MemoryBlock(new File.LBA(0x1C6D8, 0x7E)),
						new File.MemoryBlock(new File.LBA(0x1C756, 0x5F)),
						new File.MemoryBlock(new File.LBA(0x1C7B5, 0x77)),
						new File.MemoryBlock(new File.LBA(0x1C82C, 0x90)),
						new File.MemoryBlock(new File.LBA(0x1C8BC, 0x76)),
						new File.MemoryBlock(new File.LBA(0x1C932, 0x80)),
						new File.MemoryBlock(new File.LBA(0x1C9B2, 0x85)),
						new File.MemoryBlock(new File.LBA(0x1CA37, 0x81)),
						new File.MemoryBlock(new File.LBA(0x1CAB8, 0x7D)),
						new File.MemoryBlock(new File.LBA(0x1CB35, 0x7F)),
						new File.MemoryBlock(new File.LBA(0x1CBB4, 0x82)),
						new File.MemoryBlock(new File.LBA(0x1CC36, 0x82)),
						new File.MemoryBlock(new File.LBA(0x1CCB8, 0x80)),
						new File.MemoryBlock(new File.LBA(0x1CD38, 0x81)),
						new File.MemoryBlock(new File.LBA(0x1CDB9, 0x87)),
						new File.MemoryBlock(new File.LBA(0x1CE40, 0x83)),
						new File.MemoryBlock(new File.LBA(0x1CEC3, 0x7F)),
						new File.MemoryBlock(new File.LBA(0x1CF42, 0x7F)),
						new File.MemoryBlock(new File.LBA(0x1CFC1, 0x7D)),
						new File.MemoryBlock(new File.LBA(0x1D03E, 0x73)),
						new File.MemoryBlock(new File.LBA(0x1D0B1, 0x7E)),
						new File.MemoryBlock(new File.LBA(0x1D12F, 0x86)),
						new File.MemoryBlock(new File.LBA(0x1D1B5, 0x85))
					}
				},
				new File() {
					fileID = 3,
					type = File.Type.Sound,
					bigfile = "SNDBANKS",
					extension = "DAT",
					baseLBA = 0x1D4C0,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(new File.LBA(0x1D4C0, 6)),
						new File.MemoryBlock(new File.LBA(0x1D4C6, 6)),
						new File.MemoryBlock(new File.LBA(0x1D4CC, 6)),
						new File.MemoryBlock(new File.LBA(0x1D4D2, 0xC)),
						new File.MemoryBlock(new File.LBA(0x1D4DE, 6)),
						new File.MemoryBlock(new File.LBA(0x1D4E4, 4)),
						new File.MemoryBlock(new File.LBA(0x1D4E8, 8)),
						new File.MemoryBlock(new File.LBA(0x1D4F0, 6)),
						new File.MemoryBlock(new File.LBA(0x1D4F6, 3)),
						new File.MemoryBlock(new File.LBA(0x1D4F9, 7)),
						new File.MemoryBlock(new File.LBA(0x1D500, 0xCB)),
						new File.MemoryBlock(new File.LBA(0x1D5CB, 0x99)),
						new File.MemoryBlock(new File.LBA(0x1D664, 0xDD)),
						new File.MemoryBlock(new File.LBA(0x1D741, 0xA1)),
						new File.MemoryBlock(new File.LBA(0x1D7E2, 0xB3))
					}
				},
			}
		};

		public static PS1GameInfo RR_PS1_US = new PS1GameInfo() {
			maps = new string[] {
				"canopy1",
				"crypt",
				"crypt1",
				"crypt3",
				"map4",
				"map5",
				"map6",
				"lagoon0",
				"lagoon1",
				"lagoon2",
				"pirate1",
				"pirate2",
				"map12",
			},
			files = new File[] {
				new File() {
					fileID = 0,
					type = File.Type.Map,
					bigfile = "MAPS",
					extension = "DAT",
					baseLBA = 0x1F4,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(0x800F33F8, false, new File.LBA(0x1F4, 0x16A)),
						new File.MemoryBlock(0x800DD310, false, new File.LBA(0x35E, 0x190)),
						new File.MemoryBlock(0x80103214, false, new File.LBA(0x4EE, 0x16E)),
						new File.MemoryBlock(0x800E8ECC, false, new File.LBA(0x65C, 0x199)),
						new File.MemoryBlock(0x80115768, false, new File.LBA(0x7F5, 0x131)),
						new File.MemoryBlock(0x800EBBF4, false, new File.LBA(0x926, 0x15D)),
						new File.MemoryBlock(0x800E5B78, false, new File.LBA(0xA83, 0x15F)),
						new File.MemoryBlock(0x801101EC, false, new File.LBA(0xBE2, 0x149)),
						new File.MemoryBlock(0x800E5B98, false, new File.LBA(0xD2B, 0x186)),
						new File.MemoryBlock(0x800DAB98, false, new File.LBA(0xEB1, 0x180)),
						new File.MemoryBlock(0x800FB244, false, new File.LBA(0x1031, 0x166)),
						new File.MemoryBlock(0x800DF5F0, false, new File.LBA(0x1197, 0x187)),
						new File.MemoryBlock(0x800C3C4C, false, new File.LBA(0x131E, 0x185)),
						new File.MemoryBlock(0x80110000, false, new File.LBA(0x14A3, 0x1D), inEngine: false) { exeOnly = true },
					}
				},
				new File() {
					fileID = 1,
					type = File.Type.Actor,
					bigfile = "ACTOR1",
					extension = "DAT",
					baseLBA = 0x1770,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(new File.LBA(0x1770, 0x35)),
						new File.MemoryBlock(new File.LBA(0x17A5, 0x3B)),
						new File.MemoryBlock(new File.LBA(0x17E0, 0x37)),
						new File.MemoryBlock(new File.LBA(0x1817, 0x33)),
						new File.MemoryBlock(new File.LBA(0x184A, 0x3B)),
						new File.MemoryBlock(new File.LBA(0x1885, 0x38)),
						new File.MemoryBlock(new File.LBA(0x18BD, 0x38)),
						new File.MemoryBlock(new File.LBA(0x18F5, 0x39)),
						new File.MemoryBlock(new File.LBA(0x192E, 0x11)),
						new File.MemoryBlock(new File.LBA(0x193F, 0x11))
					}
				},
				new File() {
					fileID = 2,
					type = File.Type.Actor,
					bigfile = "ACTOR2",
					extension = "DAT",
					baseLBA = 0x1B58,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(new File.LBA(0x1B58, 0x37)),
						new File.MemoryBlock(new File.LBA(0x1B8F, 0x3B)),
						new File.MemoryBlock(new File.LBA(0x1BCA, 0x39)),
						new File.MemoryBlock(new File.LBA(0x1C03, 0x34)),
						new File.MemoryBlock(new File.LBA(0x1C37, 0x3B)),
						new File.MemoryBlock(new File.LBA(0x1C72, 0x38)),
						new File.MemoryBlock(new File.LBA(0x1CAA, 0x38)),
						new File.MemoryBlock(new File.LBA(0x1CE2, 0x39)),
						new File.MemoryBlock(new File.LBA(0x1D1B, 0xE)),
						new File.MemoryBlock(new File.LBA(0x1D29, 0xE))
					}
				},
				new File() {
					fileID = 3,
					type = File.Type.Sound,
					bigfile = "SNDBANKS",
					extension = "DAT",
					baseLBA = 0x1F40,
					memoryBlocks = new File.MemoryBlock[] {
						new File.MemoryBlock(new File.LBA(0x1F40, 0x1E)),
						new File.MemoryBlock(new File.LBA(0x1F5E, 0x22)),
						new File.MemoryBlock(new File.LBA(0x1F80, 0x11)),
						new File.MemoryBlock(new File.LBA(0x1F91, 0x31)),
						new File.MemoryBlock(new File.LBA(0x1FC2, 0x20)),
						new File.MemoryBlock(new File.LBA(0x1FE2, 0x23)),
						new File.MemoryBlock(new File.LBA(0x2005, 0x31)),
						new File.MemoryBlock(new File.LBA(0x2036, 0xE)),
						new File.MemoryBlock(new File.LBA(0x2044, 0x22)),
						new File.MemoryBlock(new File.LBA(0x2066, 0x29)),
						new File.MemoryBlock(new File.LBA(0x208F, 0x12)),
						new File.MemoryBlock(new File.LBA(0x20A1, 0x1B)),
						new File.MemoryBlock(new File.LBA(0x20BC, 0x6C)),
						new File.MemoryBlock(new File.LBA(0x2128, 0x20)),
						new File.MemoryBlock(new File.LBA(0x2148, 0x21)),
						new File.MemoryBlock(new File.LBA(0x2169, 0x11)),
						new File.MemoryBlock(new File.LBA(0x217A, 0x15)),
						new File.MemoryBlock(new File.LBA(0x218F, 0x15)),
						new File.MemoryBlock(new File.LBA(0x21A4, 0x2D)),
						new File.MemoryBlock(new File.LBA(0x21D1, 5)),
						new File.MemoryBlock(new File.LBA(0x21D6, 0x1B)),
						new File.MemoryBlock(new File.LBA(0x21F1, 0x22)),
						new File.MemoryBlock(new File.LBA(0x2213, 0x16)),
						new File.MemoryBlock(new File.LBA(0x2229, 0x27))
					}
				},
			}
		};

		public static Dictionary<Settings.Mode, PS1GameInfo> Games = new Dictionary<Settings.Mode, PS1GameInfo>() {
			{ Settings.Mode.Rayman2PS1, R2_PS1_US },
			{ Settings.Mode.RaymanRushPS1, RR_PS1_US },
			{ Settings.Mode.DonaldDuckPS1, DD_PS1_US },
			{ Settings.Mode.VIPPS1, VIP_PS1_US },
			{ Settings.Mode.JungleBookPS1, JB_PS1_US },
		};
	}
}
