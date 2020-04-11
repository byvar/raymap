using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class UnkStruct1 : OpenSpaceStruct { // Animation/state related
		public Pointer off_00;
		public ushort ushort_04;
		public ushort ushort_06;
		public int int_08;
		public Pointer off_0C;
		public uint uint_10;
		public Pointer off_14;
		public Pointer off_18;
		public byte byte_1C;
		public byte speed;
		public ushort ushort_1E;

		protected override void ReadInternal(Reader reader) {
			off_00 = Pointer.Read(reader);
			ushort_04 = reader.ReadUInt16();
			ushort_06 = reader.ReadUInt16();
			int_08 = reader.ReadInt32();
			off_0C = Pointer.Read(reader); // Points to animation data, incl name
			uint_10 = reader.ReadUInt32();
			off_14 = Pointer.Read(reader);
			off_18 = Pointer.Read(reader);
			byte_1C = reader.ReadByte();
			speed = reader.ReadByte(); // Usually 30, but can also be 20, 40, 60, 35
			ushort_1E = reader.ReadUInt16();
			Load.print("UnkStruct1 " + Offset + ": " + off_00 + " - " + off_0C + " - " + off_14 + " - " + off_18);
		}
	}
}
