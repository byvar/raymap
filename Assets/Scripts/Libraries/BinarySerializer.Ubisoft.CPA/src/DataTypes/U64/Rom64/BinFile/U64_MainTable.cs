namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_MainTable : BinarySerializable {
		public Pointer DataOffset { get; set; }
		public uint DataLength { get; set; }

		public Pointer[] StructTable { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DataOffset = s.SerializePointer(DataOffset, name: nameof(DataOffset));
			DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));
		}

		public void SerializeStructTable(SerializerObject s) {
			s.DoAt(DataOffset, () => {
				StructTable = s.SerializePointerArray(StructTable, DataLength / 4, name: nameof(StructTable));
			});
		}
	}
}