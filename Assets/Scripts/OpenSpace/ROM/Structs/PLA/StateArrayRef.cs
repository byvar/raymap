using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class StateArrayRef : ROMStruct {
		// Size: 4
		public Reference<StateArray> states;
		public ushort len_states;

		protected override void ReadInternal(Reader reader) {
			states = new Reference<StateArray>(reader);
			len_states = reader.ReadUInt16();
			states.Resolve(reader, s => s.length = len_states);
		}
	}
}
