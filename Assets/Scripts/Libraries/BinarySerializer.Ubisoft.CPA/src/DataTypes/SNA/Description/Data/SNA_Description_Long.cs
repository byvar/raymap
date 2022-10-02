namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Long : SNA_Description_Data, ISerializerShortLog {
		public int Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.Serialize<int>(Value, name: nameof(Value));
		}

		public string ShortLog => Value.ToString();
	}
}
