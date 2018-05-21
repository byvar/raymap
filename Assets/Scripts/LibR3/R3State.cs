using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3State {
        public R3Pointer offset;
        public R3Family family;
        public string name = null;
        public R3Pointer off_state_next;
        public R3Pointer off_state_prev;
        public R3Pointer off_anim_ref;
        public R3Pointer off_state_transitions;
        public R3Pointer off_state_auto; // Go to this state after a while if nothing changes
        public R3AnimationReference anim_ref;

        public R3State(R3Pointer offset, R3Family family) {
            this.offset = offset;
            this.family = family;
        }

        public static R3State Read(EndianBinaryReader reader, R3Pointer offset, R3Family family) {
            R3Loader l = R3Loader.Loader;
            R3State s = new R3State(offset, family);
            if (l.mode == R3Loader.Mode.Rayman3GC) s.name = new string(reader.ReadChars(0x50));
            if (l.mode != R3Loader.Mode.RaymanArenaGC) s.off_state_next = R3Pointer.Read(reader);
            if (l.mode == R3Loader.Mode.Rayman3GC) {
                s.off_state_prev = R3Pointer.Read(reader);
                R3Pointer.Read(reader); // unknown offset, end of state array?
            }
            s.off_anim_ref = R3Pointer.Read(reader);
            s.off_state_transitions = R3Pointer.Read(reader);
            if (l.mode != R3Loader.Mode.RaymanArenaGC) R3Pointer.Read(reader); // same?
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (l.mode != R3Loader.Mode.RaymanArenaGC) reader.ReadUInt32();
            s.off_state_auto = R3Pointer.Read(reader);
            R3Pointer.Read(reader); // fam end?
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();

            if (s.off_anim_ref != null) {
                R3Pointer.Goto(ref reader, s.off_anim_ref);
                s.anim_ref = R3AnimationReference.Read(reader, s.off_anim_ref);
            }

            return s;
        }

        public static R3State FromOffset(R3Family f, R3Pointer offset) {
            if (f == null || offset == null) return null;
            return f.states.FirstOrDefault(s => s.offset == offset);
        }
    }
}
