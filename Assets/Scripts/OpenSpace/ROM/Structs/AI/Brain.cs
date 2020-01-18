using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Brain : ROMStruct {
		// Size: 8
        public Reference<AIModel> mind;
		public Reference<Comport> comportIntelligence;
		public Reference<Comport> comportReflex;
		public ushort ref_107; // dsgMem

		protected override void ReadInternal(Reader reader) {
			mind = new Reference<AIModel>(reader, true);
			comportIntelligence = new Reference<Comport>(reader, true);
			comportReflex = new Reference<Comport>(reader, true);
			ref_107 = reader.ReadUInt16();
		}
	}
}
