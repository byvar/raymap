using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class InputStruct : ROMStruct {
		public Reference<EntryActionArray> entryActions;
		public ushort num_entryActions;

        protected override void ReadInternal(Reader reader) {
			entryActions = new Reference<EntryActionArray>(reader);
			num_entryActions = reader.ReadUInt16();

			entryActions.Resolve(reader, d => d.length = num_entryActions);
		}
	}
}
