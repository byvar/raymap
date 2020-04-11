using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace {
	public class Placeholder : OpenSpaceStruct {
		protected override void ReadInternal(Reader reader) {
			Load.print("Placeholder @ " + Offset);
		}
	}
}
