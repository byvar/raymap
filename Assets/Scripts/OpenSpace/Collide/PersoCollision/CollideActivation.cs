using System;

namespace OpenSpace.Collide
{
    public class CollideActivation : ILinkedListEntry
    {
        public LegacyPointer offset;
        public LegacyPointer off_next;
		public LegacyPointer off_prev;
		public LegacyPointer off_header;
		public LegacyPointer off_activationZone;
		public LinkedList<CollideActivationZone> activationZone;
        public ushort index;

        public LegacyPointer NextEntry {
            get { return off_next; }
        }

        public LegacyPointer PreviousEntry {
            get { return off_prev; }
        }

        public CollideActivation(LegacyPointer offset) {
            this.offset = offset;
        }

        public static CollideActivation Read(Reader reader, LegacyPointer offset, CollSet collset, CollideType type) {
            CollideActivation a = new CollideActivation(offset);

            if (Legacy_Settings.s.linkedListType != LinkedList.Type.Minimize) {
                a.off_next = LegacyPointer.Read(reader);
				if (Legacy_Settings.s.hasLinkedListHeaderPointers) {
					a.off_prev = LegacyPointer.Read(reader);
					a.off_header = LegacyPointer.Read(reader);
				}
            }
			a.off_activationZone = LegacyPointer.Read(reader);
			LegacyPointer.DoAt(ref reader, a.off_activationZone, () => {
				a.activationZone = LinkedList<CollideActivationZone>.Read(ref reader, offset,
						(off_element) => {
							return CollideActivationZone.Read(reader, off_element);
						},
						flags: (Legacy_Settings.s.hasLinkedListHeaderPointers ?
								LinkedList.Flags.HasHeaderPointers :
								LinkedList.Flags.NoPreviousPointersForDouble),
						type: LinkedList.Type.Minimize
					);
			});
			a.index = reader.ReadUInt16();
			reader.ReadUInt16();
			if (Legacy_Settings.s.linkedListType == LinkedList.Type.Minimize) {
				a.off_next = LegacyPointer.Current(reader);
			}
			return a;
        }
    }
}