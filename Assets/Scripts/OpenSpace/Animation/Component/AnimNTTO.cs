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
        public static ushort flag_isInvisible = 0x2;

        public static AnimNTTO Read(Reader reader) {
            AnimNTTO n = new AnimNTTO();
            n.flags = reader.ReadUInt16();
            n.object_index = reader.ReadUInt16();
            n.unk4 = reader.ReadByte();
            n.unk5 = reader.ReadByte();
            return n;
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
    }
}
