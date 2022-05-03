namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_FixPreloadSection : U64_Struct {
		public LST_ReferenceList<GAM_Character> Sections { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Sections = s.SerializeObject<LST_ReferenceList<GAM_Character>>(Sections, name: nameof(Sections))?.Resolve(s);
		}
	}
}
