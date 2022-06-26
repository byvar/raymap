namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description : BinarySerializable {
		public Section[] Sections { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Sections = s.SerializeObjectArrayUntil<Section>(Sections, _ => s.CurrentAbsoluteOffset >= s.CurrentLength-3, name: nameof(Sections));
		}

		public class Section : BinarySerializable {
			public SNA_DescriptionType SectionMark { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				SectionMark = s.Serialize<SNA_DescriptionType>(SectionMark, name: nameof(SectionMark));
				throw new System.NotImplementedException();
			}
		}
	}
}
