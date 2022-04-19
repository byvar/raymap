namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_StaticListElement<T> : LST2_ListElement<T> where T : BinarySerializable, LST2_IEntry<T>, new()
	{
		public LST2_StaticListElement() : base(LST2_ListType.Static) { }
	}
}