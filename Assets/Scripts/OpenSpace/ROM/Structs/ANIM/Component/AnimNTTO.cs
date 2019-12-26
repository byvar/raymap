using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimNTTO {
        public ushort flags;
        public byte object_index;
        public byte unk;

        public static ushort flag_isInvisible = 0x2;

		public AnimNTTO(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			flags = reader.ReadUInt16();
			object_index = reader.ReadByte();
			unk = reader.ReadByte();
		}

		public bool IsInvisibleNTTO {
            get {
                 return (flags & flag_isInvisible) == flag_isInvisible;
            }
        }

        public static bool Aligned {
            get {
                return false;
            }
        }
    }
}
