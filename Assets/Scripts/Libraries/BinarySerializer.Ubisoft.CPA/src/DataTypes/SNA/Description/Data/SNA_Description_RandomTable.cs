namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_RandomTable : SNA_Description_Data {
		public SNA_Description_RandomDesc Pre_RandomDesc { get; set; }
		public int SizeOfTable => Pre_RandomDesc.SizeOfTable;

		public uint[] Table { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Table = s.SerializeArray<uint>(Table, SizeOfTable, name: nameof(Table));
		}
	}
}
