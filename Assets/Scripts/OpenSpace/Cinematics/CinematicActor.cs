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
        public Pointer offset;
		public Pointer off_a3d;
		public AnimationReference anim_ref;
		public string name = null;
		public Pointer off_perso;
		public Pointer off_waitState;
		public Pointer off_cineState;
		public Perso perso;
		public State waitState;
		public State cineState;
		public byte setCustomBitFlag1, setCustomBitFlag2;
		public byte cineStateRepeatAnimation;
		public byte cineStateSpeed;
		public Pointer off_IPO;
		public byte hasChangeComport1, hasChangeComport2;
		public Pointer off_comport1, off_comport2;
		public byte[] hasSnd;
		public uint[] snd = new uint[4]; // CEventResData

		public Pointer off_next;
		public Pointer off_previous;
		public Pointer off_header;

        public CinematicActor(Pointer offset) {
            this.offset = offset;
        }

		public Pointer NextEntry {
			get { return off_next; }
		}

		public Pointer PreviousEntry {
			get { return off_previous; }
		}

		public static CinematicActor Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
			CinematicActor ca = new CinematicActor(offset);
			if (Settings.s.game == Settings.Game.R3 || Settings.s.game == Settings.Game.Dinosaur) {
				if (Settings.s.platform != Settings.Platform.PS2) {
					if (Settings.s.platform == Settings.Platform.GC) {
						reader.ReadUInt32();
						reader.ReadUInt32();
					}
					reader.ReadUInt32();
				}
			}
			ca.off_a3d = Pointer.Read(reader);
			reader.ReadUInt32();
			ca.name = reader.ReadString(0x100);
			ca.off_perso = Pointer.Read(reader);
			ca.off_waitState = Pointer.Read(reader);
			ca.off_cineState = Pointer.Read(reader);
			reader.ReadUInt32();
			ca.setCustomBitFlag1 = reader.ReadByte();
			ca.setCustomBitFlag2 = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ca.cineStateRepeatAnimation = reader.ReadByte();
			ca.cineStateSpeed = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ca.off_IPO = Pointer.Read(reader);
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ca.hasChangeComport1 = reader.ReadByte();
			reader.ReadByte();
			ca.off_comport1 = Pointer.Read(reader);
			ca.hasChangeComport2 = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ca.off_comport2 = Pointer.Read(reader);
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
			Pointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			ca.off_next = Pointer.Read(reader);
			ca.off_previous = Pointer.Read(reader);
			ca.off_header = Pointer.Read(reader);

			//MapLoader.Loader.print(ca.name);
			ca.anim_ref = l.FromOffsetOrRead<AnimationReference>(reader, ca.off_a3d);
			ca.perso = Perso.FromOffset(ca.off_perso);
			ca.waitState = State.FromOffset(ca.off_waitState);
			ca.cineState = State.FromOffset(ca.off_cineState);

			return ca;
        }
    }
}
