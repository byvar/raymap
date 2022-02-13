using System.Collections.Generic;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.PS1 {
	public class ActivationZone : OpenSpaceStruct {
		public uint num_activations;
		public LegacyPointer off_activations;

		// Parsed
		public uint[] activations;

		protected override void ReadInternal(Reader reader) {
			num_activations = reader.ReadUInt32();
			off_activations = LegacyPointer.Read(reader);

			activations = new uint[num_activations];
			if (num_activations > 0) {
				LegacyPointer.DoAt(ref reader, off_activations, () => {
					for (int i = 0; i < num_activations; i++) {
						activations[i] = reader.ReadUInt32();
					}
				});
			}
		}
	}
}
