using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricObjectArray : ROMStruct {
		public Reference<GeometricObject>[] objects;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			objects = new Reference<GeometricObject>[length];
			for (int i = 0; i < objects.Length; i++) {
				objects[i] = new Reference<GeometricObject>(reader, true);
			}
        }
    }
}
