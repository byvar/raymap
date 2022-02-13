using System.Collections.Generic;

namespace OpenSpace.PS1 {
	public class PhysicalObjectCollisionMapping : OpenSpaceStruct {
		public LegacyPointer off_collision;
		public LegacyPointer off_poListEntry;

		// Parsed
		public LegacyPointer off_geo_collide;
		public GeometricObjectCollide geo_collide;

		protected override void ReadInternal(Reader reader) {
			off_collision = LegacyPointer.Read(reader);
			off_poListEntry = LegacyPointer.Read(reader);
			reader.ReadBytes(0x24);

			if (off_collision != null) {
				LegacyPointer.DoAt(ref reader, off_collision + 0x10, () => {
					uint num_collision = reader.ReadUInt32();
					if (num_collision > 0) {
						off_geo_collide = LegacyPointer.Read(reader);
						geo_collide = Load.FromOffsetOrRead<GeometricObjectCollide>(reader, off_geo_collide);
					}
				});
			}
		}
	}
}
