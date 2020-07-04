using UnityEngine;
using UnityEditor;

namespace OpenSpace.Visual.ISI {
    public class RadiosityLOD : OpenSpaceStruct {
		public uint num_colors;
		public Pointer off_colors;

		// Parsed
		public ColorISI[] colors;

		protected override void ReadInternal(Reader reader) {
			num_colors = reader.ReadUInt32();
			off_colors = Pointer.Read(reader);
			Load.print(Offset + " - " + off_colors);

			colors = Load.ReadArray<ColorISI>(num_colors, reader, off_colors);
		}
	}
}