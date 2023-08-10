namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_File<T> : BinarySerializable where T : BinarySerializable, new() {
		public uint CryptKey { get; set; } = SNA_XorProcessor.DefaultCryptKey;

		public T Value { get; set; }

		private SNA_XorProcessor.DecodeMode Mode => 
			Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet) 
			? SNA_XorProcessor.DecodeMode.RedPlanet
			: SNA_XorProcessor.DecodeMode.Rayman2;
		private bool UseCryptKey => !Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet);

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				CryptKey = s.Serialize<uint>(CryptKey, name: nameof(CryptKey));
				
				s.DoProcessed<SNA_XorProcessor>(new SNA_XorProcessor(key: UseCryptKey ? CryptKey : SNA_XorProcessor.DefaultCryptKey, mode: Mode), () => {
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
