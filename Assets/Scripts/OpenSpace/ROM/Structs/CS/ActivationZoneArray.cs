using OpenSpace.Loader;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ActivationZoneArray : ROMStruct {
		public ActivationZoneArrayEntry[] elements;

		public ushort length;

        protected override void ReadInternal(Reader reader) {
			elements = new ActivationZoneArrayEntry[length];
			for (ushort i = 0; i < length; i++) {
				elements[i].state = new Reference<State>(reader, true);
				elements[i].activationList = new Reference<ActivationZone>(reader, true);
			}
        }

		public struct ActivationZoneArrayEntry {
			public Reference<State> state;
			public Reference<ActivationZone> activationList;
		}
    }
}
