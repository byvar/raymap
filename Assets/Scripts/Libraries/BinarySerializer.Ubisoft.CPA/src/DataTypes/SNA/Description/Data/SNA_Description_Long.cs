namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Long : SNA_Description_Data {
		public int Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.Serialize<int>(Value, name: nameof(Value));
		}

		public override string ShortLog => Value.ToString();
		public override bool UseShortLog => true;
	}
}
