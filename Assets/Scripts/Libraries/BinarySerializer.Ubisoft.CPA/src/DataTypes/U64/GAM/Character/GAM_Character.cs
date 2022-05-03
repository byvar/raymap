namespace BinarySerializer.Ubisoft.CPA.U64 {
	// Loaded into EngineObject
	public class GAM_Character : U64_Struct {
		public U64_Reference<GAM_Character3dData> _3dData { get; set; }
		public U64_Reference<U64_Placeholder> Brain { get; set; }
		public U64_Reference<GAM_CharacterCineInfo> CineInfo { get; set; }
		public U64_Reference<GAM_CharacterCollSet> CollSet { get; set; }
		public U64_Reference<GAM_CharacterLight> Light { get; set; }
		public U64_Reference<GAM_CharacterMicro> Micro { get; set; }
		public U64_Reference<GAM_CharacterStandardGame> StandardGame { get; set; }
		// Tonic Trouble: Stream, Stocklist, World, Way
		public U64_Reference<GAM_CharacterDynamics> Dynamics { get; set; }
		public U64_Reference<GAM_CharacterSound> Sound { get; set; }
		public byte AllocWay { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			_3dData = s.SerializeObject<U64_Reference<GAM_Character3dData>>(_3dData, name: nameof(_3dData))?.Resolve(s);
			Brain = s.SerializeObject<U64_Reference<U64_Placeholder>>(Brain, name: nameof(Brain));
			CineInfo = s.SerializeObject<U64_Reference<GAM_CharacterCineInfo>>(CineInfo, name: nameof(CineInfo))?.Resolve(s);
			CollSet = s.SerializeObject<U64_Reference<GAM_CharacterCollSet>>(CollSet, name: nameof(CollSet))?.Resolve(s);
			Light = s.SerializeObject<U64_Reference<GAM_CharacterLight>>(Light, name: nameof(Light))?.Resolve(s);
			Micro = s.SerializeObject<U64_Reference<GAM_CharacterMicro>>(Micro, name: nameof(Micro))?.Resolve(s);
			StandardGame = s.SerializeObject<U64_Reference<GAM_CharacterStandardGame>>(StandardGame, name: nameof(StandardGame))?.Resolve(s);
			Dynamics = s.SerializeObject<U64_Reference<GAM_CharacterDynamics>>(Dynamics, name: nameof(Dynamics))?.Resolve(s);
			Sound = s.SerializeObject<U64_Reference<GAM_CharacterSound>>(Sound, name: nameof(Sound))?.Resolve(s);
			AllocWay = s.Serialize<byte>(AllocWay, name: nameof(AllocWay));
			s.SerializePadding(1, logIfNotNull: true);
		}
	}
}
