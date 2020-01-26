using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Brain : ROMStruct {
		// Size: 8
        public Reference<AIModel> aiModel;
		public Reference<Comport> comportIntelligence;
		public Reference<Comport> comportReflex;
		public Reference<DsgMem> dsgMem;

		protected override void ReadInternal(Reader reader) {
			aiModel = new Reference<AIModel>(reader, true);
			comportIntelligence = new Reference<Comport>(reader, true);
			comportReflex = new Reference<Comport>(reader, true);
			dsgMem = new Reference<DsgMem>(reader, true);
		}
	}
}
