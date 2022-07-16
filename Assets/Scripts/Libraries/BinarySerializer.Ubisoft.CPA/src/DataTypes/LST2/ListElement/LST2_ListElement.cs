namespace BinarySerializer.Ubisoft.CPA
{
	public abstract class LST2_ListElement<T> : BinarySerializable, ILST2_Entry<T>
		where T : BinarySerializable, ILST2_Entry<T>, new() {
		#region Constructors

		public LST2_ListElement() { }

		public LST2_ListElement(LST2_ListType type) {
			Type = type;
		}

		public LST2_ListElement(Context context, Pointer<T> next, Pointer<T> previous, LST2_ListType type) {
			Context = context;
			NextBrother = next;
			PreviousBrother = previous;

			Type = type;
		}

		public LST2_ListElement(Context context, Pointer<T> next, LST2_ListType type) :
			this(context, next, null, type) { }

		#endregion


		public Pointer<T> NextBrother { get; set; }
		public Pointer<T> PreviousBrother { get; set; }

		public LST2_ListType Type { get; set; }

		public Pointer<T> LST2_Next => NextBrother;
		public Pointer<T> LST2_Previous => PreviousBrother;

		public abstract void Configure(Context c);

		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
			Configure(s.Context);
		}
		public override void SerializeImpl(SerializerObject s) {
			if (Type != LST2_ListType.Array && Type != LST2_ListType.Optimized) {
				NextBrother = s.SerializePointer<T>(NextBrother, name: nameof(NextBrother));
				if (Type != LST2_ListType.SingleLinked) {
					PreviousBrother = s.SerializePointer<T>(PreviousBrother, name: nameof(PreviousBrother));
				}
			}
		}

		protected bool HasFather => Type != LST2_ListType.Array && Type != LST2_ListType.Optimized && Type != LST2_ListType.SingleLinked;

		protected void ResolveSiblings(SerializerObject s) {
			NextBrother?.ResolveObject(s);
			PreviousBrother?.ResolveObject(s);
		}
	}
}