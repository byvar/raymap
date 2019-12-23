using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class StateArray : ROMStruct {
		// Size: 2 * length
		public Reference<State>[] states;
		public ushort length;

		protected override void ReadInternal(Reader reader) {
			states = new Reference<State>[length];
			for (int i = 0; i < states.Length; i++) {
				states[i] = new Reference<State>(reader, true);
			}
		}
	}
}
