using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Types_Montreal : SNA_Types {
		protected override void InitArrays() {
			DSBTypes = new Dictionary<int, SNA_DescriptionType>() {
				[0xffff] = SNA_DescriptionType.EndOfDescSection,

				[0x10000] = SNA_DescriptionType.DirectoryOfEngineDLL,
				[0x10001] = SNA_DescriptionType.DirectoryOfGameData,
				[0x10002] = SNA_DescriptionType.DirectoryOfWorld,
				[0x10003] = SNA_DescriptionType.DirectoryOfLevels,
				[0x10004] = SNA_DescriptionType.DirectoryOfSound,
				[0x10005] = SNA_DescriptionType.DirectoryOfSaveGame,
				[0x10006] = SNA_DescriptionType.DirectoryOfTexture,
				[0x10007] = SNA_DescriptionType.DirectoryOfFixTexture,
				[0x10008] = SNA_DescriptionType.DirectoryOfVignettes,
				[0x10009] = SNA_DescriptionType.DirectoryOfOptions,
			};
		}
	}
}
