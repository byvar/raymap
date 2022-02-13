using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Type = OpenSpace.Object.SuperObject.Type;

namespace OpenSpace.PS1 {
	public class SkinnableGeometricObjectList : OpenSpaceStruct {
		public LegacyPointer off_entries;
		public LegacyPointer off_skin_memory;
		public uint sz_skin_memory;

		// Parsed
		public PointerList<GeometricObject> geometricObjects;
		//public GeometricObject[] skinnedGeometricObjects;
		public uint length;

		protected override void ReadInternal(Reader reader) {
			off_entries = LegacyPointer.Read(reader);
			off_skin_memory = LegacyPointer.Read(reader);
			sz_skin_memory = reader.ReadUInt32();

			geometricObjects = Load.FromOffsetOrRead<PointerList<GeometricObject>>(reader, off_entries, onPreRead: l => l.length = length);
			
			//skinnedGeometricObjects = Load.ReadArray<GeometricObject>(length, reader, off_geometric_skin);
		}
	}
}
