using System.Collections.Generic;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.PS1 {
	public class ZdxList : OpenSpaceStruct {
		public uint num_entries;
		public Pointer off_entries;

		// Parsed
		public ZdxEntry[] entries;

		protected override void ReadInternal(Reader reader) {
			num_entries = reader.ReadUInt32();
			off_entries = Pointer.Read(reader);

			entries = Load.ReadArray<ZdxEntry>(num_entries, reader, off_entries);
		}
	}
}
