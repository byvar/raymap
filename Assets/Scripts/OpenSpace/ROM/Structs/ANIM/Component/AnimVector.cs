using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimVector {
        public Vector3 vector;

		public AnimVector(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			if (Settings.s.platform == Settings.Platform.N64) {
				float x = reader.ReadSingle();
				float z = reader.ReadSingle();
				float y = reader.ReadSingle();
				vector = new Vector3(x,y,z);
			} else {
				int x = reader.ReadInt32();
				int z = reader.ReadInt32();
				int y = reader.ReadInt32();
				vector = new Vector3(x / 4096f, y / 4096f, z / 4096f);
			}
		}

		// Size: 12

        public static bool Aligned {
            get { return true; }
        }
    }
}
