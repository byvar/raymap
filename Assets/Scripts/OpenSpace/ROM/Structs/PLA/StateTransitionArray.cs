using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class StateTransitionArray : ROMStruct {
		// Size: 6 * length
		public StateTransition[] transitions;
		public ushort length;

		protected override void ReadInternal(Reader reader) {
			transitions = new StateTransition[length];
			for (int i = 0; i < transitions.Length; i++) {
				transitions[i] = new StateTransition();
				transitions[i].state0 = new Reference<State>(reader, true);
				transitions[i].state2 = new Reference<State>(reader, true);
				transitions[i].word4 = reader.ReadUInt16();
			}
		}

		public struct StateTransition {
			public Reference<State> state0;
			public Reference<State> state2;
			public ushort word4;
		}
	}
}
