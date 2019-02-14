using System;

namespace OpenSpace.Collide
{
    public class CollideActivationZone : ILinkedListEntry
    {
        public Pointer offset;
        public Pointer nextElement;
		public ushort zdxIndex;

		public Pointer NextEntry {
            get { return nextElement; }
        }

        public Pointer PreviousEntry {
            get { return null; }
        }

        public CollideActivationZone(Pointer offset) {
            this.offset = offset;
        }

        public static CollideActivationZone Read(Reader reader, Pointer offset) {
            CollideActivationZone e = new CollideActivationZone(offset);

			if (Settings.s.linkedListType != LinkedList.Type.Minimize) {
				e.nextElement = Pointer.Read(reader);
			}
			e.zdxIndex = reader.ReadUInt16();
			if (Settings.s.linkedListType == LinkedList.Type.Minimize) {
				e.nextElement = Pointer.Current(reader);
			}
			return e;
        }
    }
}