using OpenSpace.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class State : ILinkedListEntry {
        public Pointer offset;
        public Family family;
        public string name = null;
        public Pointer off_state_next;
        public Pointer off_state_prev;
        public Pointer off_anim_ref;
        public LinkedList<int> stateTransitions;
        public Pointer off_cine_mapname = null;
        public Pointer off_cine_name = null;
        public string cine_mapname = null;
        public string cine_name = null;
        public byte speed;
        public Pointer off_state_auto; // Go to this state after a while if nothing changes
        public AnimationReference anim_ref;

        public Pointer NextEntry {
            get {
                if (MapLoader.Loader.mode == MapLoader.Mode.RaymanArenaGC) {
                    return offset + 0x28;
                } else {
                    return off_state_next;
                }
            }
        }

        public Pointer PreviousEntry {
            get { return off_state_prev; }
        }

        public State(Pointer offset, Family family) {
            this.offset = offset;
            this.family = family;
        }

        public override string ToString() {
            string result = name != null ? name : ("Unnamed state @ " + offset);
            if (cine_name != null) result += " | " + cine_name;
            if (cine_mapname != null) result += " (" + cine_mapname + ") ";
            return result;
        }

        public static State Read(Reader reader, Pointer offset, Family family) {
            MapLoader l = MapLoader.Loader;
            State s = new State(offset, family);
            l.states.Add(s);
            if (Settings.s.hasNames) s.name = new string(reader.ReadChars(0x50)).TrimEnd('\0');
            if (l.mode != MapLoader.Mode.RaymanArenaGC) s.off_state_next = Pointer.Read(reader);
            if (l.mode == MapLoader.Mode.Rayman3GC) {
                s.off_state_prev = Pointer.Read(reader);
                Pointer.Read(reader); // another header at tail of state list
            }
            s.off_anim_ref = Pointer.Read(reader);
            s.stateTransitions = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader)); // int is placeholder type
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (l.mode != MapLoader.Mode.RaymanArenaGC) reader.ReadUInt32();
            s.off_state_auto = Pointer.Read(reader);
            Pointer.Read(reader); // fam end?
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                s.off_cine_mapname = Pointer.Read(reader);
                s.off_cine_name = Pointer.Read(reader);
            }
            reader.ReadByte();
            s.speed = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            if (Settings.s.engineMode == Settings.EngineMode.R2) reader.ReadUInt32();

            if (s.off_cine_mapname != null) {
                Pointer.Goto(ref reader, s.off_cine_mapname);
                s.cine_mapname = reader.ReadNullDelimitedString();
            }
            if (s.off_cine_name != null) {
                Pointer.Goto(ref reader, s.off_cine_name);
                s.cine_name = reader.ReadNullDelimitedString();
            }
            if (s.off_anim_ref != null) {
                Pointer.Goto(ref reader, s.off_anim_ref);
                s.anim_ref = AnimationReference.Read(reader, s.off_anim_ref);
            }

            return s;
        }

        public static State FromOffset(Family f, Pointer offset) {
            if (f == null || offset == null) return null;
            return f.states.FirstOrDefault(s => (s != null && s.offset == offset));
        }

        public static State FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.states.FirstOrDefault(s => s.offset == offset);
        }
    }
}
