using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAScriptSerializer {
	public class CPAScriptDirective : CPAScriptItem
	{
		public string Directive;
		public void Read(string line)
		{
			Directive = line;
		}

		public override void Write(StreamWriter writer)
		{
			writer.Write(Directive);
		}
	}
}
