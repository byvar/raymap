using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Perso3dData : ROMStruct {
		// Size: 20
		public uint dword0;
		public float float4;
		public float float8;
		public ushort ref_5;
		public Reference<ObjectsTable> objectsTable;
		public byte byte16;
		public byte byte17;
		public ushort unk18;

		protected override void ReadInternal(Reader reader) {
			dword0 = reader.ReadUInt32();
			float4 = reader.ReadSingle();
			float8 = reader.ReadSingle();
			ref_5 = reader.ReadUInt16();
			objectsTable = new Reference<ObjectsTable>(reader, true);
			byte16 = reader.ReadByte();
			byte17 = reader.ReadByte();
			unk18 = reader.ReadUInt16();
		}
	}
}
