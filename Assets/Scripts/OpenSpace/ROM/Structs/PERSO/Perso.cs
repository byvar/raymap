using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Perso : ROMStruct {
		// Size: 20
		public Reference<Perso3dData> p3dData;
		public ushort _2;
		public ushort _4;
		public ushort _6;
		public ushort _8;
		public ushort _10;
		public Reference<StdGame> stdGame;
		public ushort _14;
		public ushort _16;
		public ushort _18;

		protected override void ReadInternal(Reader reader) {
			p3dData = new Reference<Perso3dData>(reader, true);
			_2 = reader.ReadUInt16();
			_4 = reader.ReadUInt16();
			_6 = reader.ReadUInt16();
			_8 = reader.ReadUInt16();
			_10 = reader.ReadUInt16();
			stdGame = new Reference<StdGame>(reader, true);
			_14 = reader.ReadUInt16();
			_16 = reader.ReadUInt16();
			_18 = reader.ReadUInt16();
		}
		public GameObject GetGameObject() {
			GameObject gao = new GameObject("P3dData @ " + Offset);
			if (p3dData.Value != null && p3dData.Value.objectsTable.Value != null) {
				ObjectsTable ot = p3dData.Value.objectsTable.Value;
				GameObject otGao = ot.GetGameObject();
				otGao.transform.SetParent(gao.transform);
				otGao.transform.localPosition = Vector3.zero;
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
