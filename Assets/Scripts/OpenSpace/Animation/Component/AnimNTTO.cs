using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimNTTO : OpenSpaceStruct {
        public ushort flags;
        public ushort object_index;
        public byte unk4;
        public byte unk5;

        public static ushort flag_isBoneNTTO = 0x00FF;
        public static ushort flag_isInvisible = 0x2;

		protected override void ReadInternal(Reader reader) {
			if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
				object_index = reader.ReadUInt16();
				flags = reader.ReadUInt16();
			} else {
				flags = reader.ReadUInt16();
				object_index = reader.ReadUInt16();
			}
			unk4 = reader.ReadByte();
			unk5 = reader.ReadByte();
		}

		public bool IsInvisibleNTTO {
            get {
                if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
                    return (flags & flag_isBoneNTTO) != 0;
                } else {
                    return (flags & flag_isInvisible) == flag_isInvisible;
                }
            }
        }

        /*public static int Size {
            get { return 6; }
        }*/

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
