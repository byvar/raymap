namespace BinarySerializer.Ubisoft.CPA {
	public class SCT_Sector : BinarySerializable, IHIE_LinkedObject {
		public LST2_DynamicList<SCT_Character> Characters { get; set; }
		public LST2_StaticList<SCT_StaticLight> StaticLights { get; set; }
		public LST2_DynamicList<SCT_DynamicLight> DynamicLights { get; set; }

		public LST2_StaticList<SCT_SectorInGraphicInteraction> SectorsInGraphicInteraction { get; set; }
		public LST2_StaticList<SCT_SectorInCollisionInteraction> SectorsInCollisionInteraction { get; set; }
		public LST2_StaticList<SCT_SectorInActivityInteraction> SectorsInActivityInteraction { get; set; }
		public LST2_StaticList<SCT_SectorInSoundInteraction> SectorsInSoundInteraction { get; set; }
		public LST2_StaticList<SCT_SectorSoundEvent> SoundEvents { get; set; }

		public COL_ParallelBox BoundingBox { get; set; }
		public float ZFarClip { get; set; } // Objects further from the camera than this value will be clipped. Turned off if value is 0

		public bool Virtual { get; set; }
		public byte CameraType { get; set; }
		public byte Counter { get; set; }
		public byte Priority { get; set; }

		public Pointer<GLI_Material> SkyVisualMaterial { get; set; }
		public byte FogIntensity { get; set; }

		// CPA_3
		public Pointer<HIE_SuperObject> PreviousSectorInList { get; set; }
		public Pointer<HIE_SuperObject> NextSectorInList { get; set; }
		public int ModifierMulIntensity { get; set; }
		public int ModifierAddIntensity { get; set; }
		public uint OpaqueDisplayList { get; set; }
		public uint TransparentDisplayList { get; set; }
		public byte CurrentActivity { get; set; }
		public byte WantedActivity { get; set; }
		public byte InList { get; set; }
		public int Transition { get; set; }
		public uint TransitionSoundEvent { get; set; }

		public string Name { get; set; }
		public Pointer NamePointer { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			Characters = s.SerializeObject<LST2_DynamicList<SCT_Character>>(Characters, name: nameof(Characters))
				?.Resolve(s, name: nameof(Characters));
			StaticLights = s.SerializeObject<LST2_StaticList<SCT_StaticLight>>(StaticLights, name: nameof(StaticLights))
				?.Resolve(s, name: nameof(StaticLights));
			DynamicLights = s.SerializeObject<LST2_DynamicList<SCT_DynamicLight>>(DynamicLights, name: nameof(DynamicLights))
				?.Resolve(s, name: nameof(DynamicLights));

			SectorsInGraphicInteraction = s.SerializeObject<LST2_StaticList<SCT_SectorInGraphicInteraction>>(SectorsInGraphicInteraction, name: nameof(SectorsInGraphicInteraction))
				?.Resolve(s, name: nameof(SectorsInGraphicInteraction));
			SectorsInCollisionInteraction = s.SerializeObject<LST2_StaticList<SCT_SectorInCollisionInteraction>>(SectorsInCollisionInteraction, name: nameof(SectorsInCollisionInteraction))
				?.Resolve(s, name: nameof(SectorsInCollisionInteraction));
			SectorsInActivityInteraction = s.SerializeObject<LST2_StaticList<SCT_SectorInActivityInteraction>>(SectorsInActivityInteraction, name: nameof(SectorsInActivityInteraction))
				?.Resolve(s, name: nameof(SectorsInActivityInteraction));
			SectorsInSoundInteraction = s.SerializeObject<LST2_StaticList<SCT_SectorInSoundInteraction>>(SectorsInSoundInteraction, name: nameof(SectorsInSoundInteraction))
				?.Resolve(s, name: nameof(SectorsInSoundInteraction));
			SoundEvents = s.SerializeObject<LST2_StaticList<SCT_SectorSoundEvent>>(SoundEvents, name: nameof(SoundEvents))
				?.Resolve(s, name: nameof(SoundEvents));

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
			}
			BoundingBox = s.SerializeObject<COL_ParallelBox>(BoundingBox, name: nameof(BoundingBox));
			ZFarClip = s.Serialize<float>(ZFarClip, name: nameof(ZFarClip));

			Virtual = s.Serialize<bool>(Virtual, name: nameof(Virtual));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				CameraType = s.Serialize<byte>(CameraType, name: nameof(CameraType));
			}
			Counter = s.Serialize<byte>(Counter, name: nameof(Counter));
			Priority = s.Serialize<byte>(Priority, name: nameof(Priority));
			s.Align(4, Offset);

			SkyVisualMaterial = s.SerializePointer<GLI_Material>(SkyVisualMaterial, name: nameof(SkyVisualMaterial))?.ResolveObject(s);
			FogIntensity = s.Serialize<byte>(FogIntensity, name: nameof(FogIntensity));
			if (s.GetCPASettings().HasNames) {
				Name = s.SerializeString(Name, length: 255, name: nameof(Name));
			}
			s.Align(4, Offset);

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				PreviousSectorInList = s.SerializePointer<HIE_SuperObject>(PreviousSectorInList, name: nameof(PreviousSectorInList))?.ResolveObject(s);
				NextSectorInList = s.SerializePointer<HIE_SuperObject>(NextSectorInList, name: nameof(NextSectorInList))?.ResolveObject(s);
				ModifierMulIntensity = s.Serialize<int>(ModifierMulIntensity, name: nameof(ModifierMulIntensity));
				ModifierAddIntensity = s.Serialize<int>(ModifierAddIntensity, name: nameof(ModifierAddIntensity));
				if (s.GetCPASettings().Platform == Platform.PS2) {
					OpaqueDisplayList = s.Serialize<uint>(OpaqueDisplayList, name: nameof(OpaqueDisplayList));
					TransparentDisplayList = s.Serialize<uint>(TransparentDisplayList, name: nameof(TransparentDisplayList));
				}
				CurrentActivity = s.Serialize<byte>(CurrentActivity, name: nameof(CurrentActivity));
				WantedActivity = s.Serialize<byte>(WantedActivity, name: nameof(WantedActivity));
				InList = s.Serialize<byte>(InList, name: nameof(InList));
				s.Align(4, Offset);
				Transition = s.Serialize<int>(Transition, name: nameof(Transition));
				TransitionSoundEvent = s.Serialize<uint>(TransitionSoundEvent, name: nameof(TransitionSoundEvent));
			}
		}
	}
}
