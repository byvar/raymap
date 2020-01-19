using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricElementCollideAlignedBoxArray : ROMStruct {
		// Size: 6 * length
		public GeometricElementCollideAlignedBox[] boxes;

		public ushort length;

		protected override void ReadInternal(Reader reader) {
			boxes = new GeometricElementCollideAlignedBox[length];
			for (ushort i = 0; i < length; i++) {
				boxes[i].material = new Reference<GameMaterial>(reader, true);
				boxes[i].minVertex = reader.ReadUInt16();
				boxes[i].maxVertex = reader.ReadUInt16();
			}
		}

		public struct GeometricElementCollideAlignedBox {
			// Size: 6
			public Reference<GameMaterial> material;
			public ushort minVertex;
			public ushort maxVertex;
		}
	}
}
