namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Montreal_Game : BinarySerializable, SNA_IDescription {
		public SNA_Description_String[] Directories { get; set; }
		public SNA_Description_String BigFileVignettes { get; set; }
		public SNA_Description_String BigFileTextures { get; set; }
		public uint RandomTableSize { get; set; }
		public byte UseA3b { get; set; }
		public byte UseBinaryMod { get; set; }
		public SNA_Description_String GameOptionsDefaultFile { get; set; }
		public SNA_Description_String GameOptionsCurrentFile { get; set; }
		public SNA_Description_String FirstLevelName { get; set; }

		public int DirectoryIndex(SNA_DescriptionType directoryType) {
			return Context.GetCPASettings().SNATypes.GetDescriptionInt(directoryType) - 0x10000;
		}
		public SNA_DescriptionType DirectoryType(int directoryIndex) {
			return Context.GetCPASettings().SNATypes.GetDescriptionType(directoryIndex + 0x10000);
		}

		public override void SerializeImpl(SerializerObject s) {
			Directories = s.SerializeObjectArray<SNA_Description_String>(Directories, 10, name: nameof(Directories));
			BigFileVignettes = s.SerializeObject<SNA_Description_String>(BigFileVignettes, name: nameof(BigFileVignettes));
			BigFileTextures = s.SerializeObject<SNA_Description_String>(BigFileTextures, name: nameof(BigFileTextures));
			RandomTableSize = s.Serialize<uint>(RandomTableSize, name: nameof(RandomTableSize));
			UseA3b = s.Serialize<byte>(UseA3b, name: nameof(UseA3b));
			UseBinaryMod = s.Serialize<byte>(UseBinaryMod, name: nameof(UseBinaryMod));
			GameOptionsDefaultFile = s.SerializeObject<SNA_Description_String>(GameOptionsDefaultFile, name: nameof(GameOptionsDefaultFile));
			GameOptionsCurrentFile = s.SerializeObject<SNA_Description_String>(GameOptionsCurrentFile, name: nameof(GameOptionsCurrentFile));
			FirstLevelName = s.SerializeObject<SNA_Description_String>(FirstLevelName, name: nameof(FirstLevelName));
		}

		public string GetDirectory(SNA_DescriptionType type) {
			var ind = DirectoryIndex(type);

			return Directories[ind]?.Value;
		}
	}
}
