using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ActivationIndexArray : ROMStruct {
		public ushort[] objects;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			objects = new ushort[length];
			for (int i = 0; i < objects.Length; i++) {
				objects[i] = reader.ReadUInt16();
			}
        }
    }
}
