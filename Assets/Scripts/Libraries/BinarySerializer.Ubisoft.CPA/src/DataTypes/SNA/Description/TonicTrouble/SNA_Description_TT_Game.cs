namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_TT_Game : SNA_Description_TT_Level {
		public SNA_Description_String[] Directories { get; set; }
		public SNA_Description_Section BigFiles { get; set; }
		public SNA_Description_String LoadVignette { get; set; }
		public SNA_Description_String InputDeviceFile { get; set; }
		public SNA_Description_TT_Text Text { get; set; }
		public SNA_Description_Section GameOptionsFile { get; set; }
		public SNA_Description_String Unknown { get; set; } // First level index?
		public SNA_Description_String FirstLevelName { get; set; }

		public int DirectoryIndex(SNA_DescriptionType directoryType) {
			return Context.GetCPASettings().SNATypes.GetInt(directoryType) - 0x10000;
		}
		public SNA_DescriptionType DirectoryType(int directoryIndex) {
			return Context.GetCPASettings().SNATypes.GetType(directoryIndex + 0x10000);
		}

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			Directories = s.SerializeObjectArray<SNA_Description_String>(Directories, 30, name: nameof(Directories));
			BigFiles = s.SerializeObject<SNA_Description_Section>(BigFiles, name: nameof(BigFiles));
			LoadVignette = s.SerializeObject<SNA_Description_String>(LoadVignette, name: nameof(LoadVignette));
			InputDeviceFile = s.SerializeObject<SNA_Description_String>(InputDeviceFile, name: nameof(InputDeviceFile));
			Text = s.SerializeObject<SNA_Description_TT_Text>(Text, name: nameof(Text));
			GameOptionsFile = s.SerializeObject<SNA_Description_Section>(GameOptionsFile, name: nameof(GameOptionsFile));
			Unknown = s.SerializeObject<SNA_Description_String>(Unknown, name: nameof(Unknown));
			FirstLevelName = s.SerializeObject<SNA_Description_String>(FirstLevelName, name: nameof(FirstLevelName));
		}
	}
}
