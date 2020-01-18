using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class AIModel : ROMStruct {
		// Size: 16
		public Reference<DsgVar> dsgVar;
        public Reference<ComportList> comportsIntelligence;
        public Reference<ComportList> comportsReflex;
		// seems to be unused:
		public ushort word6; // always FFFF
		public ushort word8; // same
		public ushort wordA; // always 0000
		public ushort wordC; // same
		public ushort wordE; // same

		protected override void ReadInternal(Reader reader) {
			dsgVar = new Reference<DsgVar>(reader, true);
			comportsIntelligence = new Reference<ComportList>(reader, true);
			comportsReflex = new Reference<ComportList>(reader, true);
			word6 = reader.ReadUInt16();
			word8 = reader.ReadUInt16();
			wordA = reader.ReadUInt16();
			wordC = reader.ReadUInt16();
			wordE = reader.ReadUInt16();
		}
	}
}
