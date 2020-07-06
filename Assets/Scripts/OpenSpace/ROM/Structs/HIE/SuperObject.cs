using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SuperObject : ROMStruct {
		// Size: 20
		public ushort transformIndex; // 0, references global indices. check load_so_matrices.
		public ushort dataIndex; // 2
		public Reference<SuperObjectArray> children; // 4
		public Reference<CompressedVector3Array> boundingVolume; // 6. Size: 12, so 2 vector3s
		public ushort type; // 8
		public ushort num_children; // 10
		public ushort unk; // 12
		public ushort unk2;
		public uint flags; // 16

		public GenericReference data;
		public ROMTransform transform;

		protected override void ReadInternal(Reader reader) {
			transformIndex = reader.ReadUInt16();
			dataIndex = reader.ReadUInt16();
			children = new Reference<SuperObjectArray>(reader, false);
			boundingVolume = new Reference<CompressedVector3Array>(reader, true, v => v.length = 2);
			type = reader.ReadUInt16();
			num_children = reader.ReadUInt16();
			unk = reader.ReadUInt16();
			unk2 = reader.ReadUInt16();
			flags = reader.ReadUInt32();

			transform = new ROMTransform(transformIndex);
			children.Resolve(reader, soa => soa.length = num_children);
			data = new GenericReference(type, dataIndex, reader, true);
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("SO @ " + Offset + " - " + type);

			SuperObjectComponent soc = gao.AddComponent<SuperObjectComponent>();
			gao.layer = LayerMask.NameToLayer("SuperObject");
			soc.soROM = this;
			MapLoader.Loader.controller.superObjects.Add(soc);

			if (data.Value != null) {
				if (data.Value is PhysicalObject) {
					PhysicalObjectComponent poc = ((PhysicalObject)data.Value).GetGameObject(gao);
				} else if(data.Value is Sector) {
					SectorComponent sc = ((Sector)data.Value).GetGameObject(gao);
				}
			}
			if (children.Value != null) {
				foreach (Reference<SuperObject> so in children.Value.superObjects) {
					if (so.Value != null) {
						GameObject soGao = so.Value.GetGameObject();
						if (soGao != null) {
							soc.Children.Add(soGao.GetComponent<SuperObjectComponent>());
							soGao.transform.SetParent(gao.transform);
							ROMTransform.Apply(so.Value.transform, soGao);
						}
					}
				}
			}
			return gao;
		}
	}
}
