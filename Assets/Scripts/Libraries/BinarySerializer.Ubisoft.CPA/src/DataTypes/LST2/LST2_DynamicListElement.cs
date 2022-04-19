namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_DynamicListElement<T> : LST2_ListElement<T> where T : BinarySerializable, LST2_IEntry<T>, new()
	{
		public LST2_DynamicListElement() : base(LST2_ListType.Dynamic) { }
	}
}