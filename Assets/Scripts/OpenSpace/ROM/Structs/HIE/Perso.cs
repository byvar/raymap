using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Perso : ROMStruct {
		// Size: 20
		public ushort ref_p3dData;
		public ushort _2;
		public ushort _4;
		public ushort _6;
		public ushort _8;
		public ushort _10;
		public ushort _12;
		public ushort _14;
		public ushort _16;
		public ushort _18;

		protected override void ReadInternal(Reader reader) {
			ref_p3dData = reader.ReadUInt16();
			_2 = reader.ReadUInt16();
			_4 = reader.ReadUInt16();
			_6 = reader.ReadUInt16();
			_8 = reader.ReadUInt16();
			_10 = reader.ReadUInt16();
			_12 = reader.ReadUInt16();
			_14 = reader.ReadUInt16();
			_16 = reader.ReadUInt16();
			_18 = reader.ReadUInt16();
		}
	}
}
