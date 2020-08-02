using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimEvent : OpenSpaceStruct {
        public uint unk0;
        public ushort unk4;
        public ushort unk6;
        public ushort unk8;
        public ushort unkA;

		protected override void ReadInternal(Reader reader) {
			if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
				unk0 = reader.ReadUInt32();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
			} else {
				if (Settings.s.platform != Settings.Platform.DC && Settings.s.game != Settings.Game.Donald_BinRP) {
					if (Settings.s.platform != Settings.Platform.iOS) {
						unk0 = reader.ReadUInt32();
					}
					unk4 = reader.ReadUInt16();
				}
				unk6 = reader.ReadUInt16();
				unk8 = reader.ReadUInt16();
				unkA = reader.ReadUInt16();
			}
		}

		/*public static int Size {
            get {
				if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
					return 20; // 0x14
				} else if (Settings.s.platform == Settings.Platform.DC) {
					return 6;
				}
                return 12;
            }
        }*/

        public static bool Aligned {
            get { return true; }
        }
    }
}
