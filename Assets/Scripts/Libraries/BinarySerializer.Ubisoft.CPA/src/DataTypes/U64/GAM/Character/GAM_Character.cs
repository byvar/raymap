namespace BinarySerializer.Ubisoft.CPA.U64 {
	// Loaded into EngineObject
	public class GAM_Character : U64_Struct {
		public U64_Reference<GAM_Character3dData> _3dData { get; set; }
		public U64_Reference<U64_Placeholder> Brain { get; set; }
		public U64_Reference<U64_Placeholder> CineInfo { get; set; }
		public U64_Reference<U64_Placeholder> CollSet { get; set; }
		public U64_Reference<U64_Placeholder> Light { get; set; }
		public U64_Reference<U64_Placeholder> Micro { get; set; }
		public U64_Reference<GAM_CharacterStandardGame> StandardGame { get; set; }
		// Tonic Trouble: Stream, Stocklist, World
		public U64_Reference<U64_Placeholder> Dynamics { get; set; }
		public U64_Reference<U64_Placeholder> Sound { get; set; }
		public byte AllocWay { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			_3dData = s.SerializeObject<U64_Reference<GAM_Character3dData>>(_3dData, name: nameof(_3dData))?.Resolve(s);
			Brain = s.SerializeObject<U64_Reference<U64_Placeholder>>(Brain, name: nameof(Brain));
			CineInfo = s.SerializeObject<U64_Reference<U64_Placeholder>>(CineInfo, name: nameof(CineInfo));
			CollSet = s.SerializeObject<U64_Reference<U64_Placeholder>>(CollSet, name: nameof(CollSet));
			Light = s.SerializeObject<U64_Reference<U64_Placeholder>>(Light, name: nameof(Light));
			Micro = s.SerializeObject<U64_Reference<U64_Placeholder>>(Micro, name: nameof(Micro));
			StandardGame = s.SerializeObject<U64_Reference<GAM_CharacterStandardGame>>(StandardGame, name: nameof(StandardGame))?.Resolve(s);
			Dynamics = s.SerializeObject<U64_Reference<U64_Placeholder>>(Dynamics, name: nameof(Dynamics));
			Sound = s.SerializeObject<U64_Reference<U64_Placeholder>>(Sound, name: nameof(Sound));
			AllocWay = s.Serialize<byte>(AllocWay, name: nameof(AllocWay));
			s.SerializePadding(1, logIfNotNull: true);
		}
	}
}
