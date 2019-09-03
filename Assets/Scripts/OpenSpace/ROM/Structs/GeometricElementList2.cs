using OpenSpace.Loader;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricElementList2 : ROMStruct {
		public GeometricElementListEntry[] elements;

		public ushort length;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			elements = new GeometricElementListEntry[length];
			for (ushort i = 0; i < length; i++) {
				elements[i].element = new GenericReference(reader, true);
				reader.ReadUInt16();
				reader.ReadUInt16();
			}
        }

		public struct GeometricElementListEntry {
			public GenericReference element;
		}
    }
}
