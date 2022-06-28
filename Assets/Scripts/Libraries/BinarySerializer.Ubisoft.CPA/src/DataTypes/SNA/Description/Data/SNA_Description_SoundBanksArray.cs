namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_SoundBanksArray : SNA_Description_Data {
		public int Count { get; set; }
		public int[] SoundBankIndices { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<int>(Count, name: nameof(Count));
			SoundBankIndices = s.SerializeArray<int>(SoundBankIndices, Count, name: nameof(SoundBankIndices));
		}
	}
}
