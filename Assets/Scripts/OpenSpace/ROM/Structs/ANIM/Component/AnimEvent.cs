using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimEvent {
        public ushort unk0;
        public ushort frame;
        public ushort unk4;
        public ushort unk6;

		public AnimEvent(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			unk0 = reader.ReadUInt16();
			frame = reader.ReadUInt16();
			unk4 = reader.ReadUInt16();
			unk6 = reader.ReadUInt16();
		}

        public static bool Aligned {
            get { return true; }
        }
    }
}
