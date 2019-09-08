using OpenSpace.Loader;
using OpenSpace.ROM.RSP;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricElementTrianglesCollideData : ROMStruct {
		public ushort length;
		public Triangle[] triangles;

		protected override void ReadInternal(Reader reader) {
			triangles = new Triangle[length];
			for (int i = 0; i < length; i++) {
				/*if (i % 2 == 0) {
					triangles[i].v2 = reader.ReadUInt16();
					triangles[i].v1 = reader.ReadUInt16();
					triangles[i].v3 = reader.ReadUInt16();
				} else {*/
				triangles[i].v1 = reader.ReadUInt16();
				triangles[i].v2 = reader.ReadUInt16();
				triangles[i].v3 = reader.ReadUInt16();
				//}
			}
		}


		public struct Triangle {
			public ushort v1;
			public ushort v2;
			public ushort v3;
		}
	}
}
