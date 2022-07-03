using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Types_R2 : SNA_Types_R2_Demo {
		protected override void InitArrays() {
			base.InitArrays();
			for (int i = 43; i <= 63; i++) {
				DSBTypes.Remove(i);
			}
			DSBTypes[43] = SNA_DescriptionType.DirectoryOfWorld;
			DSBTypes[44] = SNA_DescriptionType.DirectoryOfLevels;
			DSBTypes[45] = SNA_DescriptionType.DirectoryOfSound; // And GraphicsClasses for some reason. And this directory/languageID as 2nd sound path
			DSBTypes[46] = SNA_DescriptionType.DirectoryOfSaveGame;
			DSBTypes[47] = SNA_DescriptionType.DirectoryOfTexture;
			DSBTypes[48] = SNA_DescriptionType.DirectoryOfVignettes;
			DSBTypes[49] = SNA_DescriptionType.DirectoryOfOptions;
		}
	}
}
