using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SuperObject : ROMStruct {
		// Size: 20
		public ushort matrixIndex; // 0, references global indices. check load_so_matrices.
		public ushort dataIndex; // 2
		public Reference<SuperObjectArray> children; // 4
		public Reference<CompressedVector3Array> comprVec3Array; // 6. Size: 12, so 2 vector3s
		public ushort type; // 8
		public ushort num_children; // 10
		public ushort unk; // 12
		public ushort unk2;
		public uint flags; // 16

		public GenericReference data;

		protected override void ReadInternal(Reader reader) {
			matrixIndex = reader.ReadUInt16();
			dataIndex = reader.ReadUInt16();
			children = new Reference<SuperObjectArray>(reader, false);
			comprVec3Array = new Reference<CompressedVector3Array>(reader, true, v => v.length = 2);
			type = reader.ReadUInt16();
			num_children = reader.ReadUInt16();
			unk = reader.ReadUInt16();
			unk2 = reader.ReadUInt16();
			flags = reader.ReadUInt32();

			children.Resolve(reader, soa => soa.length = num_children);
			data = new GenericReference(type, dataIndex, reader, true);
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("SO @ " + Offset + " - " + type);
			if (data.Value != null && data.Value is PhysicalObject) {
				GameObject po = ((PhysicalObject)data.Value).GetGameObject();
				if(po !=  null) po.transform.SetParent(gao.transform);
			}
			if (children.Value != null) {
				foreach (Reference<SuperObject> so in children.Value.superObjects) {
					if (so.Value != null) {
						GameObject soGao = so.Value.GetGameObject();
						if (soGao != null) soGao.transform.SetParent(gao.transform);
					}
				}
			}
			return gao;
		}
	}
}
