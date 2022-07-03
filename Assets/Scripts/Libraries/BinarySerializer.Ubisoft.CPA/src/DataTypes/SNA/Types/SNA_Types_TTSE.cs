using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Types_TTSE : SNA_Types {
		protected override void InitArrays() {
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
				SNA_Module.BIN,
				SNA_Module.SND,
			};
		}
	}
}
