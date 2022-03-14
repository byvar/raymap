using System;
using System.Collections;
using System.Collections.Generic;
using Type = BinarySerializer.Ubisoft.CPA.CPA_LinkedList.Type;
using Flags = BinarySerializer.Ubisoft.CPA.CPA_LinkedList.Flags;

namespace BinarySerializer.Ubisoft.CPA {
    public static class CPA_LinkedList {
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

    public class CPA_LinkedList<T> : BinarySerializable, IList<T> 
    {
	    #region Constructors

	    public CPA_LinkedList() { }

	    public CPA_LinkedList(Type type = Type.Default, Flags flags = Flags.None)
	    {
		    Type = type;
		    Flags = flags;
	    }

		public CPA_LinkedList(Context context, Pointer head, Pointer tail, uint elementsCount, Type type = Type.Default)
	    {
		    Context = context;
		    Head = head;
		    Tail = tail;
		    ElementsCount = elementsCount;
		    list = new T[elementsCount];

		    if (type == Type.Default)
		    {
			    type = Context.GetCPASettings().LinkedListType;

			    if (type == Type.Minimize)
				    type = Type.Single;
		    }
		    else if (type == Type.Minimize)
		    {
			    /* Minimize works as follows. A linkedlist with type minimize is a default one,
                but if the default type is also Minimize (RA GC, R2 DC) then it is a SingleNoElementPointers list (i.e. optimized to an array).
                If the list itself does not specify the minimize type, it is read as a default one,
                but if the default type is Minimize then it becomes a Single list (i.e. not an array, but no previous pointers).
                */
			    type = Context.GetCPASettings().LinkedListType;

			    if (type == Type.Minimize)
				    type = Type.SingleNoElementPointers;
		    }

		    Type = type;
	    }

	    public CPA_LinkedList(Context context, Pointer head, uint num_elements, Type type = Type.Default) : 
		    this(context, head, null, num_elements, type) { }

		#endregion

		#region Properties

		// Serialized
		public Pointer Head { get; set; }
		public Pointer Tail { get; set; }
		public uint ElementsCount { get; set; }
		//public Pointer Header { get; set; }

		public Type Type { get; set; } = Type.Default;
		public Flags Flags { get; set; }
		//private bool customEntries => typeof(ILinkedListEntry).IsAssignableFrom(typeof(T));

		#endregion

		#region Public Methods

		// Call in OnPreSerialize
		public void Configure(Type type = Type.Default, Flags flags = Flags.None)
		{
			Type = type;
			Flags = flags;
		}

		public override void SerializeImpl(SerializerObject s)
		{
			if (Type == Type.Default)
			{
				Type = Context.GetCPASettings().LinkedListType;

				if (Type == Type.Minimize) 
					Type = Type.Single;
			}
			else if (Type == Type.Minimize)
			{
				/* Minimize works as follows. A linkedlist with type minimize is a default one,
                but if the default type is also Minimize (RA GC, R2 DC) then it is a SingleNoElementPointers list (i.e. optimized to an array).
                If the list itself does not specify the minimize type, it is read as a default one,
                but if the default type is Minimize then it becomes a Single list (i.e. not an array, but no previous pointers).
                */
				Type = Context.GetCPASettings().LinkedListType;
				if (Type == Type.Minimize)
				{
					Type = Type.SingleNoElementPointers;
				}
			}
			Head = s.SerializePointer(Head, name: nameof(Head));
			if (Type == Type.Double || Type == Type.DoubleNoElementPointers)
			{
				Tail = s.SerializePointer(Tail, name: nameof(Tail));
			}
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			list = new T[ElementsCount];
		}

		#endregion

		// TODO: Read entries implementation
		/*
        public delegate T ReadElement(LegacyPointer offset);
        public LinkedList<T> SerializeEntries(SerializerObject s, ReadElement readElement) {
            Pointer nextPointer = Head;
            bool elementPointerFirst = ((flags & LinkedList.Flags.ElementPointerFirst) != 0);
            bool hasHeaderPointers = ((flags & LinkedList.Flags.HasHeaderPointers) != 0);
            bool readAtPointer = ((flags & LinkedList.Flags.ReadAtPointer) != 0);
            bool noPreviousPointers = ((flags & LinkedList.Flags.NoPreviousPointersForDouble) != 0);
            if (nextPointer != null) {
                s.DoAt(Head, () => {
                    for (int i = 0; i < ElementsCount; i++) {
                        Pointer off_element = nextPointer;
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
                });
            }
            return this;
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
        */

		#region IList implementation

		private T[] list = null;

        public int Count {
            get { return (int)ElementsCount; }
            set {
                ElementsCount = (uint)value;
                if (list.Length != ElementsCount) {
                    Array.Resize(ref list, (int)ElementsCount);
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

		#endregion
	}
}
