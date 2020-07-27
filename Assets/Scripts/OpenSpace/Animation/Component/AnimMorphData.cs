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
		public float GetMorphProgressFloat(int i) {
			if (Settings.s.engineVersion >= Settings.EngineVersion.R3 && i < morphProgressArray.Length) {
				return ((float)morphProgressArray[i]) / 100.0f;
			} else {
				return morphProgressFloat;
			}
		}

		// Rayman 3
		public byte numMorphs;
		public byte[] morphProgressArray;
		public ushort[] objectIndexToArray;

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
				numMorphs = reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				objectIndexToArray = new ushort[10];
				for (int i = 0; i < objectIndexToArray.Length; i++) {
					objectIndexToArray[i] = reader.ReadUInt16();
				}
				morphProgressArray = new byte[10];
				for (int i = 0; i < morphProgressArray.Length; i++) {
					morphProgressArray[i] = reader.ReadByte();
				}
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
