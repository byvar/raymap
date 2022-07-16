using System;
using System.Collections;
using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class LST2_List<T> : BinarySerializable, IList<T>
		where T : BinarySerializable, ILST2_Entry<T>, new() {
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
			ElementList = new T[elementsCount];

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
		public abstract void Configure(Context c);

		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
			Configure(s.Context);
		}

		public override void SerializeImpl(SerializerObject s) {
			Head = s.SerializePointer<T>(Head, name: nameof(Head));
			if (Type == LST2_ListType.DoubleLinked)
				Tail = s.SerializePointer<T>(Tail, name: nameof(Tail));

			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			ActualElementsCount = ElementsCount;
			ElementList = new T[ActualElementsCount];
		}

		protected void ResolveElements(SerializerObject s, string name = null) {
			if (Type == LST2_ListType.Array || Type == LST2_ListType.Optimized) {
				s.DoAt(Head, () => {
					ElementList = s.SerializeObjectArray<T>(ElementList, ActualElementsCount, name: name);
				});
			} else {
				Pointer<T> Element = Head;
				uint elementIndex = 0;
				while (Element != null && elementIndex < ActualElementsCount) {
					// Just so we can use the name, we do DoAt first
					s.DoAt(Element?.PointerValue, () => {
						ElementList[elementIndex] = s.SerializeObject<T>(ElementList[elementIndex], name: $"{name}[{elementIndex}]");
					});
					Element.Value = ElementList[elementIndex];
					Element?.Value?.LST2_Previous?.ResolveObject(s); // Resolve previous pointer if it exists

					ElementList[elementIndex] = Element?.Value;
					elementIndex++;
					if (Type == LST2_ListType.DoubleLinked || Type == LST2_ListType.SingleLinked || Type == LST2_ListType.SemiOptimized) {
						Element = Element?.Value?.LST2_Next;
						if (Element == null) {
							ActualElementsCount = elementIndex;
						}
					}
				}
			}
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

		protected T[] ElementList = null;

		public int Count {
			get { return (int)ElementsCount; }
			set {
				ElementsCount = (uint)value;
				if (ElementList.Length != ElementsCount) {
					Array.Resize(ref ElementList, (int)ElementsCount);
				}
			}
		}

		public bool IsReadOnly {
			get { return ElementList.IsReadOnly; }
		}

		public T this[int index] {
			get { return ElementList[index]; }
			set { ElementList[index] = value; }
		}

		public IEnumerator<T> GetEnumerator() {
			return ((IEnumerable<T>)ElementList).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<T>)ElementList).GetEnumerator();
		}

		public int IndexOf(T item) {
			return Array.IndexOf(ElementList, item);
		}

		public bool Contains(T item) {
			return Array.IndexOf(ElementList, item) >= 0;
		}

		public void CopyTo(T[] array, int arrayIndex) {
			ElementList.CopyTo(array, arrayIndex);
		}

		void IList<T>.Insert(int index, T item) {
			throw new NotImplementedException();
		}

		void IList<T>.RemoveAt(int index) {
			throw new NotImplementedException();
		}

		public void Add(T item) {
			Array.Resize(ref ElementList, ElementList.Length + 1);
			ElementList[ElementList.Length - 1] = item;
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
