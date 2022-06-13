namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_LevelsNameList : U64_Struct {
		public U64_Reference<GAM_Level> Level { get; set; }
		public string Name { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Level = s.SerializeObject<U64_Reference<GAM_Level>>(Level, name: nameof(Level));
			Name = s.SerializeString(Name, 62, name: nameof(Name));
		}
	}
}
