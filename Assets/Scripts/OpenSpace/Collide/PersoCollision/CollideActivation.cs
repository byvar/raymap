using System;

namespace OpenSpace.Collide
{
    public class CollideActivation : ILinkedListEntry
    {
        public Pointer offset;
        public Pointer off_next;
		public Pointer off_prev;
		public Pointer off_header;
		public Pointer off_activationZone;
		public LinkedList<CollideActivationZone> activationZone;
        public ushort index;

        public Pointer NextEntry {
            get { return off_next; }
        }

        public Pointer PreviousEntry {
            get { return off_prev; }
        }

        public CollideActivation(Pointer offset) {
            this.offset = offset;
        }

        public static CollideActivation Read(Reader reader, Pointer offset, CollSet collset, CollideType type) {
            CollideActivation a = new CollideActivation(offset);

            if (CPA_Settings.s.linkedListType != LinkedList.Type.Minimize) {
                a.off_next = Pointer.Read(reader);
				if (CPA_Settings.s.hasLinkedListHeaderPointers) {
					a.off_prev = Pointer.Read(reader);
					a.off_header = Pointer.Read(reader);
				}
            }
			a.off_activationZone = Pointer.Read(reader);
			Pointer.DoAt(ref reader, a.off_activationZone, () => {
				a.activationZone = LinkedList<CollideActivationZone>.Read(ref reader, offset,
						(off_element) => {
							return CollideActivationZone.Read(reader, off_element);
						},
						flags: (CPA_Settings.s.hasLinkedListHeaderPointers ?
								LinkedList.Flags.HasHeaderPointers :
								LinkedList.Flags.NoPreviousPointersForDouble),
						type: LinkedList.Type.Minimize
					);
			});
			a.index = reader.ReadUInt16();
			reader.ReadUInt16();
			if (CPA_Settings.s.linkedListType == LinkedList.Type.Minimize) {
				a.off_next = Pointer.Current(reader);
			}
			return a;
        }
    }
}