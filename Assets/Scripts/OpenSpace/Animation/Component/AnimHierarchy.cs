using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimHierarchy {
        public ushort childChannelID;
        public ushort parentChannelID;

        public AnimHierarchy() {}

        public static AnimHierarchy Read(EndianBinaryReader reader) {
            AnimHierarchy h = new AnimHierarchy();
            h.childChannelID = reader.ReadUInt16();
            h.parentChannelID = reader.ReadUInt16();
            return h;
        }
    }
}
