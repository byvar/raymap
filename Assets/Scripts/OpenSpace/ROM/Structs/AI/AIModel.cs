using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class AIModel : ROMStruct {

        // size: 16

        public ushort numBehaviors;
        public ushort numReflexes;
        public short macros;
        public short flags;
        public ushort dsgVar;

		protected override void ReadInternal(Reader reader) {
            reader.ReadUInt16(); // 0x00
            reader.ReadUInt16(); // 0x02
            reader.ReadUInt16(); // 0x04
            reader.ReadUInt16(); // 0x06
            reader.ReadUInt16(); // 0x08
            reader.ReadUInt16(); // 0x0A
            numBehaviors = reader.ReadUInt16(); // 0x0C
            numReflexes = reader.ReadUInt16(); // 0x0E
            reader.ReadUInt16(); // 0x10
            reader.ReadUInt16(); // 0x12
            reader.ReadUInt16(); // 0x14
            reader.ReadUInt16(); // 0x16
		}
	}
}
