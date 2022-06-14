namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class FON_LanguagesCount : U64_Struct {
		public ushort LanguagesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LanguagesCount = s.Serialize<ushort>(LanguagesCount, name: nameof(LanguagesCount));
		}
	}
}
