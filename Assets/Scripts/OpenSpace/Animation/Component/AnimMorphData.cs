using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimMorphData {

        public byte objectIndexTo;
        public byte morphProgress;
        public byte channel;
        public byte byte3;
        public byte frame;
        public byte byte5;
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
                m.channel = reader.ReadByte(); // the channel for which this morph data is relevant
                m.byte3 = reader.ReadByte();
                m.frame = reader.ReadByte(); // the frame for which this morph data is relevant
                m.byte5 = reader.ReadByte();
                m.byte6 = reader.ReadByte();
                m.byte7 = reader.ReadByte();
            } else {
                reader.ReadBytes(0x26); // Haven't deciphered this yet
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
