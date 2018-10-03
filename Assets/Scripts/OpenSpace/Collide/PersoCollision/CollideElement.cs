using System;

namespace OpenSpace.Collide
{
    public class CollideElement : ILinkedListEntry
    {
        public Pointer offset;
        public Pointer nextElement;
        public ushort index;

        public Pointer NextEntry {
            get { return nextElement; }
        }

        public Pointer PreviousEntry {
            get { return null; }
        }

        public CollideElement(Pointer offset) {
            this.offset = offset;
        }

        public static CollideElement Read(Reader reader, Pointer offset) {
            CollideElement element = new CollideElement(offset);

            if (Settings.s.linkedListType != LinkedList.Type.Minimize) {
                element.nextElement = Pointer.Read(reader);
            }
            element.index = reader.ReadUInt16();
            reader.ReadUInt16();
            return element;
        }
    }
}