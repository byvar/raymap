namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_File<T> : BinarySerializable where T : BinarySerializable, new() {
		public uint CryptKey { get; set; } = SNA_XORCalculator.DefaultCryptKey;

		public T Value { get; set; }

		private SNA_XORCalculator.DecodeMode Mode => 
			Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet) 
			? SNA_XORCalculator.DecodeMode.RedPlanet
			: SNA_XORCalculator.DecodeMode.Rayman2;
		private bool UseCryptKey => !Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet);

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				CryptKey = s.Serialize<uint>(CryptKey, name: nameof(CryptKey));
				
				s.DoXOR(new SNA_XORCalculator(key: UseCryptKey ? CryptKey : SNA_XORCalculator.DefaultCryptKey, mode: Mode), () => {
					Value = s.SerializeObject<T>(Value, name: nameof(Value));
				});
			} else if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.TonicTrouble)) {
				s.DoEncoded(new SNA_TTWindowEncoder(), () => {
					Value = s.SerializeObject<T>(Value, name: nameof(Value));
				});
			} else {
				Value = s.SerializeObject<T>(Value, name: nameof(Value));
			}
		}
	}
}
