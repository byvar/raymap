using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Types_R2_Demo : SNA_Types {
		protected override void InitArrays() {
			DSBTypes = new Dictionary<int, SNA_DescriptionType>() {
				// From Game.mem
				[0] = SNA_DescriptionType.MemoryDescTitle,
				[1] = SNA_DescriptionType.GAMFixMemory,
				[2] = SNA_DescriptionType.ACPFixMemory,
				[3] = SNA_DescriptionType.ACPTextMemory,
				[4] = SNA_DescriptionType.AIFixMemory,
				[5] = SNA_DescriptionType.TMPFixMemory,
				[6] = SNA_DescriptionType.IPTMemory,
				[7] = SNA_DescriptionType.SAIFixMemory,
				[8] = SNA_DescriptionType.FontMemory,
				[9] = SNA_DescriptionType.PositionMemory,

				// From level.mem
				[11] = SNA_DescriptionType.GAMLevelMemory,
				[12] = SNA_DescriptionType.AILevelMemory,
				[13] = SNA_DescriptionType.ACPLevelMemory,
				[14] = SNA_DescriptionType.SAILevelMemory,
				[15] = SNA_DescriptionType.TMPLevelMemory,

				// From both
				[16] = SNA_DescriptionType.ScriptMemory,

				// From game.dsc: level description
				[30] = SNA_DescriptionType.LevelNameTitle,
				[31] = SNA_DescriptionType.LevelName,

				// From game.dsc and game.rnd: random description
				[32] = SNA_DescriptionType.RandomDescTitle,
				[33] = SNA_DescriptionType.RandomComputeTable,
				[34] = SNA_DescriptionType.RandomReadTable,

				// From game.dsc : Directories description
				[40] = SNA_DescriptionType.DirectoryDescTitle,
				[41] = SNA_DescriptionType.DirectoryOfEngineDLL,
				[42] = SNA_DescriptionType.DirectoryOfGameData,
				[43] = SNA_DescriptionType.DirectoryOfTexts,
				[44] = SNA_DescriptionType.DirectoryOfWorld,
				[45] = SNA_DescriptionType.DirectoryOfLevels,
				[46] = SNA_DescriptionType.DirectoryOfFamilies,
				[47] = SNA_DescriptionType.DirectoryOfCharacters,
				[48] = SNA_DescriptionType.DirectoryOfAnimations,
				[49] = SNA_DescriptionType.DirectoryOfGraphicsClasses,
				[50] = SNA_DescriptionType.DirectoryOfGraphicsBanks,
				[51] = SNA_DescriptionType.DirectoryOfMechanics,
				[52] = SNA_DescriptionType.DirectoryOfSound,
				[53] = SNA_DescriptionType.DirectoryOfVisuals,
				[54] = SNA_DescriptionType.DirectoryOfEnvironment,
				[55] = SNA_DescriptionType.DirectoryOfMaterials,
				[56] = SNA_DescriptionType.DirectoryOfSaveGame,
				[57] = SNA_DescriptionType.DirectoryOfExtras,
				[58] = SNA_DescriptionType.DirectoryOfTexture,
				[59] = SNA_DescriptionType.DirectoryOfVignettes,
				[60] = SNA_DescriptionType.DirectoryOfOptions,
				[61] = SNA_DescriptionType.DirectoryOfLipsSync,
				[62] = SNA_DescriptionType.DirectoryOfZdx,
				[63] = SNA_DescriptionType.DirectoryOfEffects,

				// From game.dsc: big file description
				[64] = SNA_DescriptionType.BigFileDescTitle,
				[65] = SNA_DescriptionType.BigFileVignettes,
				[66] = SNA_DescriptionType.BigFileTextures,

				// From game.pbg & level.pbg: Vignette description
				[70] = SNA_DescriptionType.VignetteDescTitle,
				[71] = SNA_DescriptionType.LoadVignette,
				[72] = SNA_DescriptionType.LoadLevelVignette,
				[73] = SNA_DescriptionType.InitVignette,
				[74] = SNA_DescriptionType.FreeVignette,
				[75] = SNA_DescriptionType.DisplayVignette,
				[76] = SNA_DescriptionType.InitBarOutlineColor,
				[77] = SNA_DescriptionType.InitBarInsideColor,
				[78] = SNA_DescriptionType.InitBarColor,
				[79] = SNA_DescriptionType.CreateBar,
				[80] = SNA_DescriptionType.AddBar,
				[81] = SNA_DescriptionType.MaxValueBar,

				// From level.dsc
				[90] = SNA_DescriptionType.LevelDscTitle,
				[91] = SNA_DescriptionType.NumberOfAlways,
				[92] = SNA_DescriptionType.LevelDscLevelSoundBanks,
				[93] = SNA_DescriptionType.LevelLoadMap,
				[94] = SNA_DescriptionType.LevelLoadSoundBank,

				// From game.dsc: game options description
				[100] = SNA_DescriptionType.GameOptionDescTitle,
				[101] = SNA_DescriptionType.DefaultFile,
				[102] = SNA_DescriptionType.CurrentFile,
				[103] = SNA_DescriptionType.FrameSynchro,

				// from game.dsc, input description
				[110] = SNA_DescriptionType.InitInputDeviceManager,

				// from Device.ipt, active devices description
				[120] = SNA_DescriptionType.ActivateDeviceTitle,
				[121] = SNA_DescriptionType.ActivatePadAction,
				[122] = SNA_DescriptionType.ActivateJoystickAction,
				[123] = SNA_DescriptionType.ActivateKeyboardAction,
				[124] = SNA_DescriptionType.ActivateMouseAction,

				// Final section
				[0xffff] = SNA_DescriptionType.EndOfDescSection,
			};

			Modules = new SNA_Module[] {
				SNA_Module.ERM,
				SNA_Module.MMG,
				SNA_Module.GMT,
				SNA_Module.SCR,
				SNA_Module.GAM,
				SNA_Module.GEO,
				SNA_Module.IPT,
				SNA_Module.RND,
				SNA_Module.CMP,
				SNA_Module.SAI,
				SNA_Module.TMP,
				SNA_Module.FIL,
				SNA_Module.VIG,
				SNA_Module.PO,
				SNA_Module.AI,
				SNA_Module.POS,
				SNA_Module.FON,
				SNA_Module.GLD,
				SNA_Module.TMR,
				SNA_Module.SND,
			};
		}
	}
}
