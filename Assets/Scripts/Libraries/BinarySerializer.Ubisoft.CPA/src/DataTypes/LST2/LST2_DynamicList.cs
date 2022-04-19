namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_DynamicList<T> : LST2_List<T> where T : BinarySerializable, LST2_IEntry<T>, new()
	{
		public LST2_DynamicList() : base(LST2_ListType.Dynamic) { }
	}
}