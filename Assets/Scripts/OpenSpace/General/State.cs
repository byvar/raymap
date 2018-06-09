using OpenSpace.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class State {
        public Pointer offset;
        public Family family;
        public string name = "No name";
        public Pointer off_state_next;
        public Pointer off_state_prev;
        public Pointer off_anim_ref;
        public Pointer off_state_transitions_first;
        public Pointer off_state_transitions_last;
        public uint num_state_transitions;
        public byte speed;
        public Pointer off_state_auto; // Go to this state after a while if nothing changes
        public AnimationReference anim_ref;

        public State(Pointer offset, Family family) {
            this.offset = offset;
            this.family = family;
        }

        public static State Read(EndianBinaryReader reader, Pointer offset, Family family) {
            MapLoader l = MapLoader.Loader;
            State s = new State(offset, family);
            if (l.mode == MapLoader.Mode.Rayman3GC) s.name = new string(reader.ReadChars(0x50)).TrimEnd('\0');
            if (l.mode != MapLoader.Mode.RaymanArenaGC) s.off_state_next = Pointer.Read(reader);
            if (l.mode == MapLoader.Mode.Rayman3GC) {
                s.off_state_prev = Pointer.Read(reader);
                Pointer.Read(reader); // unknown offset, end of state array?
            }
            s.off_anim_ref = Pointer.Read(reader);
            s.off_state_transitions_first = Pointer.Read(reader);
            if (l.mode != MapLoader.Mode.RaymanArenaGC) s.off_state_transitions_last = Pointer.Read(reader); // same?
            s.num_state_transitions = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (l.mode != MapLoader.Mode.RaymanArenaGC) reader.ReadUInt32();
            s.off_state_auto = Pointer.Read(reader);
            Pointer.Read(reader); // fam end?
            if (l.mode != MapLoader.Mode.Rayman2PC) {
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            reader.ReadByte();
            s.speed = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            if (l.mode == MapLoader.Mode.Rayman2PC) reader.ReadUInt32();

            if (s.off_anim_ref != null) {
                Pointer.Goto(ref reader, s.off_anim_ref);
                s.anim_ref = AnimationReference.Read(reader, s.off_anim_ref);
            }

            return s;
        }

        public static State FromOffset(Family f, Pointer offset) {
            if (f == null || offset == null) return null;
            return f.states.FirstOrDefault(s => s.offset == offset);
        }
    }
}
