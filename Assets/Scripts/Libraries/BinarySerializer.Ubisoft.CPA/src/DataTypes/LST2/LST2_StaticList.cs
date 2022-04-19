namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_StaticList<T> : LST2_List<T> where T : BinarySerializable, LST2_IEntry<T>, new()
	{
		public LST2_StaticList() : base(LST2_ListType.Static) { }
	}
}