using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimChannel {
        public ushort num_keyframes;
        public short id;
        public ushort vector;
		public ushort unk;

		public AnimChannel(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			num_keyframes = reader.ReadUInt16();
			id = reader.ReadInt16();
			vector = reader.ReadUInt16();
			unk = reader.ReadUInt16();
		}

        public static bool Aligned {
            get {
                if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
