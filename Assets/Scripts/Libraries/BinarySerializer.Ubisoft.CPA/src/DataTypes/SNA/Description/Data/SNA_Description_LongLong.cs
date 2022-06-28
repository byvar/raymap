namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_LongLong : SNA_Description_Data {
		public int Value0 { get; set; }
		public int Value1 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value0 = s.Serialize<int>(Value0, name: nameof(Value0));
			Value1 = s.Serialize<int>(Value1, name: nameof(Value1));
		}
	}
}
