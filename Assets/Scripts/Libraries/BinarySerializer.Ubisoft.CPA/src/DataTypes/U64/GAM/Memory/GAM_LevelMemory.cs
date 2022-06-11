namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_LevelMemory : U64_Struct {
		public uint AI { get; set; }
		public uint ACP { get; set; }
		public uint ACPU64 { get; set; }
		public uint SAI { get; set; }
		public uint TMP { get; set; }
		public uint Game { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			AI = s.Serialize<uint>(AI, name: nameof(AI));
			ACP = s.Serialize<uint>(ACP, name: nameof(ACP));
			ACPU64 = s.Serialize<uint>(ACPU64, name: nameof(ACPU64));
			SAI = s.Serialize<uint>(SAI, name: nameof(SAI));
			TMP = s.Serialize<uint>(TMP, name: nameof(TMP));
			Game = s.Serialize<uint>(Game, name: nameof(Game));
		}
	}
}
