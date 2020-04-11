using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class Perso : OpenSpaceStruct { // Animation/state related
		public Pointer off_00;
		public Pointer off_04;
		public Pointer off_08;
		public Pointer off_0C;
		public Pointer off_10;
		public Pointer off_14;

		protected override void ReadInternal(Reader reader) {
			off_00 = Pointer.Read(reader);
			off_04 = Pointer.Read(reader);
			off_08 = Pointer.Read(reader);
			off_0C = Pointer.Read(reader);
			off_10 = Pointer.Read(reader);
			off_14 = Pointer.Read(reader);
			//Load.print(off_00 + " - " + off_04 + " - " + off_08 + " - " + off_0C + " - " + off_10 + " - " + off_14);
			Pointer.DoAt(ref reader, off_04, () => {
				Pointer off_unk = Pointer.Read(reader);
				string name = reader.ReadNullDelimitedString();
				Load.print(off_unk + " - " + name);
			});
		}
	}
}
