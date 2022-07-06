namespace BinarySerializer.Ubisoft.CPA {
	public class SCR_LinkTable : BinarySerializable {
		public SCR_DynamicArray_Header Header { get; set; }
		public SCR_DynamicArray_Description LinkArray { get; set; }
		public SCR_DynamicArray_HashTable Values { get; set; }
		public SCR_DynamicArray_HashTable Keys { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Header = s.SerializeObject<SCR_DynamicArray_Header>(Header, name: nameof(Header));
			LinkArray = s.SerializeObject<SCR_DynamicArray_Description>(LinkArray, name: nameof(LinkArray));
			Values = s.SerializeObject<SCR_DynamicArray_HashTable>(Values, name: nameof(Values));
			Keys = s.SerializeObject<SCR_DynamicArray_HashTable>(Keys, name: nameof(Keys));
		}
	}
}
