

using Cysharp.Threading.Tasks;

namespace Raymap {
	public class CPA_RRRDS_Manager : CPA_U64Manager {

		string[] rrrMapBackgrounds = new string[] {
			"Background_MM_Default",
			"Background_MM_R1",
			"Background_MM_R2",
			"Background_MM_A1",
			"Background_MM_A2",
			"Background_MM_A3",
			"Background_MM_B1",
			"Background_MM_B2",
			"Background_MM_B3",
			"Background_MM_C1",
			"Background_MM_C2",
			"Background_MM_C3",
			"Background_MM_D1",
			"Background_MM_D2",
			"bg_sfx_0A",
			"bg_sfx_0B",
			"bg_sfx_1A",
			"bg_sfx_1B",
			"bg_sfx_2A",
			"bg_sfx_2B",
			"bg_sfx_3A",
			"bg_sfx_3B",
			"Interface_Ray",
			"bg_MapE_2_2",
			"bg_MapE_1_1",
			"bg_loading0",
			"bg_loading1",
			"bg_loading2",
			"bg_loading3",
			"bg_loading4",
			"Interface",
			"Interface_Ray",
			"Interface_Gar",
			"Interface_RayGear",
			"bg_com",
			"bg_rayman",
			"bg_gardien",
			"bg_MM_Default",
		};

		string[] rrrPalettes = new string[] {
			"0.pal",
			"Jauge_2.pal",
			"Jauge_Plt_1.pal",
			"Circle_Icone.pal",
			"RM_Icone.pal",
			"XP_Icone.pal",
			"Icone_Missile.pal",
			"GT_Icone.pal",
			"GF_Icone.pal",
			"GV_Icone.pal",
			"GG_Icone.pal",
			"lums_01.pal",
			"Cage_Ico1.pal",
			"Wind_Icon1.pal",
			"Serpent_Icon.pal",
			"DG_Icone.pal",
			"lap1.pal",
			"lap2.pal",
			"MapTermine.pal",
			"Cage_Ico.pal",
			"Icone_Ok.pal",
			"MM_pastillejaune.pal",
			"one.pal",
			"trophe_chiffre.pal",
			"Icone_Ok_1.pal",
			"MM_cadna.pal",
		};

