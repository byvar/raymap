using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAScriptSerializer {
	public class CPAScriptComment : CPAScriptItem
	{
		public string Comment;
		public void Read(string line)
		{
			Comment = line;
		}

		public override void Write(StreamWriter writer)
		{
			writer.Write(Comment);
		}
	}
}
