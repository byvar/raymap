using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class StdGame : ROMStruct {
		// Size: 24
		public uint dword0;
		public uint dword4;
		public Reference<Family> family;
		public ushort word10;
		public ushort word12;
		public ushort word14;
		public ushort flags;
		public ushort unk;
		public byte byte20;
		public byte byte21;
		public byte byte22;
		public byte byte23;

		protected override void ReadInternal(Reader reader) {
			dword0 = reader.ReadUInt32();
			dword4 = reader.ReadUInt32();
			family = new Reference<Family>(reader, true);
			word10 = reader.ReadUInt16();
			word12 = reader.ReadUInt16();
			word14 = reader.ReadUInt16();
			flags = reader.ReadUInt16();
			unk = reader.ReadUInt16();
			byte20 = reader.ReadByte();
			byte21 = reader.ReadByte();
			byte22 = reader.ReadByte();
			byte23 = reader.ReadByte();
		}
	}
}
