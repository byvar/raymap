namespace BinarySerializer.Ubisoft.CPA {
	public class SCR_DynamicArray_Header : BinarySerializable {
		public uint Index { get; set; } // Index in array
		public byte MemoryLevel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Index = s.Serialize<uint>(Index, name: nameof(Index));
			MemoryLevel = s.Serialize<byte>(MemoryLevel, name: nameof(MemoryLevel));
			s.Align(4, Offset);
		}
	}
}
