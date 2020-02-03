using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class StdGame : ROMStruct {
		// Size: 24
		public uint dword_00;
		public uint dword_04;
		public Reference<Family> family;
		public ushort word_0A;
		public ushort word_0C;
		public ushort word_0E;
		public ushort flags;
		public ushort word_12;
		public byte byte_14;
		public byte byte_15;
		public byte byte_16;
		public byte byte_17;

		protected override void ReadInternal(Reader reader) {
			dword_00 = reader.ReadUInt32();
			dword_04 = reader.ReadUInt32();
			family = new Reference<Family>(reader, true);
			word_0A = reader.ReadUInt16();
			word_0C = reader.ReadUInt16();
			word_0E = reader.ReadUInt16();
			flags = reader.ReadUInt16();
			word_12 = reader.ReadUInt16();
			byte_14 = reader.ReadByte();
			byte_15 = reader.ReadByte();
			byte_16 = reader.ReadByte();
			byte_17 = reader.ReadByte();
		}
	}
}
