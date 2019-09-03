using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class CompressedVector3Array : ROMStruct {
		public CompressedVector3[] vectors;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			vectors = new CompressedVector3[length];
			for (int i = 0; i < vectors.Length; i++) {
				vectors[i].x = reader.ReadInt16();
				vectors[i].y = reader.ReadInt16();
				vectors[i].z = reader.ReadInt16();
			}
        }

		public struct CompressedVector3 {
			public short x;
			public short y;
			public short z;
		}

		public Vector3[] GetVectors(float factor) {
			return vectors.Select(v => new Vector3(v.y / factor, v.x / factor, v.z / factor)).ToArray();
		}
    }
}
