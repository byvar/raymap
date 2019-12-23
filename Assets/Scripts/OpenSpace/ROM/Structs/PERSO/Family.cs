using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Family : ROMStruct {
		// Size: 8
		public Reference<StateArrayRef> states;
		public Reference<ObjectsTable> objectsTable;
		public ushort ref_105; // a vector4? maybe bounding volume. check this
		public ushort word6;

		protected override void ReadInternal(Reader reader) {
			states = new Reference<StateArrayRef>(reader, true);
			objectsTable = new Reference<ObjectsTable>(reader, true);
			ref_105 = reader.ReadUInt16();
			word6 = reader.ReadUInt16();
		}
	}
}
