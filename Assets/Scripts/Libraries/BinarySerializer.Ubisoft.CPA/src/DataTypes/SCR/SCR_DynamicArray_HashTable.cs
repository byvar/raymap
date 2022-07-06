namespace BinarySerializer.Ubisoft.CPA {
	public class SCR_DynamicArray_HashTable : BinarySerializable {
		public SCR_DynamicArray_Description[] HashTable { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			HashTable = s.SerializeObjectArray<SCR_DynamicArray_Description>(HashTable, 256, name: nameof(HashTable));
		}
	}
}
