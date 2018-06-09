using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationStack {
        public Pointer off_data;
        public uint count;

        public AnimationStack() {}

        public static AnimationStack Read(EndianBinaryReader reader) {
            AnimationStack stack = new AnimationStack();
            reader.ReadUInt32();
            stack.count = reader.ReadUInt32();
            reader.ReadUInt32();
            if(MapLoader.Loader.mode != MapLoader.Mode.Rayman2PC) reader.ReadUInt32();
            reader.ReadUInt32();
            return stack;
        }
    }
}
