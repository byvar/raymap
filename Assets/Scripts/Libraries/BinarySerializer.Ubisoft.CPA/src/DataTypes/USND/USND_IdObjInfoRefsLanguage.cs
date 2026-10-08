namespace BinarySerializer.Ubisoft.CPA {
	public class USND_IdObjInfoRefsLanguage : BinarySerializable {
		public string LanguageCode { get; set; }
		public uint ReferencesCount { get; set; }
		public USND_UUID[] References { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LanguageCode = s.SerializeString(LanguageCode, length: 4, name: nameof(LanguageCode));
			ReferencesCount = s.Serialize<uint>(ReferencesCount, name: nameof(ReferencesCount));
			References = s.SerializeObjectArray<USND_UUID>(References, ReferencesCount, name: nameof(References));
		}
	}
}