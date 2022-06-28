namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description : BinarySerializable {
		public SNA_Description_Item[] Items { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Items = s.SerializeObjectArrayUntil<SNA_Description_Item>(Items,
				i =>
				(s.CurrentAbsoluteOffset >= s.CurrentLength - 3
				|| i.Type == SNA_DescriptionType.EndOfDescSection)
				, name: nameof(Items));
		}
	}
}
