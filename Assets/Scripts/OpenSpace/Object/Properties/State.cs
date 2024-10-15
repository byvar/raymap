using Newtonsoft.Json;
using OpenSpace.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object.Properties {
    public class State : ILinkedListEntry {
        public LegacyPointer offset;
        [JsonIgnore] public Family family;
        public int index = 0;
        public string name = null;
        public LegacyPointer off_entry_next;
        public LegacyPointer off_entry_prev;
        public LegacyPointer off_anim_ref;
        public LinkedList<Transition> stateTransitions;
		public LinkedList<Prohibit> prohobitStates;
        public LegacyPointer off_mechanicsIDCard = null;
        public LegacyPointer off_cine_mapname = null;
        public LegacyPointer off_cine_name = null;
        public string cine_mapname = null;
        public string cine_name = null;
        public byte speed;
        public LegacyPointer off_nextState; // Go to this state after a while if nothing changes
        public AnimationReference anim_ref = null;
        public AnimationMontreal anim_refMontreal = null;
        public MechanicsIDCard mechanicsIDCard;
        public byte customStateBits;
		
        public LegacyPointer NextEntry {
            get {
                if (Legacy_Settings.s.linkedListType != LinkedList.Type.Minimize) {
                    return off_entry_next;
                } else {
					if (Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaGC
						|| Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaGCDemo_2002_03_07
						|| Legacy_Settings.s.mode == Legacy_Settings.Mode.DonaldDuckPKGC
                        || (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2 && Legacy_Settings.s.engineVersion == Legacy_Settings.EngineVersion.R3)) {
                        return offset + 0x28 + (Legacy_Settings.s.hasNames ? 0x50 : 0);
                    } else {//if (MapLoader.Loader.mode == MapLoader.Mode.Rayman2DC) {
                        return offset + 0x20 + (Legacy_Settings.s.hasNames ? 0x50 : 0);
                    }
                }
            }
        }
		
        public LegacyPointer PreviousEntry {
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

		public State(LegacyPointer offset, Family family, int index) {
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

        public static State Read(Reader reader, LegacyPointer offset, Family family, int index) {
            MapLoader l = MapLoader.Loader;
            //l.print("State " + Pointer.Current(reader));
            State s = new State(offset, family, index);
            l.states.Add(s);
            if (Legacy_Settings.s.hasNames) s.name = new string(reader.ReadChars(0x50)).TrimEnd('\0');
            if (Legacy_Settings.s.linkedListType != LinkedList.Type.Minimize) s.off_entry_next = LegacyPointer.Read(reader);
            if (Legacy_Settings.s.hasLinkedListHeaderPointers) {
                s.off_entry_prev = LegacyPointer.Read(reader);
                LegacyPointer.Read(reader); // another header at tail of state list
            }
            s.off_anim_ref = LegacyPointer.Read(reader);
			s.stateTransitions = LinkedList<Transition>.Read(ref reader, LegacyPointer.Current(reader), element => {
				return l.FromOffsetOrRead<Transition>(reader, element);
			});
			s.prohobitStates = LinkedList<Prohibit>.Read(ref reader, LegacyPointer.Current(reader), element => {
				return l.FromOffsetOrRead<Prohibit>(reader, element);
			});
            s.off_nextState = LegacyPointer.Read(reader, allowMinusOne: true);
            s.off_mechanicsIDCard = LegacyPointer.Read(reader);
            if (Legacy_Settings.s.engineVersion == Legacy_Settings.EngineVersion.R3
				&& Legacy_Settings.s.game != Legacy_Settings.Game.Dinosaur
				&& Legacy_Settings.s.game != Legacy_Settings.Game.LargoWinch) {
                s.off_cine_mapname = LegacyPointer.Read(reader);
                s.off_cine_name = LegacyPointer.Read(reader);
			}
			if (Legacy_Settings.s.engineVersion <= Legacy_Settings.EngineVersion.Montreal) {
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
				if (Legacy_Settings.s.game == Legacy_Settings.Game.LargoWinch) reader.ReadByte();
            }
            if (s.off_mechanicsIDCard != null) {
                s.mechanicsIDCard = MechanicsIDCard.FromOffsetOrRead(s.off_mechanicsIDCard, reader);
            }
			LegacyPointer.DoAt(ref reader, s.off_cine_mapname, () => {
				s.cine_mapname = reader.ReadNullDelimitedString();
			});
			LegacyPointer.DoAt(ref reader, s.off_cine_name, () => {
				s.cine_name = reader.ReadNullDelimitedString();
			});
            if (Legacy_Settings.s.engineVersion == Legacy_Settings.EngineVersion.Montreal || Legacy_Settings.s.game == Legacy_Settings.Game.TTSE || Legacy_Settings.s.game == Legacy_Settings.Game.R2Beta) {
                s.anim_refMontreal = l.FromOffsetOrRead<AnimationMontreal>(reader, s.off_anim_ref);
            } else {
                s.anim_ref = l.FromOffsetOrRead<AnimationReference>(reader, s.off_anim_ref);
            }
            return s;
        }

        public static State FromOffset(Family f, LegacyPointer offset) {
            if (f == null || offset == null) return null;
            return f.states.FirstOrDefault(s => (s != null && s.offset == offset));
        }

        public static State FromOffset(LegacyPointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.states.FirstOrDefault(s => s.offset == offset);
        }

		public class Transition : OpenSpaceStruct, ILinkedListEntry {
			public LegacyPointer off_entry_next;
			public LegacyPointer off_entry_prev;
			public LegacyPointer off_entry_hdr;

			public LegacyPointer off_targetState;
			public LegacyPointer off_stateToGo;
			public byte linkingType;

			// Custom
			public LegacyPointer NextEntry => (Legacy_Settings.s.linkedListType != LinkedList.Type.Minimize) ? (off_entry_next) : (Offset + Size);
			public LegacyPointer PreviousEntry => off_entry_prev;

			protected override void ReadInternal(Reader reader) {
				if (Legacy_Settings.s.linkedListType != LinkedList.Type.Minimize) {
					off_entry_next = LegacyPointer.Read(reader);
					if (Legacy_Settings.s.hasLinkedListHeaderPointers) {
						off_entry_prev = LegacyPointer.Read(reader);
						off_entry_hdr = LegacyPointer.Read(reader);
					}
				}
				//MapLoader.Loader.print("Transition " + Offset);
				off_targetState = LegacyPointer.Read(reader);
				off_stateToGo = LegacyPointer.Read(reader);
				linkingType = reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
			}
		}

		public class Prohibit : OpenSpaceStruct, ILinkedListEntry {
			public LegacyPointer off_entry_next;
			public LegacyPointer off_entry_prev;
			public LegacyPointer off_entry_hdr;

			public LegacyPointer off_state;

			//Custom
			public LegacyPointer NextEntry => (Legacy_Settings.s.linkedListType != LinkedList.Type.Minimize) ? (off_entry_next) : (Offset + Size);
			public LegacyPointer PreviousEntry => off_entry_prev;

			protected override void ReadInternal(Reader reader) {
				if (Legacy_Settings.s.linkedListType != LinkedList.Type.Minimize) {
					off_entry_next = LegacyPointer.Read(reader);
					if (Legacy_Settings.s.hasLinkedListHeaderPointers) {
						off_entry_prev = LegacyPointer.Read(reader);
						off_entry_hdr = LegacyPointer.Read(reader);
					}
				}
				//MapLoader.Loader.print("Prohibit " + Offset);
				off_state = LegacyPointer.Read(reader);
			}
		}
	}
}
