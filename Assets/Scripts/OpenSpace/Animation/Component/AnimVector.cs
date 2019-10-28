using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimVector : OpenSpaceStruct {
        public Vector3 vector;

		protected override void ReadInternal(Reader reader) {
			float x = reader.ReadSingle();
			float z = reader.ReadSingle();
			float y = reader.ReadSingle();
			vector = new Vector3(x, y, z);
		}

		// Size: 12

        public static bool Aligned {
            get { return true; }
        }
    }
}
