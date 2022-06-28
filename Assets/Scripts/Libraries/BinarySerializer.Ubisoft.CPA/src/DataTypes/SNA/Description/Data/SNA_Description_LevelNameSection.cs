namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_LevelNameSection : SNA_Description_Section {
		public int CurrentLevel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			CurrentLevel = s.Serialize<int>(CurrentLevel, name: nameof(CurrentLevel));
			base.SerializeImpl(s);
		}
	}
}
