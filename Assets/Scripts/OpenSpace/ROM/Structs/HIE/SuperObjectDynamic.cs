using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SuperObjectDynamic : ROMStruct {
		// Size: 12
		public Reference<Perso> perso;
		public ushort transformIndex;
		public ushort flags;
		public ushort unkByte; // read as byte?
		public uint unk;
		public ROMTransform transform;

		protected override void ReadInternal(Reader reader) {
			//Loader.print(Pointer.Current(reader));
			perso = new Reference<Perso>(reader);
			transformIndex = reader.ReadUInt16();
			flags = reader.ReadUInt16();
			unkByte = reader.ReadUInt16();
			unk = reader.ReadUInt32();


			transform = new ROMTransform(transformIndex);
			perso.Resolve(reader);
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("SOD @ " + Offset);
			if (perso.Value != null) {
				GameObject persoGao = perso.Value.GetGameObject();
				persoGao.transform.SetParent(gao.transform);
			}
			/*if (data.Value != null && data.Value is PhysicalObject) {
				GameObject po = ((PhysicalObject)data.Value).GetGameObject();
				if (po != null) po.transform.SetParent(gao.transform);
			}
			if (children.Value != null) {
				foreach (Reference<SuperObject> so in children.Value.superObjects) {
					if (so.Value != null) {
						GameObject soGao = so.Value.GetGameObject();
						if (soGao != null) {
							soGao.transform.SetParent(gao.transform);
							so.Value.SetTransform(soGao);
						}
					}
				}
			}*/
			return gao;
		}
	}
}
