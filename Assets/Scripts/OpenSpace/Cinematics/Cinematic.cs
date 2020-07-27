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
        public Pointer offset;
		public LinkedList<CinematicActor> actors;
		public Pointer off_next;
		public Pointer off_previous;
		public Pointer off_header;
		public uint unk;
        public string name = null;

        public Cinematic(Pointer offset) {
            this.offset = offset;
        }

		public Pointer NextEntry {
			get { return off_next; }
		}

		public Pointer PreviousEntry {
			get { return off_previous; }
		}

		public static Cinematic Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
			Cinematic c = new Cinematic(offset);
			c.actors = LinkedList<CinematicActor>.Read(ref reader, Pointer.Current(reader), (off_actor) => {
				return CinematicActor.Read(reader, off_actor);
			}, type: LinkedList.Type.Double);
			c.off_next = Pointer.Read(reader);
			c.off_previous = Pointer.Read(reader);
			c.off_header = Pointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			c.name = reader.ReadString(0x100);
			//MapLoader.Loader.print(c.name);
			return c;
        }
    }
}
