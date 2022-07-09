namespace BinarySerializer.Ubisoft.CPA {
	public class FON_General : BinarySerializable {
		public uint ElapsedTime { get; set; } // Both of these were used for text effects, but are no longer used in R2
		public uint RandomIndex { get; set; }

		public ushort LanguagesCount { get; set; }
		public Pointer<FON_Language[]> Languages { get; set; }
		public Pointer<FON_Language> CommonLanguage { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElapsedTime = s.Serialize<uint>(ElapsedTime, name: nameof(ElapsedTime));
			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution))
				RandomIndex = s.Serialize<uint>(RandomIndex, name: nameof(RandomIndex));

			LanguagesCount = s.Serialize<ushort>(LanguagesCount, name: nameof(LanguagesCount));
			s.Align(4, Offset);
			Languages = s.SerializePointer<FON_Language[]>(Languages, name: nameof(Languages))?.ResolveObjectArray(s, LanguagesCount);
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet)) {
				CommonLanguage = s.SerializePointer<FON_Language>(CommonLanguage, name: nameof(CommonLanguage))?.ResolveObject(s);
			}
		}
	}
}
