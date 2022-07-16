namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_StaticList<T> : LST2_List<T> where T : BinarySerializable, ILST2_StaticEntry<T>, new()
	{
		public LST2_StaticList<T> Resolve(SerializerObject s, string name = null) {
			ResolveElements(s, name: name);
			if (ElementList != null)
				foreach (var el in ElementList) el?.LST2_Parent?.ResolveObject(s);
			return this;
		}
		public override void Configure(Context c) {
			Type = c.GetCPASettings().StaticListType;
		}
	}
}