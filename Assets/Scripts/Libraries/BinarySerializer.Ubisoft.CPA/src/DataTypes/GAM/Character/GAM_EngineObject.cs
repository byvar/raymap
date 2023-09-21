namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_EngineObject : BinarySerializable, IHIE_LinkedObject {
		public Pointer<GAM_Character3dData> _3dData { get; set; }
		public Pointer<GAM_CharacterStandardGame> StandardGame { get; set; }
		public Pointer<GAM_CharacterDynamics> Dynam { get; set; }
		public Pointer DynamPoly { get; set; }
		public Pointer Brain { get; set; }
		public Pointer CineInfo { get; set; }
		public Pointer CollSet { get; set; }
		public Pointer<GAM_CharacterAimData> AimData { get; set; }
		public Pointer<GAM_CharacterWay> Way { get; set; }
		public Pointer<GAM_CharacterLight> Light { get; set; }
		public Pointer<GAM_CharacterSectorInfo> SectorInfo { get; set; }
		public Pointer<GAM_CharacterMicro> Micro { get; set; }
		public Pointer<GAM_CharacterWorld> World { get; set; }
		public Pointer<GAM_CharacterTakePut> TakePut { get; set; }
		public Pointer StockList { get; set; }
		public Pointer Stream { get; set; }
		public Pointer ParticleSource { get; set; }
		public Pointer<GAM_CharacterSound> Sound { get; set; }
		public Pointer<GAM_CharacterAnimEffect> AnimEffect { get; set; }
		public Pointer<GAM_CharacterMagnet> Magnet { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			_3dData = s.SerializePointer<GAM_Character3dData>(_3dData, name: nameof(_3dData))?.ResolveObject(s);
			StandardGame = s.SerializePointer<GAM_CharacterStandardGame>(StandardGame, name: nameof(StandardGame))?.ResolveObject(s);
			Dynam = s.SerializePointer<GAM_CharacterDynamics>(Dynam, name: nameof(Dynam))?.ResolveObject(s);
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_Montreal))
				DynamPoly = s.SerializePointer(DynamPoly, name: nameof(DynamPoly));
			Brain = s.SerializePointer(Brain, name: nameof(Brain));
			CineInfo = s.SerializePointer(CineInfo, name: nameof(CineInfo));
			CollSet = s.SerializePointer(CollSet, name: nameof(CollSet));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				AimData = s.SerializePointer<GAM_CharacterAimData>(AimData, name: nameof(AimData))?.ResolveObject(s);
			Way = s.SerializePointer<GAM_CharacterWay>(Way, name: nameof(Way))?.ResolveObject(s);
			Light = s.SerializePointer<GAM_CharacterLight>(Light, name: nameof(Light))?.ResolveObject(s);
			SectorInfo = s.SerializePointer<GAM_CharacterSectorInfo>(SectorInfo, name: nameof(SectorInfo))?.ResolveObject(s);
			Micro = s.SerializePointer<GAM_CharacterMicro>(Micro, name: nameof(Micro))?.ResolveObject(s);
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				World = s.SerializePointer<GAM_CharacterWorld>(World, name: nameof(World))?.ResolveObject(s);
				TakePut = s.SerializePointer<GAM_CharacterTakePut>(TakePut, name: nameof(TakePut))?.ResolveObject(s);
				StockList = s.SerializePointer(StockList, name: nameof(StockList));
				Stream = s.SerializePointer(Stream, name: nameof(Stream));
			}
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				ParticleSource = s.SerializePointer(ParticleSource, name: nameof(ParticleSource));
			}
			Sound = s.SerializePointer<GAM_CharacterSound>(Sound, name: nameof(Sound))?.ResolveObject(s);
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				AnimEffect = s.SerializePointer<GAM_CharacterAnimEffect>(AnimEffect, name: nameof(AnimEffect))?.ResolveObject(s);
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					Magnet = s.SerializePointer<GAM_CharacterMagnet>(Magnet, name: nameof(Magnet))?.ResolveObject(s);
				}
			}
		}
	}
}
