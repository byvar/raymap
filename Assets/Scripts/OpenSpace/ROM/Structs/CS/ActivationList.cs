using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ActivationList : ROMStruct {
		// Size: 4
		public Reference<ActivationZoneArray> objects;
		public ushort num_objects;

		protected override void ReadInternal(Reader reader) {
			objects = new Reference<ActivationZoneArray>(reader);
			num_objects = reader.ReadUInt16();
			objects.Resolve(reader, n => n.length = num_objects);
		}
	}
}
