using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ActivationZone : ROMStruct {
		// Size: 4
		public Reference<ActivationIndexArray> objects;
		public ushort num_objects;

		protected override void ReadInternal(Reader reader) {
			objects = new Reference<ActivationIndexArray>(reader);
			num_objects = reader.ReadUInt16();
			objects.Resolve(reader, n => n.length = num_objects);
		}
	}
}
