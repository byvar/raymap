using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SuperObjectDynamicArray : ROMStruct {
		// Size: 2
		public Reference<SuperObjectDynamic>[] superObjects;

		public ushort length;

		protected override void ReadInternal(Reader reader) {
			//Loader.print(Pointer.Current(reader) + " - " + length);
			superObjects = new Reference<SuperObjectDynamic>[length];
			for (int i = 0; i < superObjects.Length; i++) {
				superObjects[i] = new Reference<SuperObjectDynamic>(reader, true);
				//MapLoader.Loader.print(superObjects[i].index + " - " + superObjects[i].Value);
			}
		}
	}
}
