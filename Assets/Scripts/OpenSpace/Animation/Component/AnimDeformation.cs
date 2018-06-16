using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimDeformation {
        public ushort channel;
        public ushort bone;
        public ushort linkChannel; // channel that is controlling/controlled by this channel
        public ushort linkBone; // controlled/controlling bone

        public AnimDeformation() {}

        public static AnimDeformation Read(EndianBinaryReader reader) {
            AnimDeformation d = new AnimDeformation();
            d.channel = reader.ReadUInt16();
            d.bone = reader.ReadUInt16();
            d.linkChannel = reader.ReadUInt16();
            d.linkBone = reader.ReadUInt16();
            return d;
        }
    }
}
