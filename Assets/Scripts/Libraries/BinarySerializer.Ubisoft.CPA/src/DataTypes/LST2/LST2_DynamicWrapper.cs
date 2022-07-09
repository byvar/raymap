namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_DynamicWrapper<T> : BinarySerializable, LST2_IEntry<LST2_DynamicWrapper<T>> where T : BinarySerializable, new() {
		public LST2_DynamicListElement<LST2_DynamicWrapper<T>> Element { get; set; }
		public T Value { get; set; }

		public Pointer<LST2_DynamicWrapper<T>> LST2_Next => ((LST2_IEntry<LST2_DynamicWrapper<T>>)Element).LST2_Next;
		public Pointer<LST2_DynamicWrapper<T>> LST2_Previous => ((LST2_IEntry<LST2_DynamicWrapper<T>>)Element).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			Element = s.SerializeObject<LST2_DynamicListElement<LST2_DynamicWrapper<T>>>(Element, name: nameof(Element));
			Value = s.SerializeObject<T>(Value, name: nameof(Value));
		}
	}
}