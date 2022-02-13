

using System;
using System.Collections;
using System.Collections.Generic;
using Type = OpenSpace.LinkedList.Type;

namespace OpenSpace {
    public static class LinkedList {
        public enum Type { Default = -1, Single, Double, SingleNoElementPointers, DoubleNoElementPointers, Minimize };

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
        public delegate T ReadElement(LegacyPointer offset);

        public LegacyPointer offset;
        public Type type;
        public LegacyPointer off_head;
        public LegacyPointer off_tail;
        private uint num_elements;
        public LegacyPointer off_header;
        private bool customEntries = false;

        private T[] list = null;

        public int Count {
            get { return (int)num_elements; }
            set {
				num_elements = (uint)value;
				if (list.Length != num_elements) {
					Array.Resize(ref list, (int)num_elements);
				}
			}
        }

        public bool IsReadOnly {
            get { return list.IsReadOnly; }
        }

        public T this[int index] {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public LinkedList(LegacyPointer offset) {
            this.offset = offset;
            if (typeof(ILinkedListEntry).IsAssignableFrom(typeof(T))) customEntries = true;
        }

		public LinkedList(LegacyPointer offset, LegacyPointer off_head, LegacyPointer off_tail, uint num_elements, Type type = Type.Default) : this(offset) {
			this.off_head = off_head;
			this.off_tail = off_tail;
			this.num_elements = num_elements;
			list = new T[num_elements];

			if (type == Type.Default) {
				type = CPA_Settings.s.linkedListType;
				if (type == Type.Minimize) {
					type = Type.Single;
				}
			} else if (type == Type.Minimize) {
				/* Minimize works as follows. A linkedlist with type minimize is a default one,
                but if the default type is also Minimize (RA GC, R2 DC) then it is a SingleNoElementPointers list (i.e. optimized to an array).
                If the list itself does not specify the minimize type, it is read as a default one,
                but if the default type is Minimize then it becomes a Single list (i.e. not an array, but no previous pointers).
                */
				type = CPA_Settings.s.linkedListType;
				if (type == Type.Minimize) {
					type = Type.SingleNoElementPointers;
				}
			}
			this.type = type;

		}

		public LinkedList(LegacyPointer offset, LegacyPointer off_head, uint num_elements, Type type = Type.Default) : this(offset, off_head, null, num_elements, type) {}


		public static LinkedList<T> ReadHeader(Reader reader, LegacyPointer offset, Type type = Type.Default) {
            MapLoader l = MapLoader.Loader;
            LinkedList<T> li = new LinkedList<T>(offset);
            li.type = type;
            if (li.type == Type.Default) {
                li.type = CPA_Settings.s.linkedListType;
                if (li.type == Type.Minimize) {
                    li.type = Type.Single;
                }
            } else if (li.type == Type.Minimize) {
                /* Minimize works as follows. A linkedlist with type minimize is a default one,
                but if the default type is also Minimize (RA GC, R2 DC) then it is a SingleNoElementPointers list (i.e. optimized to an array).
                If the list itself does not specify the minimize type, it is read as a default one,
                but if the default type is Minimize then it becomes a Single list (i.e. not an array, but no previous pointers).
                */
                li.type = CPA_Settings.s.linkedListType;
                if (li.type == Type.Minimize) {
                    li.type = Type.SingleNoElementPointers;
                }
            }
            li.off_head = LegacyPointer.Read(reader);
            if (li.type == Type.Double || li.type == Type.DoubleNoElementPointers) li.off_tail = LegacyPointer.Read(reader);
            li.num_elements = reader.ReadUInt32();
            li.list = new T[li.num_elements];
            return li;
        }

        public void ReadEntries(ref Reader reader, ReadElement readElement, LinkedList.Flags flags = LinkedList.Flags.None) {
            LegacyPointer off_next = off_head;
            bool elementPointerFirst = ((flags & LinkedList.Flags.ElementPointerFirst) != 0);
            bool hasHeaderPointers = ((flags & LinkedList.Flags.HasHeaderPointers) != 0);
            bool readAtPointer = ((flags & LinkedList.Flags.ReadAtPointer) != 0);
            bool noPreviousPointers = ((flags & LinkedList.Flags.NoPreviousPointersForDouble) != 0);
            if (off_head != null) {
                LegacyPointer off_current = LegacyPointer.Goto(ref reader, off_head);
                for (int i = 0; i < num_elements; i++) {
                    LegacyPointer off_element = off_next;
                    if (elementPointerFirst) off_element = LegacyPointer.Read(reader);
                    if (type != Type.SingleNoElementPointers && type != Type.DoubleNoElementPointers && !customEntries) {
                        off_next = LegacyPointer.Read(reader);
                        if (type == Type.Double && !noPreviousPointers) LegacyPointer.Read(reader); // previous element pointer
                        if (hasHeaderPointers) LegacyPointer.Read(reader); // header struct pointer
                    }
                    if (readAtPointer && !elementPointerFirst) off_element = LegacyPointer.Read(reader);
                    // Read element
                    if (!readAtPointer) {
                        list[i] = readElement(off_element);
                    } else {
                        LegacyPointer.DoAt(ref reader, off_element, () => {
                            list[i] = readElement(off_element);
                        });
                    }

                    // Goto next element
                    if (customEntries) {
                        off_next = ((ILinkedListEntry)list[i]).NextEntry;
                    }
                    if (i < num_elements-1 && (customEntries || (type != Type.SingleNoElementPointers && type != Type.DoubleNoElementPointers))) {
                        if (off_next == null) {
                            num_elements = (uint)i + 1;
                            break;
                        }
                        LegacyPointer.Goto(ref reader, off_next);
                    } else {
                        off_next = LegacyPointer.Current(reader);
                    }
                }
                LegacyPointer.Goto(ref reader, off_current);
            }
        }

        public void ReadEntriesBackwards(ref Reader reader, ReadElement readElement, LinkedList.Flags flags = LinkedList.Flags.None) {
            LegacyPointer off_next = off_tail;
            bool elementPointerFirst = ((flags & LinkedList.Flags.ElementPointerFirst) != 0);
            bool hasHeaderPointers = ((flags & LinkedList.Flags.HasHeaderPointers) != 0);
            bool readAtPointer = ((flags & LinkedList.Flags.ReadAtPointer) != 0);
            bool noPreviousPointers = ((flags & LinkedList.Flags.NoPreviousPointersForDouble) != 0);
            if (off_tail != null) {
                LegacyPointer off_current = LegacyPointer.Goto(ref reader, off_tail);
                for (int i = 0; i < num_elements; i++) {
                    LegacyPointer off_element = off_next;
                    if (elementPointerFirst) off_element = LegacyPointer.Read(reader);
                    if (type != Type.SingleNoElementPointers && type != Type.DoubleNoElementPointers && !customEntries) {
                        off_next = LegacyPointer.Read(reader);
                        if (type == Type.Double && !noPreviousPointers) off_next = LegacyPointer.Read(reader); // previous element pointer
                        if (hasHeaderPointers) LegacyPointer.Read(reader); // header struct pointer
                    }
                    if (readAtPointer && !elementPointerFirst) off_element = LegacyPointer.Read(reader);
                    // Read element
                    if (!readAtPointer) {
                        list[i] = readElement(off_element);
                    } else {
                        LegacyPointer.DoAt(ref reader, off_element, () => {
                            list[i] = readElement(off_element);
                        });
                    }

                    // Goto next element
                    if (customEntries) {
                        off_next = ((ILinkedListEntry)list[i]).PreviousEntry;
                    }
                    if (i < num_elements - 1 && (customEntries || (type != Type.SingleNoElementPointers && type != Type.DoubleNoElementPointers))) {
                        if (off_next == null) {
                            num_elements = (uint)i + 1;
                            break;
                        }
                        LegacyPointer.Goto(ref reader, off_next);
                    } else {
                        off_next = LegacyPointer.Current(reader);
                    }
                }
                LegacyPointer.Goto(ref reader, off_current);
            }
        }

        public static LinkedList<T> Read(ref Reader reader, LegacyPointer offset, ReadElement readElement,
            LinkedList.Flags flags = LinkedList.Flags.None,
            Type type = Type.Default) {
            LinkedList<T> li = ReadHeader(reader, offset, type: type);
            li.ReadEntries(ref reader, readElement, flags: flags);
            return li;
        }

        public void FillPointers(Reader reader, LegacyPointer lastEntry, LegacyPointer header, uint nextOffset = 0, uint prevOffset = 4, uint headerOffset = 8) {
            LegacyPointer current_entry = lastEntry;
            LegacyPointer next_entry = null;
            LegacyPointer off_current = LegacyPointer.Current(reader);
            while (current_entry != null) {
                LegacyPointer.Goto(ref reader, current_entry);
                current_entry.file.AddPointer(current_entry.offset + nextOffset, next_entry);
                if (header != null) {
                    current_entry.file.AddPointer(current_entry.offset + headerOffset, header);
                }
                next_entry = current_entry;
                current_entry = LegacyPointer.GetPointerAtOffset(current_entry + prevOffset);
            }
            LegacyPointer.Goto(ref reader, off_current);
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
			Count = Count + 1;
        }

        void ICollection<T>.Clear() {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item) {
            throw new NotImplementedException();
        }
    }
}
