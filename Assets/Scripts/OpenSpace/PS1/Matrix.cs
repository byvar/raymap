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

		public int int_14;
		public int int_18;
		public int int_1C;
		public int int_20;
		public int int_24;



		protected override void ReadInternal(Reader reader) {
			int_00 = reader.ReadInt32();
			int_04 = reader.ReadInt32();
			int_08 = reader.ReadInt32();
			int_0C = reader.ReadInt32();
			int_10 = reader.ReadInt32();
			int_14 = reader.ReadInt32();
			int_18 = reader.ReadInt32();
			int_1C = reader.ReadInt32();
			int_20 = reader.ReadInt32();
			int_24 = reader.ReadInt32();
		}
	}
}
