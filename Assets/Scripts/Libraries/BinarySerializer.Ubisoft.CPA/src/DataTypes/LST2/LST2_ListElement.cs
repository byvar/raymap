namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_ListElement<T> : BinarySerializable, LST2_IEntry<T> where T : BinarySerializable, LST2_IEntry<T>, new() {
		#region Constructors

		public LST2_ListElement() { }

		public LST2_ListElement(LST2_ListType type) {
			Type = type;
		}

		public LST2_ListElement(Context context, Pointer<T> next, Pointer<T> previous, Pointer<LST2_List<T>> father, LST2_ListType type) {
			Context = context;
			NextBrother = next;
			PreviousBrother = previous;
			Father = father;

			Type = type;
		}

		public LST2_ListElement(Context context, Pointer<T> next, LST2_ListType type) :
			this(context, next, null, null, type) { }

		#endregion


		public Pointer<T> NextBrother { get; set; }
		public Pointer<T> PreviousBrother { get; set; }
		public Pointer<LST2_List<T>> Father { get; set; }

		public LST2_ListType Type { get; set; }

		public Pointer<T> LST2_Next => NextBrother;
		public Pointer<T> LST2_Previous => PreviousBrother;

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
			if (Type != LST2_ListType.Array && Type != LST2_ListType.Optimized) {
				NextBrother = s.SerializePointer<T>(NextBrother, name: nameof(NextBrother));
				if (Type != LST2_ListType.SingleLinked) {
					PreviousBrother = s.SerializePointer<T>(PreviousBrother, name: nameof(PreviousBrother));
					Father = s.SerializePointer<LST2_List<T>>(Father, name: nameof(Father));
				}
			}

			Father?.ResolveObject(s);
			NextBrother?.ResolveObject(s);
			PreviousBrother?.ResolveObject(s);
		}
	}
}