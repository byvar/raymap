using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimHierarchy {
        public short childChannelID;
        public short parentChannelID;

		public AnimHierarchy(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			childChannelID = reader.ReadInt16();
			parentChannelID = reader.ReadInt16();
		}

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
