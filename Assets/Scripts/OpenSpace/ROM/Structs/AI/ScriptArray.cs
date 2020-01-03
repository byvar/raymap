using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ScriptArray : ROMStruct {
		public Reference<Script>[] scripts;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			scripts = new Reference<Script>[length];
			for (int i = 0; i < scripts.Length; i++) {
				scripts[i] = new Reference<Script>(reader, true);
			}
        }
    }
}
