using System;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class CPA_Globals_Montreal : CPA_Globals_SNA {

		public CPA_Globals_Montreal(Context c, string map) : base(c, map) { }

		public SNA_File<SNA_Description_Montreal_Game> GameDSB_Montreal { get; set; }
		public override SNA_IDescription DirectoryDescription => GameDSB_Montreal?.Value;

		public override string LevelsDirectory =>
			Context.GetCPASettings().ApplyPathCapitalization(
				DirectoryDescription?.GetDirectory(SNA_DescriptionType.DirectoryOfLevels),
				PathCapitalizationType.All);
	}
}
