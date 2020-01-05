using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ZdxList : ROMStruct {
		// Size: 4
		public Reference<GeometricObjectArray> objects;
		public ushort num_objects;

		protected override void ReadInternal(Reader reader) {
			objects = new Reference<GeometricObjectArray>(reader);
			num_objects = reader.ReadUInt16();
			objects.Resolve(reader, n => n.length = num_objects);
		}
	}
}
