using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAScriptSerializer {
	public abstract class CPAScriptSection : CPAScriptItem
	{
		public abstract string SectionType { get; }
		public readonly string SectionId;

		public CPAScriptSection(string sectionId)
		{
			SectionId = sectionId;
		}

		public void Parse(string[] lines)
		{

		}

		public override void Write(StreamWriter writer)
		{
			writer.WriteLine($"{CPAScript.MarkSectionBegin}{SectionType}:{SectionId}");
			WriteContent(writer);
			writer.WriteLine(CPAScript.MarkSectionEnd);
		}

		public abstract void WriteContent(StreamWriter writer);
	}
}
