using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.ComponentLargo {
    public class AnimFrameVector : OpenSpaceStruct {
		public ushort frame;
		public ushort vector;

		protected override void ReadInternal(Reader reader) {
			frame = reader.ReadUInt16();
			vector = reader.ReadUInt16();
		}

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
