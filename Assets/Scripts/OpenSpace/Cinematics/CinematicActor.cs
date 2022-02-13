using OpenSpace.Animation;
using OpenSpace.Animation.Component;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Cinematics {
	/// <summary>
	/// Called "Cine" in the code
	/// </summary>
    public class CinematicActor : ILinkedListEntry {
        public LegacyPointer offset;
		public LegacyPointer off_a3d;
		public AnimationReference anim_ref;
		public string name = null;
		public LegacyPointer off_perso;
		public LegacyPointer off_waitState;
		public LegacyPointer off_cineState;
		public Perso perso;
		public State waitState;
		public State cineState;
		public byte setCustomBitFlag1, setCustomBitFlag2;
		public byte cineStateRepeatAnimation;
		public byte cineStateSpeed;
		public LegacyPointer off_IPO;
		public byte hasChangeComport1, hasChangeComport2;
		public LegacyPointer off_comport1, off_comport2;
		public byte[] hasSnd;
		public uint[] snd = new uint[4]; // CEventResData
		public LegacyPointer off_cinematic;

		public LegacyPointer off_next;
		public LegacyPointer off_previous;
		public LegacyPointer off_header;

        public CinematicActor(LegacyPointer offset) {
            this.offset = offset;
        }

		public LegacyPointer NextEntry {
			get { return off_next; }
		}

		public LegacyPointer PreviousEntry {
			get { return off_previous; }
		}

		public static CinematicActor Read(Reader reader, LegacyPointer offset) {
            MapLoader l = MapLoader.Loader;
			CinematicActor ca = new CinematicActor(offset);
			if (CPA_Settings.s.game == CPA_Settings.Game.R3 || CPA_Settings.s.game == CPA_Settings.Game.Dinosaur) {
				if (CPA_Settings.s.platform != CPA_Settings.Platform.PS2) {
					if (CPA_Settings.s.platform == CPA_Settings.Platform.GC) {
						reader.ReadUInt32();
						reader.ReadUInt32();
					}
					reader.ReadUInt32();
				}
			}
			ca.off_a3d = LegacyPointer.Read(reader);
			reader.ReadUInt32();
			ca.name = reader.ReadString(0x100);
			ca.off_perso = LegacyPointer.Read(reader);
			ca.off_waitState = LegacyPointer.Read(reader);
			ca.off_cineState = LegacyPointer.Read(reader);
			reader.ReadUInt32();
			ca.setCustomBitFlag1 = reader.ReadByte();
			ca.setCustomBitFlag2 = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ca.cineStateRepeatAnimation = reader.ReadByte();
			ca.cineStateSpeed = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ca.off_IPO = LegacyPointer.Read(reader);
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ca.hasChangeComport1 = reader.ReadByte();
			reader.ReadByte();
			ca.off_comport1 = LegacyPointer.Read(reader);
			ca.hasChangeComport2 = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ca.off_comport2 = LegacyPointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			ca.hasSnd = reader.ReadBytes(4);
			for (int i = 0; i < 4; i++) {
				ca.snd[i] = reader.ReadUInt32();
			}
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			ca.off_cinematic = LegacyPointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			ca.off_next = LegacyPointer.Read(reader);
			ca.off_previous = LegacyPointer.Read(reader);
			ca.off_header = LegacyPointer.Read(reader);

			//MapLoader.Loader.print(ca.name);
			ca.anim_ref = l.FromOffsetOrRead<AnimationReference>(reader, ca.off_a3d);
			ca.perso = Perso.FromOffset(ca.off_perso);
			ca.waitState = State.FromOffset(ca.off_waitState);
			ca.cineState = State.FromOffset(ca.off_cineState);

			return ca;
        }
    }
}
