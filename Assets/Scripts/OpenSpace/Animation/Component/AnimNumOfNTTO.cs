using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimNumOfNTTO : OpenSpaceStruct {
        public ushort numOfNTTO;

		protected override void ReadInternal(Reader reader) {
			numOfNTTO = reader.ReadUInt16();
		}

		/*public static int Size {
            get { return 2; }
        }*/

        public static bool Aligned {
            get { return false; }
        }
    }
}
