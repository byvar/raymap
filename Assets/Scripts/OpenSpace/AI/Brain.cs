using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Brain {
        public LegacyPointer offset;

        public LegacyPointer off_mind;
        public uint unknown1;
        public uint unknown2;
        
        public Mind mind;

        public Brain(LegacyPointer offset) {
            this.offset = offset;
        }

        public static Brain Read(Reader reader, LegacyPointer offset) {
            Brain b = new Brain(offset);
			//MapLoader.Loader.print("Brain " + offset);
            b.off_mind = LegacyPointer.Read(reader);
			if (Legacy_Settings.s.game != Legacy_Settings.Game.R2Revolution) b.unknown1 = reader.ReadUInt32(); // init at 0xCDCDCDCD
			if (Legacy_Settings.s.game == Legacy_Settings.Game.LargoWinch) {
				b.unknown2 = reader.ReadByte(); // 0
			} else {
				b.unknown2 = reader.ReadUInt32(); // 0
			}

            b.mind = MapLoader.Loader.FromOffsetOrRead<Mind>(reader, b.off_mind);

            return b;
        }
    }
}
