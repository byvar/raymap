using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimNTTO {
        public ushort flags;
        public ushort object_index;
        public byte unk4;
        public byte unk5;

        public AnimNTTO() {}

        public static ushort flag_isBoneNTTO = 0x00FF;

        public static AnimNTTO Read(EndianBinaryReader reader) {
            AnimNTTO n = new AnimNTTO();
            n.flags = reader.ReadUInt16();
            n.object_index = reader.ReadUInt16();
            n.unk4 = reader.ReadByte();
            n.unk5 = reader.ReadByte();
            return n;
        }

        public bool IsBoneNTTO {
            get {
                //return flags != 0;
                return (flags & flag_isBoneNTTO) != 0;
                //return flags != 0;
            }
        }
    }
}
