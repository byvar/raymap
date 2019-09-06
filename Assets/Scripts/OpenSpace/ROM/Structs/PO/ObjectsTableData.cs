using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ObjectsTableData : ROMStruct {
		public Entry[] entries;
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			entries = new Entry[length];
			for (int i = 0; i < entries.Length; i++) {
				entries[i] = new Entry(reader);
			}
        }

		public struct Entry {
			public ushort entryType;
			public ushort unk1;
			public Reference<PhysicalObject> obj;
			public ushort index_scale;
			public ushort type;
			public byte unk2;
			public byte unk3;

			public uint unk4;
			public byte unk5;
			public byte unk6;

			public Entry(Reader reader) {
				entryType = reader.ReadUInt16();
				if (entryType != 0) {
					unk1 = reader.ReadUInt16();
					unk4 = reader.ReadUInt32();
					unk5 = reader.ReadByte();
					unk6 = reader.ReadByte();
					unk2 = reader.ReadByte();
					unk3 = reader.ReadByte();

					// Unused stuff for this entry type
					obj = new Reference<PhysicalObject>();
					index_scale = 0;
					type = 0xFFFF;
				} else {
					unk1 = reader.ReadUInt16();
					obj = new Reference<PhysicalObject>(reader, true);
					index_scale = reader.ReadUInt16();
					type = reader.ReadUInt16();
					unk2 = reader.ReadByte();
					unk3 = reader.ReadByte();

					// Unused stuff for this entry type
					unk4 = 0;
					unk5 = 0;
					unk6 = 0;
				}
			}
		}
	}
}
