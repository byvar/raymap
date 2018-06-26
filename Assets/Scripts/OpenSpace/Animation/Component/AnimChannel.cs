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
        public short numOfNTTO;
        public uint framesKF;
        public uint keyframe;

        public AnimChannel() {}

        public static AnimChannel Read(EndianBinaryReader reader) {
            AnimChannel ch = new AnimChannel();
            ch.unk0 = reader.ReadUInt16();
            ch.id = reader.ReadInt16();
            ch.vector = reader.ReadUInt16();
            ch.numOfNTTO = reader.ReadInt16();
            ch.framesKF = reader.ReadUInt32();
            ch.keyframe = reader.ReadUInt32();
            return ch;
        }
    }
}
