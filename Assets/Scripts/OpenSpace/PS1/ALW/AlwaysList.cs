using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Type = OpenSpace.Object.SuperObject.Type;

namespace OpenSpace.PS1 {
	public class AlwaysList : OpenSpaceStruct {
		public uint index;
		public uint length;
		public Pointer off_items;
		public uint invalidPointer; // pointer in exe?

		public AlwaysItem[] items;

		protected override void ReadInternal(Reader reader) {
			index = reader.ReadUInt32();
			length = reader.ReadUInt32();
			off_items = Pointer.Read(reader);
			invalidPointer = reader.ReadUInt32();

			items = Load.ReadArray<AlwaysItem>(length, reader, off_items);
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("Always List " + index);
			for(int i = 0; i < items.Length; i++) {
				GameObject g = items[i].GetGameObject();
				if (g != null) {
					g.name = $"({i}) {g.name}";
					g.transform.SetParent(gao.transform);
					g.transform.localPosition = new Vector3(0f, 0f, i * 5f);
					g.transform.localRotation = Quaternion.identity;
					g.transform.localScale = Vector3.one;
				}
			}
			return gao;
		}
	}
}
