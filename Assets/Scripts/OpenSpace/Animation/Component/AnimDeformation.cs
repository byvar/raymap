using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimDeformation : OpenSpaceStruct {
        public short channel;
        public ushort bone;
        public short linkChannel; // channel that is controlling/controlled by this channel
        public ushort linkBone; // controlled/controlling bone

		protected override void ReadInternal(Reader reader) {
			if (Settings.s.game == Settings.Game.LargoWinch) {
				channel = reader.ReadByte();
				bone = reader.ReadByte();
				linkChannel = reader.ReadByte();
				linkBone = reader.ReadByte();
			} else {
				channel = reader.ReadInt16();
				bone = reader.ReadUInt16();
				linkChannel = reader.ReadInt16();
				linkBone = reader.ReadUInt16();
			}
		}

		/*public static int Size {
            get {
				if (Settings.s.game == Settings.Game.LargoWinch) {
					return 4;
				} else {
					return 8;
				}
			}
        }*/

        public static bool Aligned {
            get {
                return true;
            }
        }
    }
}
