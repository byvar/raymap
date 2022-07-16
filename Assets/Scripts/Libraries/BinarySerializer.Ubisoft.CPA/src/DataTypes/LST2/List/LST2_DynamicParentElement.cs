namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_DynamicParentElement<T, U> : LST2_List<T>
		where T : BinarySerializable, ILST2_Child<T, U>, new()
		where U : BinarySerializable, new() {
		public LST2_DynamicParentElement<T, U> Resolve(SerializerObject s, string name = null) {
			base.ResolveElements(s, name: name);
			if (ElementList != null)
				foreach (var el in ElementList) el?.LST2_Parent?.ResolveObject(s);
			return this;
		}

		public override void Configure(Context c) {
			Type = LST2_ListType.DoubleLinked;
		}
	}
}