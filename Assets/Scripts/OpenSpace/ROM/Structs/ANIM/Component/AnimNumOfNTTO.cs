using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimNumOfNTTO {
        public ushort numOfNTTO;

		public AnimNumOfNTTO(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			numOfNTTO = reader.ReadUInt16();
		}

        public static bool Aligned {
            get { return false; }
        }
    }
}
