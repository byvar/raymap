using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class PhysicalCollSet : ROMStruct {
		public Reference<GeometricObject> mesh;
		public ushort type;

        protected override void ReadInternal(Reader reader) {
			mesh = new Reference<GeometricObject>(reader, true);
			type = reader.ReadUInt16(); // Can be either 1,2,3,4. Probably ZDX
        }
	}
}
