namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_DynamicList<T> : LST2_List<T> where T : BinarySerializable, ILST2_DynamicEntry<T>, new() {
		public LST2_DynamicList<T> Resolve(SerializerObject s, string name = null) {
			ResolveElements(s, name: name);
			if(ElementList != null)
				foreach(var el in ElementList) el?.LST2_Parent?.ResolveObject(s);
			return this;
		}

		public override void Configure(Context c) {
			Type = LST2_ListType.DoubleLinked;
		}
	}
}