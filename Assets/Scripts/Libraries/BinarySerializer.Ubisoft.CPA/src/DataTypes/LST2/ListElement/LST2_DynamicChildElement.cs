namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_DynamicChildElement<T, U> : LST2_ListElement<T>
		where T : BinarySerializable, ILST2_Child<T, U>, new()
		where U : BinarySerializable, new()
	{
		public Pointer<U> Father { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			if (HasFather) Father = s.SerializePointer<U>(Father, name: nameof(Father));
		}

		public LST2_DynamicChildElement<T, U> Resolve(SerializerObject s) {
			ResolveSiblings(s);
			Father?.ResolveObject(s);
			return this;
		}
		public override void Configure(Context c) {
			Type = LST2_ListType.DoubleLinked;
		}
	}
}