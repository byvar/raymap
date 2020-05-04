using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1AnimationHierarchy : OpenSpaceStruct {
		public int child;
		public int parent;

		protected override void ReadInternal(Reader reader) {
			child = reader.ReadInt32();
			parent = reader.ReadInt32();
		}
	}
}
