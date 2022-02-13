using System;

namespace OpenSpace.Collide
{
    public class CollideActivationZone : ILinkedListEntry
    {
        public LegacyPointer offset;
        public LegacyPointer off_next;
		public LegacyPointer off_prev;
		public LegacyPointer off_header;
		public ushort zdxIndex;

		public LegacyPointer NextEntry {
            get { return off_next; }
        }

        public LegacyPointer PreviousEntry {
            get { return off_prev; }
        }

        public CollideActivationZone(LegacyPointer offset) {
            this.offset = offset;
        }

        public static CollideActivationZone Read(Reader reader, LegacyPointer offset) {
            CollideActivationZone z = new CollideActivationZone(offset);
			if (CPA_Settings.s.linkedListType != LinkedList.Type.Minimize) {
				z.off_next = LegacyPointer.Read(reader);
				if (CPA_Settings.s.hasLinkedListHeaderPointers) {
					z.off_prev = LegacyPointer.Read(reader);
					z.off_header = LegacyPointer.Read(reader);
				}
			}
			z.zdxIndex = reader.ReadUInt16();
			if (CPA_Settings.s.linkedListType == LinkedList.Type.Minimize) {
				z.off_next = LegacyPointer.Current(reader);
			}
			return z;
        }
    }
}