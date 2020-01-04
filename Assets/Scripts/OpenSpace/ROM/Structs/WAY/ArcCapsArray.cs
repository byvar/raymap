using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ArcCapsArray : ROMStruct {
		public uint[] caps;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			caps = new uint[length];
			for (int i = 0; i < caps.Length; i++) {
				caps[i] = reader.ReadUInt32();
			}
        }
    }
}
