using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.ComponentLargo {
    public class AnimFrameQuaternion : OpenSpaceStruct {
		public ushort frame;
		public ushort quaternion;

		protected override void ReadInternal(Reader reader) {
			frame = reader.ReadUInt16();
			quaternion = reader.ReadUInt16();
		}

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
