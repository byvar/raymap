using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SectorSuperObjectArray1 : ROMStruct {
		public Reference<SuperObject>[] superObjects;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			superObjects = new Reference<SuperObject>[length];
			for (int i = 0; i < superObjects.Length; i++) {
				superObjects[i] = new Reference<SuperObject>(reader, true);
			}
        }
    }
}
