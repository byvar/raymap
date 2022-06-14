namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class FON_LanguageString : U64_Struct {
		public LST_ReferenceList<FON_TextString> StringList { get; set; }
		public LST_ReferenceList<FON_StringLength> StringLengthList { get; set; }
		public ushort Language { get; set; }
		public string LanguageName { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			StringList = s.SerializeObject<LST_ReferenceList<FON_TextString>>(StringList, name: nameof(StringList))?.Resolve(s);
			StringLengthList = s.SerializeObject<LST_ReferenceList<FON_StringLength>>(StringLengthList, name: nameof(StringLengthList))?.Resolve(s);
			Language = s.Serialize<ushort>(Language, name: nameof(Language));
			LanguageName = s.SerializeString(LanguageName, 18, name: nameof(LanguageName));
		}
	}
}
