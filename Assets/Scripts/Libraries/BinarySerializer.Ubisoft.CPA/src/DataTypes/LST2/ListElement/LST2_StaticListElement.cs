namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_StaticListElement<T> : LST2_ListElement<T>, ILST2_StaticEntry<T>
		where T : BinarySerializable, ILST2_StaticEntry<T>, new() {
		public Pointer<LST2_StaticList<T>> Father { get; set; }
		public Pointer<LST2_StaticList<T>> LST2_Parent => Father;

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			if (HasFather) Father = s.SerializePointer<LST2_StaticList<T>>(Father, name: nameof(Father));
		}
		public LST2_StaticListElement<T> Resolve(SerializerObject s) {
			ResolveSiblings(s);
			Father?.ResolveObject(s);
			return this;
		}
		public override void Configure(Context c) {
			Type = c.GetCPASettings().StaticListType;
		}
	}
}