namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_DynamicListElement<T> : LST2_ListElement<T>, ILST2_DynamicEntry<T>
		where T : BinarySerializable, ILST2_DynamicEntry<T>, new() {
		public Pointer<LST2_DynamicList<T>> Father { get; set; }
		public Pointer<LST2_DynamicList<T>> LST2_Parent => Father;

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			if (HasFather) Father = s.SerializePointer<LST2_DynamicList<T>>(Father, name: nameof(Father));
		}

		public LST2_DynamicListElement<T> Resolve(SerializerObject s) {
			ResolveSiblings(s);
			Father?.ResolveObject(s);
			return this;
		}
		public override void Configure(Context c) {
			Type = LST2_ListType.DoubleLinked;
		}
	}
}