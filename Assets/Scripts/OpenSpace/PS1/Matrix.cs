using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class Matrix : OpenSpaceStruct {
		public int int_00;
		public int int_04;
		public int int_08;
		public int int_0C;
		public int int_10;

		public int x;
		public int y;
		public int z;
		public int int_20;
		public int int_24;



		protected override void ReadInternal(Reader reader) {
			int_00 = reader.ReadInt32();
			int_04 = reader.ReadInt32();
			int_08 = reader.ReadInt32();
			int_0C = reader.ReadInt32();
			int_10 = reader.ReadInt32();
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
			int_20 = reader.ReadInt32();
			int_24 = reader.ReadInt32();
		}
	}
}
