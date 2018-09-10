

using System;
using System.Collections;
using System.Collections.Generic;
using Type = OpenSpace.LinkedList.Type;

namespace OpenSpace {
    public static class LinkedList {
        public enum Type { Default = -1, Single, Double, SingleNoElementPointers, DoubleNoElementPointers };

        [Flags]
        public enum Flags {
            None = 0,
            HasHeaderPointers = 1,
            ReadAtPointer = 2, // The list contains a pointer which points to the struct location
            ElementPointerFirst = 4, // Order: element pointer, next pointer, prev pointer
            NoPreviousPointersForDouble = 8 // Even with double linked list type, there is no prev pointer here
        };
    }

    public class LinkedList<T> : IList<T> {
        public delegate T ReadElement(Pointer offset);

        public Pointer offset;
        public Type type;
        public Pointer off_head;
        public Pointer off_tail;
        private uint num_elements;
        public Pointer off_header;
        private bool customEntries = false;

        private T[] list = null;

        public int Count {
            get { return (int)num_elements; }
            set { num_elements = (uint)value; }
        }

        public bool IsReadOnly {
            get { return list.IsReadOnly; }
        }

        public T this[int index] {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public LinkedList(Pointer offset) {
            this.offset = offset;
            if (typeof(ILinkedListEntry).IsAssignableFrom(typeof(T))) customEntries = true;
        }

        public static LinkedList<T> ReadHeader(Reader reader, Pointer offset, Type type = Type.Default) {
            MapLoader l = MapLoader.Loader;
            LinkedList<T> li = new LinkedList<T>(offset);
            li.type = type;
            if (li.type == Type.Default) {
                li.type = Settings.s.linkedListType;
            }
            li.off_head = Pointer.Read(reader);
            if (li.type == Type.Double || li.type == Type.DoubleNoElementPointers) li.off_tail = Pointer.Read(reader);
            li.num_elements = reader.ReadUInt32();
            li.list = new T[li.num_elements];
            return li;
        }

        public void ReadEntries(ref Reader reader, ReadElement readElement, LinkedList.Flags flags = LinkedList.Flags.None) {
            Pointer off_next = off_head;
            bool elementPointerFirst = ((flags & LinkedList.Flags.ElementPointerFirst) != 0);
            bool hasHeaderPointers = ((flags & LinkedList.Flags.HasHeaderPointers) != 0);
            bool readAtPointer = ((flags & LinkedList.Flags.ReadAtPointer) != 0);
            bool noPreviousPointers = ((flags & LinkedList.Flags.NoPreviousPointersForDouble) != 0);
            if (off_head != null) {
                Pointer off_current = Pointer.Goto(ref reader, off_head);
                for (int i = 0; i < num_elements; i++) {
                    Pointer off_element = off_next;
                    if (elementPointerFirst) off_element = Pointer.Read(reader);
                    if (type != Type.SingleNoElementPointers && type != Type.DoubleNoElementPointers && !customEntries) {
                        off_next = Pointer.Read(reader);
                        if (type == Type.Double && !noPreviousPointers) Pointer.Read(reader); // previous element pointer
                        if (hasHeaderPointers) Pointer.Read(reader); // header struct pointer
                    }
                    if (readAtPointer && !elementPointerFirst) off_element = Pointer.Read(reader);
                    // Read element
                    if (!readAtPointer) {
                        list[i] = readElement(off_element);
                    } else {
                        Pointer.DoAt(ref reader, off_element, () => {
                            list[i] = readElement(off_element);
                        });
                    }

                    // Goto next element
                    if (customEntries) {
                        off_next = ((ILinkedListEntry)list[i]).NextEntry;
                    }
                    if (i < num_elements-1 && (customEntries || (type != Type.SingleNoElementPointers && type != Type.DoubleNoElementPointers))) {
                        Pointer.Goto(ref reader, off_next);
                    } else {
                        off_next = Pointer.Current(reader);
                    }
                }
                Pointer.Goto(ref reader, off_current);
            }
        }

        public static LinkedList<T> Read(ref Reader reader, Pointer offset, ReadElement readElement,
            LinkedList.Flags flags = LinkedList.Flags.None,
            Type type = Type.Default) {
            LinkedList<T> li = ReadHeader(reader, offset, type: type);
            li.ReadEntries(ref reader, readElement, flags: flags);
            return li;
        }

        public void FillPointers(Reader reader, Pointer lastEntry, Pointer header, uint nextOffset = 0, uint prevOffset = 4, uint headerOffset = 8) {
            Pointer current_entry = lastEntry;
            Pointer next_entry = null;
            Pointer off_current = Pointer.Current(reader);
            while (current_entry != null) {
                Pointer.Goto(ref reader, current_entry);
                current_entry.file.AddPointer(current_entry.offset + nextOffset, next_entry);
                if (header != null) {
                    current_entry.file.AddPointer(current_entry.offset + headerOffset, header);
                }
                next_entry = current_entry;
                current_entry = Pointer.GetPointerAtOffset(current_entry + prevOffset);
            }
            Pointer.Goto(ref reader, off_current);
        }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<T>)list).GetEnumerator();
        }

        public int IndexOf(T item) {
            return Array.IndexOf(list, item);
        }

        public bool Contains(T item) {
            return Array.IndexOf(list, item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            list.CopyTo(array, arrayIndex);
        }

        void IList<T>.Insert(int index, T item) {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index) {
            throw new NotImplementedException();
        }

        public void Add(T item) {
            Array.Resize(ref list, list.Length + 1);
            list[list.Length - 1] = item;
        }

        void ICollection<T>.Clear() {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item) {
            throw new NotImplementedException();
        }
    }
}
