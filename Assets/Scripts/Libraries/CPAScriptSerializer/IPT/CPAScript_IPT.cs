using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAScriptSerializer.IPT {

	public class CPAScript_IPT : CPAScript {

		public override CPAScriptSection GenerateSection(string sectionType, string sectionId)
		{
			switch (sectionType) {
				case "InputAction": return new CPAScriptSection_InputAction(sectionType, sectionId); break;
			}

			throw new ArgumentException($"Unknown section type {sectionType}");
		}
	}
}
