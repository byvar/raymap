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
        public Pointer off_entry_next;
        public Pointer off_entry_prev;
        public Pointer off_anim_ref;
        public LinkedList<Transition> stateTransitions;
		public LinkedList<Prohibit> prohobitStates;
        public Pointer off_mechanicsIDCard = null;
        public Pointer off_cine_mapname = null;
        public Pointer off_cine_name = null;
        public string cine_mapname = null;
        public string cine_name = null;
        public byte speed;
        public Pointer off_nextState; // Go to this state after a while if nothing changes
        public AnimationReference anim_ref = null;
        public AnimationMontreal anim_refMontreal = null;
        public MechanicsIDCard mechanicsIDCard;
        public byte customStateBits;
		
        public Pointer NextEntry {
            get {
                if (Settings.s.linkedListType != LinkedList.Type.Minimize) {
                    return off_entry_next;
                } else {
					if (Settings.s.mode == Settings.Mode.RaymanArenaGC
						|| Settings.s.mode == Settings.Mode.RaymanArenaGCDemo
						|| Settings.s.mode == Settings.Mode.DonaldDuckPKGC
                        || (Settings.s.platform == Settings.Platform.PS2 && Settings.s.engineVersion == Settings.EngineVersion.R3)) {
                        return offset + 0x28 + (Settings.s.hasNames ? 0x50 : 0);
                    } else {//if (MapLoader.Loader.mode == MapLoader.Mode.Rayman2DC) {
                        return offset + 0x20 + (Settings.s.hasNames ? 0x50 : 0);
                    }
                }
            }
        }
		
        public Pointer PreviousEntry {
            get { return off_entry_prev; }
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
            //l.print("State " + Pointer.Current(reader));
            State s = new State(offset, family, index);
            l.states.Add(s);
            if (Settings.s.hasNames) s.name = new string(reader.ReadChars(0x50)).TrimEnd('\0');
            if (Settings.s.linkedListType != LinkedList.Type.Minimize) s.off_entry_next = Pointer.Read(reader);
            if (Settings.s.hasLinkedListHeaderPointers) {
                s.off_entry_prev = Pointer.Read(reader);
                Pointer.Read(reader); // another header at tail of state list
            }
            s.off_anim_ref = Pointer.Read(reader);
			s.stateTransitions = LinkedList<Transition>.Read(ref reader, Pointer.Current(reader), element => {
				return l.FromOffsetOrRead<Transition>(reader, element);
			});
			s.prohobitStates = LinkedList<Prohibit>.Read(ref reader, Pointer.Current(reader), element => {
				return l.FromOffsetOrRead<Prohibit>(reader, element);
			});
            s.off_nextState = Pointer.Read(reader, allowMinusOne: true);
            s.off_mechanicsIDCard = Pointer.Read(reader);
            if (Settings.s.engineVersion == Settings.EngineVersion.R3
				&& Settings.s.game != Settings.Game.Dinosaur
				&& Settings.s.game != Settings.Game.LargoWinch) {
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
				if (Settings.s.game == Settings.Game.LargoWinch) reader.ReadByte();
            }
            if (s.off_mechanicsIDCard != null) {
                s.mechanicsIDCard = MechanicsIDCard.FromOffsetOrRead(s.off_mechanicsIDCard, reader);
            }
			Pointer.DoAt(ref reader, s.off_cine_mapname, () => {
				s.cine_mapname = reader.ReadNullDelimitedString();
			});
			Pointer.DoAt(ref reader, s.off_cine_name, () => {
				s.cine_name = reader.ReadNullDelimitedString();
			});
            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal || Settings.s.game == Settings.Game.TTSE) {
                s.anim_refMontreal = l.FromOffsetOrRead<AnimationMontreal>(reader, s.off_anim_ref);
            } else {
                s.anim_ref = l.FromOffsetOrRead<AnimationReference>(reader, s.off_anim_ref);
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

		public class Transition : OpenSpaceStruct, ILinkedListEntry {
			public Pointer off_entry_next;
			public Pointer off_entry_prev;
			public Pointer off_entry_hdr;

			public Pointer off_targetState;
			public Pointer off_stateToGo;
			public byte linkingType;

			// Custom
			public Pointer NextEntry => (Settings.s.linkedListType != LinkedList.Type.Minimize) ? (off_entry_next) : (Offset + Size);
			public Pointer PreviousEntry => off_entry_prev;

			protected override void ReadInternal(Reader reader) {
				if (Settings.s.linkedListType != LinkedList.Type.Minimize) {
					off_entry_next = Pointer.Read(reader);
					if (Settings.s.hasLinkedListHeaderPointers) {
						off_entry_prev = Pointer.Read(reader);
						off_entry_hdr = Pointer.Read(reader);
					}
				}
				//MapLoader.Loader.print("Transition " + Offset);
				off_targetState = Pointer.Read(reader);
				off_stateToGo = Pointer.Read(reader);
				linkingType = reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
			}
		}

		public class Prohibit : OpenSpaceStruct, ILinkedListEntry {
			public Pointer off_entry_next;
			public Pointer off_entry_prev;
			public Pointer off_entry_hdr;

			public Pointer off_state;

			//Custom
			public Pointer NextEntry => (Settings.s.linkedListType != LinkedList.Type.Minimize) ? (off_entry_next) : (Offset + Size);
			public Pointer PreviousEntry => off_entry_prev;

			protected override void ReadInternal(Reader reader) {
				if (Settings.s.linkedListType != LinkedList.Type.Minimize) {
					off_entry_next = Pointer.Read(reader);
					if (Settings.s.hasLinkedListHeaderPointers) {
						off_entry_prev = Pointer.Read(reader);
						off_entry_hdr = Pointer.Read(reader);
					}
				}
				//MapLoader.Loader.print("Prohibit " + Offset);
				off_state = Pointer.Read(reader);
			}
		}
	}
}
