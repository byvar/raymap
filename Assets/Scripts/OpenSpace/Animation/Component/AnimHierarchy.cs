using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimHierarchy {
        public short childChannelID;
        public short parentChannelID;

        public AnimHierarchy() {}

        public static AnimHierarchy Read(Reader reader) {
            AnimHierarchy h = new AnimHierarchy();
            h.childChannelID = reader.ReadInt16();
            h.parentChannelID = reader.ReadInt16();
            return h;
        }
    }
}
