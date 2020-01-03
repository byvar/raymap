using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Brain : ROMStruct {
		// Size: 8
		public uint dword0;
		public uint word4;
        public Reference<Mind> mind;

		protected override void ReadInternal(Reader reader) {
			dword0 = reader.ReadUInt32();
			word4 = reader.ReadUInt16();
            mind = new Reference<Mind>(reader, true);
		}
	}
}
