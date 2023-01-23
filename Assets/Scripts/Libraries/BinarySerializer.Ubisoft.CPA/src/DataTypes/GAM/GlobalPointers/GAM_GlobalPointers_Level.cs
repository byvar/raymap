namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_GlobalPointers_Level : BinarySerializable {
		public GAM_FixInfo FixInfo { get; set; }
		public Pointer<HIE_SuperObject> ActualWorld { get; set; }
		public Pointer<HIE_SuperObject> DynamicWorld { get; set; }
		public Pointer<HIE_SuperObject> InactiveDynamicWorld { get; set; }
		public Pointer<HIE_SuperObject> FatherSector { get; set; }
		public Pointer<GAM_SubMapPosition> FirstSubmapPosition { get; set; }
		public GAM_Always Always { get; set; }
		public GAM_AlwaysListInfo AlwaysListInfo { get; set; }
		public GAM_ObjectTypeListInfo ObjectTypeListInfo { get; set; }
		public GAM_ObjectType ObjectType { get; set; }
		public GAM_EngineStructure EngineStructure { get; set; }
		public Pointer<GLI_Light> Light { get; set; }
		public Pointer<HIE_SuperObject> MainCharacter { get; set; }
		public Pointer<GAM_MainCharacter> MainCharacterNode { get; set; }
		public Pointer<HIE_SuperObject> CharacterLaunchingSoundEvents { get; set; }

		public Pointer<GMT_GameMaterial> BackgroundGameMaterial { get; set; }
		public Pointer<GLI_Material> ShadowPolygonVisualMaterial { get; set; }
		public Pointer<GMT_GameMaterial> ShadowPolygonGameMaterialInit { get; set; }
		public Pointer<GMT_GameMaterial> ShadowPolygonGameMaterial { get; set; }
		public Pointer<GLI_Texture> TextureOfTextureShadow { get; set; }
		public Pointer<uint[]> CollisionTaggedFacesTable { get; set; }
		public SHW_ShadowObject[] ShadowObjects { get; set; }

		public Pointer<GAM_DemoSOList> DemoSOList { get; set; }
		public Pointer<GAM_DemoGMTList> DemoGMTList { get; set; }
		public uint DonaldDuckUnknown0 { get; set; }
		public uint DonaldDuckUnknown1 { get; set; }
		public A3D_AnimationBank Animations { get; set; }

		public Pointer<GAM_EngineObject> AlphabetCharacter { get; set; }
		public Pointer<GAM_EngineObject> AlphabetCharacterNew { get; set; }
		public Pointer<GEO_GeometricObject> MenuBackgroundObject { get; set; }
		public int BeginMapSoundEventFlag { get; set; }
		public int BeginMapSoundEvent { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				FixInfo = s.SerializeObject<GAM_FixInfo>(FixInfo, name: nameof(FixInfo));
				ActualWorld = s.SerializePointer<HIE_SuperObject>(ActualWorld, name: nameof(ActualWorld));
				DynamicWorld = s.SerializePointer<HIE_SuperObject>(DynamicWorld, name: nameof(DynamicWorld));
				InactiveDynamicWorld = s.SerializePointer<HIE_SuperObject>(InactiveDynamicWorld, name: nameof(InactiveDynamicWorld));
				FatherSector = s.SerializePointer<HIE_SuperObject>(FatherSector, name: nameof(FatherSector));
				FirstSubmapPosition = s.SerializePointer<GAM_SubMapPosition>(FirstSubmapPosition, name: nameof(FirstSubmapPosition))?.ResolveObject(s);
				Always = s.SerializeObject<GAM_Always>(Always, name: nameof(Always));
				AlwaysListInfo = s.SerializeObject<GAM_AlwaysListInfo>(AlwaysListInfo, name: nameof(AlwaysListInfo))
					?.RepairLists(s, Always);
				ObjectTypeListInfo = s.SerializeObject<GAM_ObjectTypeListInfo>(ObjectTypeListInfo, name: nameof(ObjectTypeListInfo));
				ObjectType = s.SerializeObject<GAM_ObjectType>(ObjectType, name: nameof(ObjectType))
					?.RepairLists(s)
					?.Resolve(s);
				EngineStructure = s.SerializeObject<GAM_EngineStructure>(EngineStructure, name: nameof(EngineStructure));
				Light = s.SerializePointer<GLI_Light>(Light, name: nameof(Light))?.ResolveObject(s);

				// Now that EngineStructure is resolved, resolve object hierarchy & always
				ActualWorld?.ResolveObject(s);
				DynamicWorld?.ResolveObject(s);
				InactiveDynamicWorld?.ResolveObject(s);
				FatherSector?.ResolveObject(s);
				Always?.Resolve(s);
				AlwaysListInfo?.Resolve(s);

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2)
					&& !s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					MainCharacter = s.SerializePointer<HIE_SuperObject>(MainCharacter, name: nameof(MainCharacter))?.ResolveObject(s);
				} else {
					MainCharacterNode = s.SerializePointer<GAM_MainCharacter>(MainCharacterNode, name: nameof(MainCharacterNode))?.ResolveObject(s);
				}
				CharacterLaunchingSoundEvents = s.SerializePointer<HIE_SuperObject>(CharacterLaunchingSoundEvents, name: nameof(CharacterLaunchingSoundEvents))?.ResolveObject(s);
				
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.DonaldDuckQuackAttackDemo)) {
					BackgroundGameMaterial = s.SerializePointer<GMT_GameMaterial>(BackgroundGameMaterial, name: nameof(BackgroundGameMaterial))?.ResolveObject(s);
				}
				// Shadow
				ShadowPolygonVisualMaterial = s.SerializePointer<GLI_Material>(ShadowPolygonVisualMaterial, name: nameof(ShadowPolygonVisualMaterial))?.ResolveObject(s);
				ShadowPolygonGameMaterialInit = s.SerializePointer<GMT_GameMaterial>(ShadowPolygonGameMaterialInit, name: nameof(ShadowPolygonGameMaterialInit))?.ResolveObject(s);
				ShadowPolygonGameMaterial = s.SerializePointer<GMT_GameMaterial>(ShadowPolygonGameMaterial, name: nameof(ShadowPolygonGameMaterial))?.ResolveObject(s);
				TextureOfTextureShadow = s.SerializePointer<GLI_Texture>(TextureOfTextureShadow, name: nameof(TextureOfTextureShadow))?.ResolveObject(s);
				CollisionTaggedFacesTable = s.SerializePointer<uint[]>(CollisionTaggedFacesTable, name: nameof(CollisionTaggedFacesTable));
				ShadowObjects = s.SerializeObjectArray<SHW_ShadowObject>(ShadowObjects, 10, name: nameof(ShadowObjects));

				DemoSOList = s.SerializePointer<GAM_DemoSOList>(DemoSOList, name: nameof(DemoSOList))?.ResolveObject(s);
				if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2) || s.GetCPASettings().EngineVersion == EngineVersion.DonaldDuckQuackAttackDemo) {
					DemoGMTList = s.SerializePointer<GAM_DemoGMTList>(DemoGMTList, name: nameof(DemoGMTList))?.ResolveObject(s);
				}
				if (s.GetCPASettings().EngineVersion == EngineVersion.DonaldDuckQuackAttackDemo) {
					DonaldDuckUnknown0 = s.Serialize<uint>(DonaldDuckUnknown0, name: nameof(DonaldDuckUnknown0));
					DonaldDuckUnknown1 = s.Serialize<uint>(DonaldDuckUnknown1, name: nameof(DonaldDuckUnknown1));
				}

				Animations = s.SerializeObject<A3D_AnimationBank>(Animations, name: nameof(Animations));
				Animations?.SerializeData(s, A3D_AnimationBank.SerializeMode.Inline, append: true);

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Demo)) {
					AlphabetCharacter = s.SerializePointer<GAM_EngineObject>(AlphabetCharacter, name: nameof(AlphabetCharacter))?.ResolveObject(s);
					if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2)) {
						if (s.GetCPASettings().Platform == Platform.iOS)
							AlphabetCharacterNew = s.SerializePointer<GAM_EngineObject>(AlphabetCharacterNew, name: nameof(AlphabetCharacterNew))?.ResolveObject(s);

						MenuBackgroundObject = s.SerializePointer<GEO_GeometricObject>(MenuBackgroundObject, name: nameof(MenuBackgroundObject))?.ResolveObject(s);

						BeginMapSoundEventFlag = s.Serialize<int>(BeginMapSoundEventFlag, name: nameof(BeginMapSoundEventFlag));
						BeginMapSoundEvent = s.Serialize<int>(BeginMapSoundEvent, name: nameof(BeginMapSoundEvent));
					}
				}
			}
		}
	}
}
