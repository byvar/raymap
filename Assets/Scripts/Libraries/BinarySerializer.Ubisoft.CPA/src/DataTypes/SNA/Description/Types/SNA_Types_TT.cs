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

				// ??? gap

				// BigFiles
				[19] = SNA_DescriptionType.BigFileVignettes,
				[20] = SNA_DescriptionType.BigFileTextures,
				[21] = SNA_DescriptionType.TT_BigFileCredits,

				// GameOptionsFile
				[22] = SNA_DescriptionType.TT_CreditsLevelName,
				[23] = SNA_DescriptionType.TT_SkipMainMenu,
				[24] = SNA_DescriptionType.DefaultFile,
				[25] = SNA_DescriptionType.CurrentFile,

				[0xffff] = SNA_DescriptionType.EndOfDescSection
			};
		}
	}
}
