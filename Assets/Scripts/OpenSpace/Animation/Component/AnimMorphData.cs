using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimMorphData : OpenSpaceStruct {
        public ushort objectIndexTo;
        public byte morphProgress;
		public short channel;
        public ushort frame;
        public byte byte6;
        public byte byte7;
        public float morphProgressFloat {
            get {
                return ((float)morphProgress) / 100.0f;
            }
        }

		protected override void ReadInternal(Reader reader) {
			if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
				objectIndexTo = reader.ReadByte(); // object index to morph to
				morphProgress = reader.ReadByte(); // 0-100, at 100 the morph is over.
				channel = reader.ReadInt16(); // the channel for which this morph data is relevant
				frame = reader.ReadUInt16(); // the frame for which this morph data is relevant
				byte6 = reader.ReadByte();
				byte7 = reader.ReadByte();
			} else {
				channel = reader.ReadInt16(); // the channel for which this morph data is relevant
				frame = reader.ReadUInt16(); // the frame for which this morph data is relevant
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				objectIndexTo = reader.ReadUInt16(); // 5
				reader.ReadUInt16();
				reader.ReadBytes(0x10); // Haven't deciphered this yet
				morphProgress = reader.ReadByte();
				reader.ReadBytes(0x9);
			}
		}

		/*public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.R3: return 0x26;
                    default: return 0x8;
                }
            }
        }*/

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
