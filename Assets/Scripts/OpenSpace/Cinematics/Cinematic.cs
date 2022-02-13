using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Cinematics {
	/// <summary>
	/// Called "Cine" in the code
	/// </summary>
    public class Cinematic : ILinkedListEntry {
        public LegacyPointer offset;
		public LinkedList<CinematicActor> actors;
		public LegacyPointer off_next;
		public LegacyPointer off_previous;
		public LegacyPointer off_header;
		public uint unk;
        public string name = null;

        public Cinematic(LegacyPointer offset) {
            this.offset = offset;
        }

		public LegacyPointer NextEntry {
			get { return off_next; }
		}

		public LegacyPointer PreviousEntry {
			get { return off_previous; }
		}

		public static Cinematic Read(Reader reader, LegacyPointer offset) {
            MapLoader l = MapLoader.Loader;
			Cinematic c = new Cinematic(offset);
			c.actors = LinkedList<CinematicActor>.Read(ref reader, LegacyPointer.Current(reader), (off_actor) => {
				return CinematicActor.Read(reader, off_actor);
			}, type: LinkedList.Type.Double);
			c.off_next = LegacyPointer.Read(reader);
			c.off_previous = LegacyPointer.Read(reader);
			c.off_header = LegacyPointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			c.name = reader.ReadString(0x100);
			//MapLoader.Loader.print(c.name);
			return c;
        }
    }
}
