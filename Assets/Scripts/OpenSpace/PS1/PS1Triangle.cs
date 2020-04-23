using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1Triangle : OpenSpaceStruct {
		public ushort v0;
		public ushort v1;
		public ushort v2;
		public ushort ushort_06;
		public ushort ushort_08;
		public ushort paletteInfo;
		public ushort ushort_0C;
		public ushort pageInfo;
		public ushort ushort_10;
		public ushort ushort_12;

		protected override void ReadInternal(Reader reader) {
			v0 = reader.ReadUInt16();
			v1 = reader.ReadUInt16();
			v2 = reader.ReadUInt16();
			ushort_06 = reader.ReadUInt16();
			ushort_08 = reader.ReadUInt16();
			paletteInfo = reader.ReadUInt16(); // palette info?
			ushort_0C = reader.ReadUInt16();
			pageInfo = reader.ReadUInt16();// page info?
			ushort_10 = reader.ReadUInt16();
			ushort_12 = reader.ReadUInt16();
		}
	}
}
