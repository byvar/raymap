using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Vector3Array : ROMStruct {
		public Vector3[] vectors;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			vectors = new Vector3[length];
			for (int i = 0; i < vectors.Length; i++) {
				vectors[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			}
        }

		public Vector3[] GetVectors(bool switchAxes = true) {
			if (!switchAxes) {
				return vectors;
			} else {
				return vectors.Select(v => new Vector3(v.x, v.z, v.y)).ToArray();
			}
		}
    }
}
