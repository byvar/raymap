using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class TriangleNoTexture : OpenSpaceStruct, IPS1Polygon {
		public ushort v0;
		public ushort v1;
		public ushort v2;
		public ushort ushort_06;

		public TextureBounds Texture => null;

		protected override void ReadInternal(Reader reader) {
			v0 = reader.ReadUInt16();
			v1 = reader.ReadUInt16();
			v2 = reader.ReadUInt16();
			ushort_06 = reader.ReadUInt16();
		}
	}
}
