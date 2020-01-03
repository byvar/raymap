using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Comport : ROMStruct {
		// Size: 6
		public Reference<Script> firstScript;
		public Reference<ScriptArray> scripts;
		public ushort num_scripts;

        protected override void ReadInternal(Reader reader) {
			firstScript = new Reference<Script>(reader, true); 
			scripts = new Reference<ScriptArray>(reader);
			num_scripts = reader.ReadUInt16();

			scripts.Resolve(reader, s => s.length = num_scripts);
        }
	}
}