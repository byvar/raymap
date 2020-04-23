using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class Sector : OpenSpaceStruct {
		public Pointer off_00;
		public Pointer off_04;
		public Pointer off_08;
		public Pointer off_0C;
		public Pointer off_10;
		public Pointer off_14;
		public Pointer off_18;
		public int int_1C;
		public int int_20;
		public int int_24;
		public int int_28;
		public int int_2C;
		public int int_30;
		public int int_34;
		public int int_38;
		public int int_3C;
		public short short_40;
		public short short_42;
		public short short_44;
		public short short_46;
		public short short_48;
		public short short_4A;
		public int int_4C;
		public int int_50;

		protected override void ReadInternal(Reader reader) {
			off_00 = Pointer.Read(reader);
			off_04 = Pointer.Read(reader);
			off_08 = Pointer.Read(reader);
			off_0C = Pointer.Read(reader);
			off_10 = Pointer.Read(reader);
			off_14 = Pointer.Read(reader);
			off_18 = Pointer.Read(reader);
			int_1C = reader.ReadInt32();
			int_20 = reader.ReadInt32();
			int_24 = reader.ReadInt32();
			int_28 = reader.ReadInt32();
			int_2C = reader.ReadInt32();
			int_30 = reader.ReadInt32();
			int_34 = reader.ReadInt32();
			int_38 = reader.ReadInt32();
			int_3C = reader.ReadInt32();
			short_40 = reader.ReadInt16();
			short_42 = reader.ReadInt16();
			short_44 = reader.ReadInt16();
			short_46 = reader.ReadInt16();
			short_48 = reader.ReadInt16();
			short_4A = reader.ReadInt16();
			int_4C = reader.ReadInt32();
			int_50 = reader.ReadInt32();
		}
	}
}
