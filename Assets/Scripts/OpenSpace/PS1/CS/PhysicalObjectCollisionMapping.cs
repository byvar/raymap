using System.Collections.Generic;

namespace OpenSpace.PS1 {
	public class PhysicalObjectCollisionMapping : OpenSpaceStruct {
		public Pointer off_collision;
		public Pointer off_poListEntry;

		// Parsed
		public Pointer off_geo_collide;
		public GeometricObjectCollide geo_collide;

		protected override void ReadInternal(Reader reader) {
			off_collision = Pointer.Read(reader);
			off_poListEntry = Pointer.Read(reader);

			if (off_collision != null) {
				Pointer.DoAt(ref reader, off_collision + 0x10, () => {
					uint num_collision = reader.ReadUInt32();
					if (num_collision > 0) {
						off_geo_collide = Pointer.Read(reader);
						geo_collide = Load.FromOffsetOrRead<GeometricObjectCollide>(reader, off_geo_collide);
					}
				});
			}
		}
	}
}