		RRRPalettedTextureReference[] rrrTexRefs = new RRRPalettedTextureReference[] {
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_8.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_2.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_R.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_Fond_8.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_Fond_R.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_Fond_L.bgc"),
			new RRRPalettedTextureReference(1, 8, 8, 0, 2, "Jauge_Plt_1.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "slash.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "0.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "1.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "2.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "3.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "4.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "5.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "6.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "7.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "8.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "9.bgc"),
			new RRRPalettedTextureReference(6, 0x20, 0x20, 0x80000000, 4, "RM_Icone.bgc"),
			new RRRPalettedTextureReference(2, 0x20, 0x20, 0x80000000, 5, "XP_Icone.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 6, "Icone_Missile.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 4, "MissBB_Icone.bgc"),
			new RRRPalettedTextureReference(1, 0x40, 0x40, 0xC0000000, 3, "Circle_Icone.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 7, "GT_Icone.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 8, "GF_Icone.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 9, "GV_Icone.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xA, "GG_Icone.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 0xB, "lums_01.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 0xB, "lums_02.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 0xB, "lums_03.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico1.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico2.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico3.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xD, "Wind_Icon1.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xD, "Wind_Icon2.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xD, "Wind_Icon3.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xE, "Serpent_Icon.bgc"),
			new RRRPalettedTextureReference(1, 8, 8, 0, 3, "hud_Map_Ind.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 4, "RM_Icone_Map.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico_Map.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xF, "DG_Icone.bgc"),
			new RRRPalettedTextureReference(2, 0x20, 0x40, 0xC0008000, 0x10, "lap1.bgc"),
			new RRRPalettedTextureReference(2, 0x20, 0x40, 0xC0008000, 0x11, "lap2.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x12, "MapTermine.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x13, "Cage_Ico.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x14, "Icone_Ok.bgc"),
			new RRRPalettedTextureReference(8, 0x10, 0x10, 0x40000000, 0x15, "MM_pastillejaune.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "one.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "seven.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "twintyfive.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "fourtysix.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "nintyn.bgc"),
			new RRRPalettedTextureReference(8, 0x40, 0x40, 0xC0000000, 0x17, "trophe_chiffre.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x18, "Icone_Ok_1.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 4, "RM_Icone.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x19, "MM_cadna.bgc")
		};

		private struct RRRPalettedTextureReference {
			public string name;
			public byte width;
			public byte height;
			public byte palette;
			public RRRPalettedTextureReference(uint unk, byte width, byte height, uint unk2, byte palette, string name) {
				this.name = name;
				this.width = width;
				this.height = height;
				this.palette = palette;
			}
		}

		public override async UniTask ExportTexturesAsync(MapViewerSettings settings, string outputDir) {
			await base.ExportTexturesAsync(settings, outputDir);

			/*foreach (string bg in rrrMapBackgrounds) {
				ExportGFX("hud/" + bg + ".bgc", "hud/" + bg + ".scr", "hud/" + bg + ".pal", 32, 32);
			}
			PAL[] palettes = rrrPalettes.Select(p => new PAL(gameDataBinFolder + "hud/" + p)).ToArray();
			foreach (RRRPalettedTextureReference texRef in rrrTexRefs) {
				ExportNBFC("hud/" + texRef.name, texRef.width / 8, texRef.height / 8, palettes[texRef.palette].palette, i4: true);
			}
			ExportNBFC("hud/Cage_Icone.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Cage_Icone.pal").palette, i4: true);
			ExportNBFC("hud/Etoile_Icone.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Etoile_Icone.pal").palette, i4: true);
			ExportNBFC("hud/hud_map_corner.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/hud_Map_Ind.pal").palette, i4: true);
			ExportNBFC("hud/ICO_Wind_NZ.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/ICO_Wind_NZ.pal").palette, i4: true);
			ExportNBFC("hud/Jauge_Block.bgc", 1, 1, new PAL(gameDataBinFolder + "hud/Jauge_Block.pal").palette, i4: true);
			ExportNBFC("hud/Jauge_Fond.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Jauge_Block.pal").palette, i4: true);
			ExportNBFC("hud/Jauge_Quart.bgc", 1, 1, new PAL(gameDataBinFolder + "hud/Jauge_Block.pal").palette, i4: true);
			ExportNBFC("hud/lums_jaune.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/lums_jaune.pal").palette, i4: true);
			ExportNBFC("hud/Map2d_blocage.bgc", 1, 2, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
			ExportNBFC("hud/Map2d_Cage.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
			ExportNBFC("hud/Map2d_Crystal.bgc", 2, 4, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
			ExportNBFC("hud/Map2d_Gard_Pris.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
			ExportNBFC("hud/Map2d_Mont_Or.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
			ExportNBFC("hud/Map2d_Teleport.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
			ExportNBFC("hud/Panneau1.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Panneau1.pal").palette, i4: true);
			ExportNBFC("hud/Panneau2.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Panneau2.pal").palette, i4: true);
			ExportNBFC("hud/Panneau3.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Panneau3.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_0.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_1.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_2.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_3.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_4.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_5.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_6.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_7.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_8.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_9.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/Ray_Fnt_slash.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
			ExportNBFC("hud/trophee.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/trophee.pal").palette, i4: true);
			ExportNBFC("hud/wifi_level_0.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/wifi_level_0.pal").palette, i4: true);
			ExportNBFC("hud/wifi_level_1.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/wifi_level_0.pal").palette, i4: true);
			ExportNBFC("hud/wifi_level_2.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/wifi_level_0.pal").palette, i4: true);
			ExportNBFC("hud/wifi_level_3.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/wifi_level_0.pal").palette, i4: true);*/
		}
	}
}
