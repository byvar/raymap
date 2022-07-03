using System;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class CPA_Globals_TTPC : CPA_Globals_SNA {

		public CPA_Globals_TTPC(Context c) : base(c) { }

		public SNA_File<SNA_Description_TT_Game> GameDSB_TTPC { get; set; }
		public override SNA_IDescription DirectoryDescription => GameDSB_TTPC?.Value;

		public override string LevelsDirectory => 
			Context.GetCPASettings().ApplyPathCapitalization(
				DirectoryDescription?.GetDirectory(SNA_DescriptionType.DirectoryOfLevels),
				PathCapitalizationType.All);
	}
}
