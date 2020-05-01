using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class StateTransition : OpenSpaceStruct {
		// Size: 8 * length
		public Pointer<State> targetState;
		public Pointer<State> stateToGo;

		protected override void ReadInternal(Reader reader) {
			targetState = new Pointer<State>(reader, true);
			stateToGo = new Pointer<State>(reader, true);
		}
	}
}
