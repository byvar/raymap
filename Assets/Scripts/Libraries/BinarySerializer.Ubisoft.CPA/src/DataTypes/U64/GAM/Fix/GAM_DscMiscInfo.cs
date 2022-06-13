namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_DscMiscInfo : U64_Struct {
		public string FirstLevelName { get; set; }
		/*public string CreditsLevelName { get; set; }
		public bool SkipMainMenu { get; set; }*/

		public override void SerializeImpl(SerializerObject s) {
			FirstLevelName = s.SerializeString(FirstLevelName, 32, name: nameof(FirstLevelName));
			/*CreditsLevelName = s.SerializeString(CreditsLevelName, 32, name: nameof(CreditsLevelName));
			SkipMainMenu = s.Serialize<bool>(SkipMainMenu, name: nameof(SkipMainMenu));
			s.SerializePadding(1, logIfNotNull: true);*/
		}
	}
}
