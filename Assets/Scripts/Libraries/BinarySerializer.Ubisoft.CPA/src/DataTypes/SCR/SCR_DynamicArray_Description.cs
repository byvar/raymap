namespace BinarySerializer.Ubisoft.CPA {
	public class SCR_DynamicArray_Description : BinarySerializable {
		public Pointer DynamicArray { get; set; } // Pointer to an element
		public uint ValuesCount { get; set; }
		public uint MaxValuesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DynamicArray = s.SerializePointer(DynamicArray, name: nameof(DynamicArray));
			ValuesCount = s.Serialize<uint>(ValuesCount, name: nameof(ValuesCount));
			MaxValuesCount = s.Serialize<uint>(MaxValuesCount, name: nameof(MaxValuesCount));
		}
	}
}
