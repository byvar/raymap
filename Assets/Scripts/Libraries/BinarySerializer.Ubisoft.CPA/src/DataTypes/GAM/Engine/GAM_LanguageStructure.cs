namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_LanguageStructure : BinarySerializable {
		public string LanguageCode { get; set; }
		public string LanguageText { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LanguageCode = s.SerializeString(LanguageCode, length: 20, name: nameof(LanguageCode));
			LanguageText = s.SerializeString(LanguageText, length: 20, name: nameof(LanguageText));
		}
	}
}
