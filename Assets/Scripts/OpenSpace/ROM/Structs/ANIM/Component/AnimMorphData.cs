using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimMorphData {
        public byte objectIndexTo;
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

		public AnimMorphData(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			objectIndexTo = reader.ReadByte(); // object index to morph to
			morphProgress = reader.ReadByte(); // 0-100, at 100 the morph is over.
			channel = reader.ReadInt16(); // the channel for which this morph data is relevant
			frame = reader.ReadUInt16(); // the frame for which this morph data is relevant
			byte6 = reader.ReadByte();
			byte7 = reader.ReadByte();
		}

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
