using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimNumOfNTTO {
        public ushort numOfNTTO;

        public AnimNumOfNTTO() {}

        public static AnimNumOfNTTO Read(Reader reader) {
            AnimNumOfNTTO n = new AnimNumOfNTTO();
            n.numOfNTTO = reader.ReadUInt16();
            return n;
        }
    }
}
