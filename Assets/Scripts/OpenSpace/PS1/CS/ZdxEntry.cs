using System.Collections.Generic;
using UnityEngine;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.PS1 {
	public class ZdxEntry : OpenSpaceStruct {
		public uint num_spheres;
		public LegacyPointer off_spheres; // 0x10 large
		public uint num_boxes;
		public LegacyPointer off_boxes; // 0x14 large
		public uint unk;
		public LegacyPointer off_unk; // 0x3c large

		// Parsed
		public ZdxSphere[] spheres;
		public ZdxBox[] boxes;

		protected override void ReadInternal(Reader reader) {
			num_spheres = reader.ReadUInt32();
			off_spheres = LegacyPointer.Read(reader);
			num_boxes = reader.ReadUInt32();
			off_boxes = LegacyPointer.Read(reader);
			unk = reader.ReadUInt32();
			off_unk = LegacyPointer.Read(reader);

			spheres = Load.ReadArray<ZdxSphere>(num_spheres, reader, off_spheres);
			boxes = Load.ReadArray<ZdxBox>(num_boxes, reader, off_boxes);
		}

		public GameObject GetGameObject(CollideType collideType = CollideType.None) {
			GameObject gao = new GameObject("ZdxEntry @ " + Offset);
			gao.layer = LayerMask.NameToLayer("Collide");
			if (spheres != null) {
				for(int i = 0; i < spheres.Length; i++) {
					spheres[i].GetGameObject(gao, i, collideType: collideType);
				}
			}
			if (boxes != null) {
				for (int i = 0; i < boxes.Length; i++) {
					boxes[i].GetGameObject(gao, i, collideType: collideType);
				}
			}
			return gao;
		}
	}
}
