using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class Perso : OpenSpaceStruct { // Animation/state related
		public Pointer off_p3dData;
		public Pointer off_04;
		public Pointer off_08;
		public Pointer off_0C;
		public Pointer off_10;
		public Pointer off_14;

		// Parsed
		public Perso3dData p3dData;
		public string name;

		protected override void ReadInternal(Reader reader) {
			off_p3dData = Pointer.Read(reader);
			off_04 = Pointer.Read(reader);
			off_08 = Pointer.Read(reader);
			off_0C = Pointer.Read(reader);
			off_10 = Pointer.Read(reader);
			off_14 = Pointer.Read(reader);
			//Load.print(off_00 + " - " + off_04 + " - " + off_08 + " - " + off_0C + " - " + off_10 + " - " + off_14);

			p3dData = Load.FromOffsetOrRead<Perso3dData>(reader, off_p3dData);
			Pointer.DoAt(ref reader, off_04, () => {
				Pointer off_superobject = Pointer.Read(reader);
				name = reader.ReadNullDelimitedString();
				Load.print(off_superobject + " - " + name);
			});
			/*Pointer.DoAt(ref reader, off_00, () => {
				reader.ReadBytes(0x5c);
				ushort num_unk = reader.ReadUInt16();
				reader.ReadBytes(0xa);
				Pointer off_family = Pointer.Read(reader);
				Pointer.DoAt(ref reader, off_family, () => {
					for (int i = 0; i < num_unk; i++) {
						reader.ReadUInt16();
					}
					reader.ReadByte();
					byte unk = reader.ReadByte();
					if (unk != 0xFF) {
						string familyName = reader.ReadString(0x20);
						Load.print(familyName);
					}
				});
			});*/
		}
	}
}
