using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Script : ROMStruct {
		// Size: 4
		public Reference<ScriptNodeArray> nodes;
		public ushort num_scriptNodes;

		protected override void ReadInternal(Reader reader) {
			nodes = new Reference<ScriptNodeArray>(reader);
			num_scriptNodes = reader.ReadUInt16();
			nodes.Resolve(reader, sna => sna.length = num_scriptNodes);
		}
	}
}
