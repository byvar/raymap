using Newtonsoft.Json;
using OpenSpace.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object.Properties {
    public class State : ILinkedListEntry {
        public Pointer offset;
        [JsonIgnore] public Family family;
        public int index = 0;
        public string name = null;
        public Pointer off_state_next;
        public Pointer off_state_prev;
        public Pointer off_anim_ref;
        public LinkedList<int> stateTransitions;
        public Pointer off_mechanicsIDCard = null;
        public Pointer off_cine_mapname = null;
        public Pointer off_cine_name = null;
        public string cine_mapname = null;
        public string cine_name = null;
        public byte speed;
        public Pointer off_state_auto; // Go to this state after a while if nothing changes
        public AnimationReference anim_ref = null;
        public AnimationMontreal anim_refMontreal = null;
        public MechanicsIDCard mechanicsIDCard;
        public byte customStateBits;
		
        public Pointer NextEntry {
            get {
                if (Settings.s.linkedListType != LinkedList.Type.Minimize) {
                    return off_state_next;
                } else {
                    if (Settings.s.mode == Settings.Mode.RaymanArenaGC) {
                        return offset + 0x28;
                    } else {//if (MapLoader.Loader.mode == MapLoader.Mode.Rayman2DC) {
                        return offset + 0x20;
                    }
                }
            }
        }
		
        public Pointer PreviousEntry {
            get { return off_state_prev; }
        }

		public string ShortName {
			get {
				string shortName = "";
				if (name != null) {
					shortName = name;
					if (shortName.StartsWith(family.name + " - ")) {
						shortName = shortName.Substring((family.name + " - ").Length);
					}
					shortName = "[\"" + shortName + "\"]";
				}
				shortName = family.name + ".Action[" + index + "]" + shortName;
				return shortName;
			}
		}

		public State(Pointer offset, Family family, int index) {
            this.offset = offset;
            this.family = family;
            this.index = index;
        }

        public override string ToString() {
            string result = name != null ? name : ("State #"+index+" "+ Convert.ToString(customStateBits, 2).PadLeft(8, '0')+" @ " + offset);
            if (cine_name != null) result += " | " + cine_name;
            if (cine_mapname != null) result += " (" + cine_mapname + ") ";
            return result;
        }

        public static State Read(Reader reader, Pointer offset, Family family, int index) {
            MapLoader l = MapLoader.Loader;
            State s = new State(offset, family, index);
            l.states.Add(s);
            if (Settings.s.hasNames) s.name = new string(reader.ReadChars(0x50)).TrimEnd('\0');
            if (Settings.s.linkedListType != LinkedList.Type.Minimize) s.off_state_next = Pointer.Read(reader);
            if (Settings.s.hasLinkedListHeaderPointers) {
                s.off_state_prev = Pointer.Read(reader);
                Pointer.Read(reader); // another header at tail of state list
            }
            s.off_anim_ref = Pointer.Read(reader);
            s.stateTransitions = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader)); // int is placeholder type
            LinkedList<int>.ReadHeader(reader, Pointer.Current(reader));
            s.off_state_auto = Pointer.Read(reader, allowMinusOne: true);
            s.off_mechanicsIDCard = Pointer.Read(reader);
            if (Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.game != Settings.Game.Dinosaur) {
                s.off_cine_mapname = Pointer.Read(reader);
                s.off_cine_name = Pointer.Read(reader);
            }
            if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                s.speed = reader.ReadByte();
            } else {
                reader.ReadByte();
                s.speed = reader.ReadByte();
                reader.ReadByte();
                s.customStateBits = reader.ReadByte();
            }
            if (s.off_mechanicsIDCard != null) {
                s.mechanicsIDCard = MechanicsIDCard.FromOffsetOrRead(s.off_mechanicsIDCard, reader);
            }
            if (s.off_cine_mapname != null) {
                Pointer.Goto(ref reader, s.off_cine_mapname);
                s.cine_mapname = reader.ReadNullDelimitedString();
            }
            if (s.off_cine_name != null) {
                Pointer.Goto(ref reader, s.off_cine_name);
                s.cine_name = reader.ReadNullDelimitedString();
            }
            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal || Settings.s.game == Settings.Game.TTSE) {
                s.anim_refMontreal = AnimationMontreal.FromOffsetOrRead(s.off_anim_ref, reader);
            } else {
                s.anim_ref = AnimationReference.FromOffsetOrRead(s.off_anim_ref, reader);
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
