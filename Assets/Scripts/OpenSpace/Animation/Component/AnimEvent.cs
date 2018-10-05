using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimEvent {
        public uint unk0;
        public ushort unk4;
        public ushort unk6;
        public ushort unk8;
        public ushort unkA;

        public AnimEvent() {}

        public static AnimEvent Read(Reader reader) {
            AnimEvent e = new AnimEvent();
            if (Settings.s.platform != Settings.Platform.DC) {
                e.unk0 = reader.ReadUInt32();
                e.unk4 = reader.ReadUInt16();
            }
            e.unk6 = reader.ReadUInt16();
            e.unk8 = reader.ReadUInt16();
            e.unkA = reader.ReadUInt16();
            return e;
        }

        public static int Size {
            get {
                if (Settings.s.platform == Settings.Platform.DC) {
                    return 6;
                }
                return 12;
            }
        }

        public static bool Aligned {
            get { return true; }
        }
    }
}
