using System;

namespace OpenSpace.Collide
{
    public class CollideActivationZone : ILinkedListEntry
    {
        public Pointer offset;
        public Pointer off_next;
		public Pointer off_prev;
		public Pointer off_header;
		public ushort zdxIndex;

		public Pointer NextEntry {
            get { return off_next; }
        }

        public Pointer PreviousEntry {
            get { return off_prev; }
        }

        public CollideActivationZone(Pointer offset) {
            this.offset = offset;
        }

        public static CollideActivationZone Read(Reader reader, Pointer offset) {
            CollideActivationZone z = new CollideActivationZone(offset);
			if (Settings.s.linkedListType != LinkedList.Type.Minimize) {
				z.off_next = Pointer.Read(reader);
				if (Settings.s.hasLinkedListHeaderPointers) {
					z.off_prev = Pointer.Read(reader);
					z.off_header = Pointer.Read(reader);
				}
			}
			z.zdxIndex = reader.ReadUInt16();
			if (Settings.s.linkedListType == LinkedList.Type.Minimize) {
				z.off_next = Pointer.Current(reader);
			}
			return z;
        }
    }
}