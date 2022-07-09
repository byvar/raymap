using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAScriptSerializer.IPT {
	public class CPAScriptSection_InputAction : CPAScriptSection {

		public CPAScriptSection_InputAction(string sectionId) : base(sectionId) { }
		public override string SectionType => "InputAction";
	}
}
