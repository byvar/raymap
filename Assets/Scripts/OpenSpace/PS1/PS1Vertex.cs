using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1Vertex : OpenSpaceStruct {
		public short x;
		public short y;
		public short z;
		public ushort ushort_06;
		public byte r;
		public byte g;
		public byte b;
		public byte padding;

		protected override void ReadInternal(Reader reader) {
			x = reader.ReadInt16();
			y = reader.ReadInt16();
			z = reader.ReadInt16();
			ushort_06 = reader.ReadUInt16();
			r = reader.ReadByte();
			g = reader.ReadByte();
			b = reader.ReadByte();
			padding = reader.ReadByte();
		}
	}
}
