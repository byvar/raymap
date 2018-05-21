using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3AnimationStack {
        public R3Pointer off_data;
        public uint count;

        public R3AnimationStack() {}

        public static R3AnimationStack Read(EndianBinaryReader reader) {
            R3AnimationStack stack = new R3AnimationStack();
            reader.ReadUInt32();
            stack.count = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            return stack;
        }
    }
}
