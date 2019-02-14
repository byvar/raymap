using System;

namespace OpenSpace.Collide
{
    public class CollideActivation : ILinkedListEntry
    {
        public Pointer offset;
        public Pointer nextElement;
		public Pointer off_activationZone;
		public LinkedList<CollideActivationZone> activationZone;
        public ushort index;

        public Pointer NextEntry {
            get { return nextElement; }
        }

        public Pointer PreviousEntry {
            get { return null; }
        }

        public CollideActivation(Pointer offset) {
            this.offset = offset;
        }

        public static CollideActivation Read(Reader reader, Pointer offset, CollSet collset, CollideType type) {
            CollideActivation a = new CollideActivation(offset);

            if (Settings.s.linkedListType != LinkedList.Type.Minimize) {
                a.nextElement = Pointer.Read(reader);
            }
			a.off_activationZone = Pointer.Read(reader);
			Pointer.DoAt(ref reader, a.off_activationZone, () => {
				a.activationZone = LinkedList<CollideActivationZone>.Read(ref reader, offset,
						(off_element) => {
							return CollideActivationZone.Read(reader, off_element);
						},
						flags: LinkedList.Flags.NoPreviousPointersForDouble,
						type: LinkedList.Type.Minimize
					);
			});
			a.index = reader.ReadUInt16();
			reader.ReadUInt16();
			if (Settings.s.linkedListType == LinkedList.Type.Minimize) {
				a.nextElement = Pointer.Current(reader);
			}
			return a;
        }
    }
}