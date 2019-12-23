using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class HierarchyRoot : ROMStruct {
		// Size: 12
		public Reference<SuperObjectDynamicArray> persos;
		public Reference<SuperObject> fatherSector;
		public Reference<ShortArray> matrixIndices;
		public ushort len_persos;
		public ushort len_matrixIndices;
		public ushort unk;

		protected override void ReadInternal(Reader reader) {
			persos = new Reference<SuperObjectDynamicArray>(reader);
			fatherSector = new Reference<SuperObject>(reader, true);
			matrixIndices = new Reference<ShortArray>(reader);
			len_persos = reader.ReadUInt16();
			len_matrixIndices = reader.ReadUInt16();
			unk = reader.ReadUInt16();

			matrixIndices.Resolve(reader, r => r.length = len_matrixIndices);
			persos.Resolve(reader, p => p.length = len_persos);
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("Hierarchy Root @ " + Offset);
			if (fatherSector.Value != null) {
				GameObject fs = fatherSector.Value.GetGameObject();
				fs.name = "[Father Sector] " + fs.name;
				fs.transform.SetParent(gao.transform);
				fatherSector.Value.SetTransform(fs);
			}
			if (persos.Value != null) {
				GameObject dynGao = new GameObject("Dynamic World @ " + persos.Value.Offset);
				dynGao.transform.SetParent(gao.transform);
				foreach (SuperObjectDynamic sod in persos.Value.superObjects) {
					GameObject sodGao = sod.GetGameObject();
					sodGao.transform.SetParent(dynGao.transform);
					sod.SetTransform(sodGao);
				}
			}
			return gao;
		}
	}
}
