using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimHierarchy : OpenSpaceStruct {
        public short childChannelID;
        public short parentChannelID;

		protected override void ReadInternal(Reader reader) {
			if (Settings.s.game == Settings.Game.LargoWinch) {
				childChannelID = reader.ReadByte();
				parentChannelID = reader.ReadByte();
			} else {
				childChannelID = reader.ReadInt16();
				parentChannelID = reader.ReadInt16();
			}
		}

		/*public static int Size {
            get { return 4; }
        }*/

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
