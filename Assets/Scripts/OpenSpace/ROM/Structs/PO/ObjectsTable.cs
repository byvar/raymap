using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ObjectsTable : ROMStruct {
		public Reference<ObjectsTableData> data;
		public ushort length;
		public ushort unk0;
		public ushort unk1;

        protected override void ReadInternal(Reader reader) {
			data = new Reference<ObjectsTableData>(reader);
			length = reader.ReadUInt16();
			unk0 = reader.ReadUInt16();
			unk1 = reader.ReadUInt16();

			data.Resolve(reader, d => d.length = length);
		}

		private GameObject gao;
		public GameObject Gao {
			get {
				if (gao == null) {
					gao = new GameObject("ObjectsTable @ " + Offset);
					InitGameObject();
				}
				return gao;
			}
		}

		protected void InitGameObject() {
			if (data.Value != null) {
				for (int i = 0; i < data.Value.entries.Length; i++) {
					if (data.Value.entries[i].obj.Value != null) {
						GameObject child = data.Value.entries[i].obj.Value.Gao;
						child.transform.SetParent(Gao.transform);
						child.name = "[Entry " + i + "] " + child.name;
					}
				}
			}
		}
	}
}
