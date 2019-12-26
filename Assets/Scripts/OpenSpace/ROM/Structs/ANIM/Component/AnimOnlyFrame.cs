using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimOnlyFrame {
        public ushort quaternion;
        public ushort vector;
        public ushort num_hierarchies_for_frame;
        public ushort start_hierarchies_for_frame;
        public ushort unk;
        public ushort numOfNTTO;

		public AnimOnlyFrame(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			quaternion = reader.ReadUInt16();
			vector = reader.ReadUInt16();
			num_hierarchies_for_frame = reader.ReadUInt16();
			start_hierarchies_for_frame = reader.ReadUInt16();
			numOfNTTO = reader.ReadUInt16();
			unk = reader.ReadUInt16();
		}

		public static bool Aligned {
            get { return false; }
        }
    }
}
