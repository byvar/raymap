using System.Collections.Generic;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.PS1 {
	public class ActivationList : OpenSpaceStruct {
		public uint num_activationZones;
		public Pointer off_activations;

		// Parsed
		public ActivationZone[] activationZones;

		protected override void ReadInternal(Reader reader) {
			num_activationZones = reader.ReadUInt32();
			off_activations = Pointer.Read(reader);

			activationZones = Load.ReadArray<ActivationZone>(num_activationZones, reader, off_activations);
		}
	}
}
