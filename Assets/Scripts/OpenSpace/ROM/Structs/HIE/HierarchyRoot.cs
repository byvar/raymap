using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class HierarchyRoot : ROMStruct {
		// Size: 12
		public ushort ref_48;
		public Reference<SuperObject> fatherSector;
		public ushort ref_46;
		public ushort len_48;
		public ushort len_46;
		public ushort unk;

		protected override void ReadInternal(Reader reader) {
			ref_48 = reader.ReadUInt16();
			fatherSector = new Reference<SuperObject>(reader, true);
			ref_46 = reader.ReadUInt16();
			len_48 = reader.ReadUInt16();
			len_46 = reader.ReadUInt16();
			unk = reader.ReadUInt16();

		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("Hierarchy Root @ " + Offset);
			if (fatherSector.Value != null) {
				GameObject fs = fatherSector.Value.GetGameObject();
				fs.name = "[Father Sector] " + fs.name;
				fs.transform.SetParent(gao.transform);
				fatherSector.Value.SetTransform(fs);
			}
			return gao;
		}
	}
}
