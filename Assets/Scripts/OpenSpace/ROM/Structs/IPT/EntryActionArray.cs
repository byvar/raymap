using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class EntryActionArray : ROMStruct {
		public Reference<EntryAction>[] entryActions;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			entryActions = new Reference<EntryAction>[length];
			for (int i = 0; i < entryActions.Length; i++) {
				entryActions[i] = new Reference<EntryAction>(reader, true);
			}
        }
    }
}
