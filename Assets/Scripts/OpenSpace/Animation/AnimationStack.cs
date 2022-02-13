using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationStack {
        public LegacyPointer off_data = null;
        public uint count;
        public uint reservedMemory;
        public uint countInFix;

        public AnimationStack() {}

        public static AnimationStack Read(Reader reader) {
            AnimationStack stack = new AnimationStack();
            reader.ReadUInt32();
            stack.count = reader.ReadUInt32();
            stack.reservedMemory = reader.ReadUInt32();
            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.R3
				&& CPA_Settings.s.game != CPA_Settings.Game.Dinosaur) reader.ReadUInt32();
            stack.countInFix = reader.ReadUInt32();
            return stack;
        }

        public uint Count(bool append = false) {
            if (append) {
                return count - countInFix;
            } else return count;
        }
    }
}
