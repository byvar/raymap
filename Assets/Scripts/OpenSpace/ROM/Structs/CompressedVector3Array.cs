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
				vectors[i] = new CompressedVector3(reader);
			}
        }

		public Vector3[] GetVectors(float factor, bool switchAxes = true) {
			return vectors.Select(v => v.GetVector(factor, switchAxes: switchAxes)).ToArray();
		}
    }
}
