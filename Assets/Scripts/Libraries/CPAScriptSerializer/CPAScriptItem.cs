using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAScriptSerializer {
	public abstract class CPAScriptItem
	{
		public abstract void Write(StreamWriter writer);
	}
}
