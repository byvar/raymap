namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_EngineStructure : BinarySerializable {
		public GAM_EngineMode EngineMode { get; set; }
		public string LevelName { get; set; }
		public string NextLevelName { get; set; }
		public string FirstLevelName { get; set; }
		public string[] SubLevelNames { get; set; }
		public string CreditsLevelName { get; set; }
		public string NextLevelPositionPersoName { get; set; }
		public string NextLevelPositionCameraName { get; set; }

		private int LevelNameSize =>
			Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2) ? 30 : 260;
		private int MaxSubLevelsCount => 2;

		public override void SerializeImpl(SerializerObject s) {
			EngineMode = s.Serialize<GAM_EngineMode>(EngineMode, name: nameof(EngineMode));
			LevelName = s.SerializeString(LevelName, length: LevelNameSize, name: nameof(LevelName));
			NextLevelName = s.SerializeString(NextLevelName, length: LevelNameSize, name: nameof(NextLevelName));
			FirstLevelName = s.SerializeString(FirstLevelName, length: LevelNameSize, name: nameof(FirstLevelName));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				SubLevelNames = s.SerializeStringArray(SubLevelNames, MaxSubLevelsCount, length: LevelNameSize, name: nameof(SubLevelNames));
				CreditsLevelName = s.SerializeString(CreditsLevelName, length: LevelNameSize, name: nameof(CreditsLevelName));
				NextLevelPositionPersoName = s.SerializeString(NextLevelPositionPersoName, length: 50, name: nameof(NextLevelPositionPersoName));
				NextLevelPositionCameraName = s.SerializeString(NextLevelPositionCameraName, length: 50, name: nameof(NextLevelPositionCameraName));
				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
			}
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
