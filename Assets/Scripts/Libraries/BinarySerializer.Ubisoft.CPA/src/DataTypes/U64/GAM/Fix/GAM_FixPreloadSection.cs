namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_FixPreloadSection : U64_Struct {
		public U64_ArrayReference<LST_Ref<GAM_Character>> Sections { get; set; }
		public ushort SectionsCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Sections = s.SerializeObject<U64_ArrayReference<LST_Ref<GAM_Character>>>(Sections, name: nameof(Sections));
			SectionsCount = s.Serialize<ushort>(SectionsCount, name: nameof(SectionsCount));

			// Resolve references
			Sections.Resolve(s, SectionsCount);
		}
	}
}
