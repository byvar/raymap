namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_RandomDesc : SNA_Description_Section {
		public int SizeOfTable { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SizeOfTable = s.Serialize<int>(SizeOfTable, name: nameof(SizeOfTable));
			base.SerializeImpl(s);
		}
	}
}
