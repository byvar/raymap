using System;
using System.Collections;
using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public class LST2_List<T> : BinarySerializable, IList<T> where T : BinarySerializable, LST2_IEntry<T>, new() {
		#region Constructors

		public LST2_List() { }

		public LST2_List(LST2_ListType type) {
			Type = type;
		}

		public LST2_List(Context context, Pointer<T> head, Pointer<T> tail, uint elementsCount, LST2_ListType type) {
			Context = context;
			Head = head;
			Tail = tail;
			ElementsCount = elementsCount;
			list = new T[elementsCount];

			Type = type;
		}

		public LST2_List(Context context, Pointer<T> head, uint num_elements, LST2_ListType type) :
			this(context, head, null, num_elements, type) { }

		#endregion

		#region Properties

		// Serialized
		public Pointer<T> Head { get; set; }
		public Pointer<T> Tail { get; set; }
		public uint ElementsCount { get; set; }

		public LST2_ListType Type { get; set; }
		public uint ActualElementsCount { get; set; }

		#endregion

		#region Public Methods
		public void Configure(Context c) {
			switch (Type) {
				case LST2_ListType.Dynamic:
					Type = LST2_ListType.DoubleLinked;
					break;
				case LST2_ListType.Static:
					Type = c.GetCPASettings().StaticListType;
					break;
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			Configure(s.Context);
			Head = s.SerializePointer<T>(Head, name: nameof(Head));
			if (Type == LST2_ListType.DoubleLinked)
				Tail = s.SerializePointer<T>(Tail, name: nameof(Tail));

			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			ActualElementsCount = ElementsCount;
			list = new T[ActualElementsCount];
		}

		public LST2_List<T> Resolve(SerializerObject s, string name = null) {
			if (Type == LST2_ListType.Array || Type == LST2_ListType.Optimized) {
				s.DoAt(Head, () => {
					list = s.SerializeObjectArray<T>(list, ActualElementsCount, name: name);
				});
			} else {
				Pointer<T> Element = Head;
				uint elementIndex = 0;
				while (Element != null && elementIndex < ActualElementsCount) {
					// Just so we can use the name, we do DoAt first
					s.DoAt(Element?.PointerValue, () => {
						list[elementIndex] = s.SerializeObject<T>(list[elementIndex], name: $"{name}[{elementIndex}]");
					});
					Element.Value = list[elementIndex];
					Element?.Value?.LST2_Previous?.ResolveObject(s); // Resolve previous pointer if it exists

					list[elementIndex] = Element?.Value;
					elementIndex++;
					if (Type == LST2_ListType.DoubleLinked || Type == LST2_ListType.SingleLinked || Type == LST2_ListType.SemiOptimized) {
						Element = Element?.Value?.LST2_Next;
						if (Element == null) {
							ActualElementsCount = elementIndex;
						}
					}
				}
			}
			return this;
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
