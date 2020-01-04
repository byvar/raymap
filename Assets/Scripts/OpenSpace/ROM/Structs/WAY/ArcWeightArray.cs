using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ArcWeightArray : ROMStruct {
		public ushort[] weights;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			weights = new ushort[length];
			for (int i = 0; i < weights.Length; i++) {
				weights[i] = reader.ReadUInt16();
			}
        }
    }
}
