using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimChannel {
        public ushort unk0;
        public short id;
        public ushort vector;
        public ushort numOfNTTO;
        public uint framesKF;
        public uint keyframe;

        public AnimChannel() {}

        public static AnimChannel Read(Reader reader) {
            AnimChannel ch = new AnimChannel();
            ch.unk0 = reader.ReadUInt16();
            ch.id = reader.ReadInt16();
            ch.vector = reader.ReadUInt16();
            ch.numOfNTTO = reader.ReadUInt16();
            if (Settings.s.engineVersion > Settings.EngineVersion.TT && Settings.s.game != Settings.Game.R2Revolution) {
                ch.framesKF = reader.ReadUInt32();
                ch.keyframe = reader.ReadUInt32();
            } else {
                ch.framesKF = reader.ReadUInt16();
                ch.keyframe = reader.ReadUInt16();
            }
            return ch;
        }

        public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.TT: return 12;
                    default: return 16;
                }
            }
        }

        public static bool Aligned {
            get {
                if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
