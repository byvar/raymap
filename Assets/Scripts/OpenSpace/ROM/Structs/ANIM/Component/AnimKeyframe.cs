using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimKeyframe {
        public ushort frame;
        public ushort flags;
        public ushort quaternion;
        public ushort quaternion2;
        public ushort scaleVector;
        public ushort positionVector;
		public ushort unk;

		public AnimKeyframe(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			frame = reader.ReadUInt16();
			flags = reader.ReadUInt16();
			quaternion = reader.ReadUInt16();
			quaternion2 = reader.ReadUInt16();
			scaleVector = reader.ReadUInt16();
			positionVector = reader.ReadUInt16();
			unk = reader.ReadUInt16();
		}

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
