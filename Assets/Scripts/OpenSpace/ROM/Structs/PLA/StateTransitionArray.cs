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
				transitions[i].targetState = new Reference<State>(reader, true);
				transitions[i].stateToGo = new Reference<State>(reader, true);
				transitions[i].linkingType = reader.ReadUInt16();
			}
		}

		public struct StateTransition {
			public Reference<State> targetState;
			public Reference<State> stateToGo;
			public ushort linkingType;
		}
	}
}
