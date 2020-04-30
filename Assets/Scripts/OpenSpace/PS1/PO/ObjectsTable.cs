using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Type = OpenSpace.Object.SuperObject.Type;

namespace OpenSpace.PS1 {
	public class ObjectsTable : OpenSpaceStruct {
		public uint unk0;
		public uint unk1;
		public Entry[] entries;
		public uint length;

		protected override void ReadInternal(Reader reader) {
			unk0 = reader.ReadUInt32();
			unk1 = reader.ReadUInt32();
			entries = new Entry[length];
			for (int i = 0; i < entries.Length; i++) {
				entries[i] = new Entry();
				entries[i].Read(reader);
			}

		}

		public class Entry {
			public Pointer off_0; // object of 0x50, 5 rows of 0x10
			public Pointer off_geo;
			public GeometricObject geo;

			public void Read(Reader reader) {
				off_0 = Pointer.Read(reader);
				off_geo = Pointer.Read(reader);
				geo = Load.FromOffsetOrRead<GeometricObject>(reader, off_geo);
			}
			public GameObject GetGameObject() {
				return geo?.GetGameObject();
			}
		}

		public GameObject GetGameObject(int i) {
			if (i < 0 || i >= length) return null;
			return entries[i]?.GetGameObject();
		}
	}
}
