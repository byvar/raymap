namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Section : SNA_Description_Data {
		public SNA_Description_Item[] Items { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Items = s.SerializeObjectArrayUntil<SNA_Description_Item>(Items,
				i => 
				(s.CurrentAbsoluteOffset >= s.CurrentLength-3
				|| i.Type == SNA_DescriptionType.EndOfDescSection),
				onPreSerialize: (i,_) => i.Pre_ParentData = this,
				name: nameof(Items));
		}
	}
}
