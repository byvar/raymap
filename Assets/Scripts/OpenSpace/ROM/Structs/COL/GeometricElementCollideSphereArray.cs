using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricElementCollideSphereArray : ROMStruct {
		// Size: 8 * length
		public GeometricElementCollideSphere[] spheres;

		public ushort length;

		protected override void ReadInternal(Reader reader) {
			spheres = new GeometricElementCollideSphere[length];
			for (ushort i = 0; i < length; i++) {
				spheres[i].radius = reader.ReadSingle();
				spheres[i].material = new Reference<GameMaterial>(reader, true);
				spheres[i].centerPoint = reader.ReadUInt16();
			}
		}

		public struct GeometricElementCollideSphere {
			// Size: 8
			public float radius;
			public Reference<GameMaterial> material;
			public ushort centerPoint;
		}
	}
}
