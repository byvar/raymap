using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class AIModel : ROMStruct {

		// size: 16
		public ushort word0;
		public ushort ref_72;
		public ushort ref_111_behaviors;
		public ushort ref_111_reflexes;
		public ushort num_dsgVars_2;
		public ushort num_dsgVars;
        public ushort num_behaviors;
        public ushort num_reflexes;

		protected override void ReadInternal(Reader reader) {
            word0 = reader.ReadUInt16(); // 0x00
            ref_72 = reader.ReadUInt16(); // 0x02
            ref_111_behaviors = reader.ReadUInt16(); // 0x04
            ref_111_reflexes = reader.ReadUInt16(); // 0x06
            num_dsgVars_2 = reader.ReadUInt16(); // 0x08
            num_dsgVars = reader.ReadUInt16(); // 0x0A
            num_behaviors = reader.ReadUInt16(); // 0x0C
            num_reflexes = reader.ReadUInt16(); // 0x0E
		}
	}
}
