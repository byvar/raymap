using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.ComponentLargo {
    public class AnimChannelLargo : OpenSpaceStruct {
		public short id;
		public ushort numFrameVectors;
		public ushort numFrameQuaternions;
		public ushort numFrameUnknowns;
		public short unk;
		public ushort numOfNTTO;

		protected override void ReadInternal(Reader reader) {
			id = reader.ReadInt16();
			numFrameVectors = reader.ReadUInt16();
			numFrameQuaternions = reader.ReadUInt16();
			numFrameUnknowns = reader.ReadUInt16();
			unk = reader.ReadInt16();
			numOfNTTO = reader.ReadUInt16();
		}

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
