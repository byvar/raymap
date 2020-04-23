using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1TriangleNoTexture : OpenSpaceStruct {
		public ushort v0;
		public ushort v1;
		public ushort v2;
		public ushort ushort_06;

		protected override void ReadInternal(Reader reader) {
			v0 = reader.ReadUInt16();
			v1 = reader.ReadUInt16();
			v2 = reader.ReadUInt16();
			ushort_06 = reader.ReadUInt16();
		}
	}
}
