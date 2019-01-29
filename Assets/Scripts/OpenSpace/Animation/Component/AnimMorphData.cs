using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimMorphData {

        public ushort objectIndexTo;
        public byte morphProgress;
		public short channel;
        public ushort frame;
        public byte byte6;
        public byte byte7;
        public float morphProgressFloat
        {
            get
            {
                return ((float)morphProgress) / 100.0f;
            }
        }

        public AnimMorphData() {}

        public static AnimMorphData Read(Reader reader) {
            AnimMorphData m = new AnimMorphData();
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                m.objectIndexTo = reader.ReadByte(); // object index to morph to
                m.morphProgress = reader.ReadByte(); // 0-100, at 100 the morph is over.
                m.channel = reader.ReadInt16(); // the channel for which this morph data is relevant
                m.frame = reader.ReadUInt16(); // the frame for which this morph data is relevant
                m.byte6 = reader.ReadByte();
                m.byte7 = reader.ReadByte();
            } else {
				m.channel = reader.ReadInt16(); // the channel for which this morph data is relevant
				m.frame = reader.ReadUInt16(); // the frame for which this morph data is relevant
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				m.objectIndexTo = reader.ReadUInt16(); // 5
				reader.ReadUInt16();
				reader.ReadBytes(0x10); // Haven't deciphered this yet
				m.morphProgress = reader.ReadByte();
				reader.ReadBytes(0x9);
			}
            return m;
        }

        public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.R3: return 0x26;
                    default: return 0x8;
                }
            }
        }

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
