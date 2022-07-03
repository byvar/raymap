using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Types_TT : SNA_Types {
		protected override void InitArrays() {
			DSBTypes = new Dictionary<int, SNA_DescriptionType>() {
				// Memory
				[0] = SNA_DescriptionType.GAMFixMemory,
				[1] = SNA_DescriptionType.GAMLevelMemory,
				[2] = SNA_DescriptionType.TT_MenuMemory,
				[3] = SNA_DescriptionType.TT_FontMemory,
				[4] = SNA_DescriptionType.SAIFixMemory,
				[5] = SNA_DescriptionType.SAILevelMemory,
				[6] = SNA_DescriptionType.AIFixMemory,
				[7] = SNA_DescriptionType.AILevelMemory,
				[8] = SNA_DescriptionType.TMPFixMemory,
				[9] = SNA_DescriptionType.TMPLevelMemory,
				[10] = SNA_DescriptionType.ACPTextMemory,
				[11] = SNA_DescriptionType.ACPFixMemory,
				[12] = SNA_DescriptionType.ACPLevelMemory,
				[13] = SNA_DescriptionType.TT_InventoryMemory,
				[14] = SNA_DescriptionType.PositionMemory,
				[15] = SNA_DescriptionType.ScriptMemory,
				[16] = SNA_DescriptionType.TT_LipsSynchMemory,
				[17] = SNA_DescriptionType.TT_PLAMaxSuperObject,
				[18] = SNA_DescriptionType.TT_PLAMaxMatrix,

				// BigFiles
				[19] = SNA_DescriptionType.BigFileVignettes,
				[20] = SNA_DescriptionType.BigFileTextures,
				[21] = SNA_DescriptionType.TT_BigFileCredits,

				// GameOptionsFile
				[22] = SNA_DescriptionType.TT_CreditsLevelName,
				[23] = SNA_DescriptionType.TT_SkipMainMenu,
				[24] = SNA_DescriptionType.DefaultFile,
				[25] = SNA_DescriptionType.CurrentFile,

				[0xffff] = SNA_DescriptionType.EndOfDescSection,

				[0x10000] = SNA_DescriptionType.DirectoryOfEngineDLL,
				[0x10001] = SNA_DescriptionType.DirectoryOfMenu,
				[0x10002] = SNA_DescriptionType.DirectoryOfMenuAnimations,
				[0x10003] = SNA_DescriptionType.DirectoryOfGameData,
				[0x10004] = SNA_DescriptionType.DirectoryOfTexts,
				[0x10005] = SNA_DescriptionType.DirectoryOfWorld,
				[0x10006] = SNA_DescriptionType.DirectoryOfLevels,
				[0x10007] = SNA_DescriptionType.DirectoryOfFamilies,
				[0x10008] = SNA_DescriptionType.DirectoryOfCharacters,
				[0x10009] = SNA_DescriptionType.DirectoryOfAnimations,
				[0x1000A] = SNA_DescriptionType.DirectoryOfGraphicsClasses,
				[0x1000B] = SNA_DescriptionType.DirectoryOfGraphicsBanks,
				[0x1000C] = SNA_DescriptionType.DirectoryOfMechanics,
				[0x1000D] = SNA_DescriptionType.DirectoryOfSound,
				[0x1000E] = SNA_DescriptionType.DirectoryOfVisuals,
				[0x1000F] = SNA_DescriptionType.DirectoryOfEnvironment,
				[0x10010] = SNA_DescriptionType.DirectoryOfCollideMaterials, // all DirectoryOfMaterials
				[0x10011] = SNA_DescriptionType.DirectoryOfSoundMaterials,
				[0x10012] = SNA_DescriptionType.DirectoryOfMechanicsMaterials,
				[0x10013] = SNA_DescriptionType.DirectoryOfGameMaterials,
				[0x10014] = SNA_DescriptionType.DirectoryOfSaveGame,
				[0x10015] = SNA_DescriptionType.DirectoryOfExtras,
				[0x10016] = SNA_DescriptionType.DirectoryOfTexture,
				[0x10017] = SNA_DescriptionType.DirectoryOfFixTexture,
				[0x10018] = SNA_DescriptionType.DirectoryOfVignettes,
				[0x10019] = SNA_DescriptionType.DirectoryOfOptions,
				[0x1001A] = SNA_DescriptionType.DirectoryOfLipsSync,
				[0x1001B] = SNA_DescriptionType.DirectoryOfZdx,
				[0x1001C] = SNA_DescriptionType.DirectoryOfEffects,
				[0x1001D] = SNA_DescriptionType.DirectoryOfInventory,
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
				SNA_Module.INV,
				SNA_Module.SAI,
				SNA_Module.TMP,
				SNA_Module.FIL,
				SNA_Module.VIG,
				SNA_Module.POS,
				SNA_Module.PO ,
				SNA_Module.AI ,
				SNA_Module.MNU,
				SNA_Module.FON,
				SNA_Module.LS ,
				SNA_Module.GLD,
				SNA_Module.TMR,
				SNA_Module.SND,
			};
		}
	}
}
