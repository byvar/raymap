using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Mind : ROMStruct {
		// Size: 16
		public Reference<AIModel> aiModel;
        public Reference<Intelligence> intelligence;
        public Reference<Intelligence> reflex;
		// seems to be unused:
		public ushort word6; // always FFFF
		public ushort word8; // same
		public ushort wordA; // always 0000
		public ushort wordC; // same
		public ushort wordE; // same

		protected override void ReadInternal(Reader reader) {
			aiModel = new Reference<AIModel>(reader, true);
			intelligence = new Reference<Intelligence>(reader, true);
			reflex = new Reference<Intelligence>(reader, true);
			word6 = reader.ReadUInt16();
			word8 = reader.ReadUInt16();
			wordA = reader.ReadUInt16();
			wordC = reader.ReadUInt16();
			wordE = reader.ReadUInt16();
		}
	}
}
