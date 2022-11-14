namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_String : SNA_Description_Data, ISerializerShortLog {
		public ushort Length { get; set; }
		public string Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_Montreal)) {
				Length = s.Serialize<byte>((byte)Length, name: nameof(Length));
			} else {
				Length = s.Serialize<ushort>(Length, name: nameof(Length));
			}
			Value = s.SerializeString(Value, length: Length, name: nameof(Value));
		}

		public string ShortLog => Value;
	}
}
