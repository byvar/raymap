namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_EngineObject : BinarySerializable, IHIE_LinkedObject {
		public Pointer<GAM_Character3dData> _3dData { get; set; }
		public Pointer<GAM_StandardGame> StandardGame { get; set; }
		public Pointer Dynam { get; set; }
		public Pointer DynamPoly { get; set; }
		public Pointer Brain { get; set; }
		public Pointer CineInfo { get; set; }
		public Pointer CollSet { get; set; }
		public Pointer AimData { get; set; }
		public Pointer Way { get; set; }
		public Pointer Light { get; set; }
		public Pointer SectInfo { get; set; }
		public Pointer Micro { get; set; }
		public Pointer World { get; set; }
		public Pointer TakPut { get; set; }
		public Pointer StockList { get; set; }
		public Pointer Stream { get; set; }
		public Pointer ParticleSource { get; set; }
		public Pointer Sound { get; set; }
		public Pointer AnimEffect { get; set; }
		public Pointer Magnet { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			_3dData = s.SerializePointer<GAM_Character3dData>(_3dData, name: nameof(_3dData))?.ResolveObject(s);
			StandardGame = s.SerializePointer<GAM_StandardGame>(StandardGame, name: nameof(StandardGame))?.ResolveObject(s);
			Dynam = s.SerializePointer(Dynam, name: nameof(Dynam));
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_Montreal))
				DynamPoly = s.SerializePointer(DynamPoly, name: nameof(DynamPoly));
			Brain = s.SerializePointer(Brain, name: nameof(Brain));
			CineInfo = s.SerializePointer(CineInfo, name: nameof(CineInfo));
			CollSet = s.SerializePointer(CollSet, name: nameof(CollSet));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				AimData = s.SerializePointer(AimData, name: nameof(AimData));
			Way = s.SerializePointer(Way, name: nameof(Way));
			Light = s.SerializePointer(Light, name: nameof(Light));
			SectInfo = s.SerializePointer(SectInfo, name: nameof(SectInfo));
			Micro = s.SerializePointer(Micro, name: nameof(Micro));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				World = s.SerializePointer(World, name: nameof(World));
				TakPut = s.SerializePointer(TakPut, name: nameof(TakPut));
				StockList = s.SerializePointer(StockList, name: nameof(StockList));
				Stream = s.SerializePointer(Stream, name: nameof(Stream));
			}
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				ParticleSource = s.SerializePointer(ParticleSource, name: nameof(ParticleSource));
			}
			Sound = s.SerializePointer(Sound, name: nameof(Sound));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				AnimEffect = s.SerializePointer(AnimEffect, name: nameof(AnimEffect));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					Magnet = s.SerializePointer(Magnet, name: nameof(Magnet));
				}
			}
		}
	}
}
