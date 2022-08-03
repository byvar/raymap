namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_EngineStructure : BinarySerializable {
		public GAM_EngineMode EngineMode { get; set; }
		public string LevelName { get; set; }
		public string NextLevelName { get; set; }
		public string FirstLevelName { get; set; }
		public string[] SubLevelNames { get; set; }
		public string CreditsLevelName { get; set; }
		public string NextLevelPositionPersoName { get; set; }
		public string NextLevelPositionCameraName { get; set; }

		public IPT_InputMode InputMode { get; set; }
		public GAM_DisplayFixMode DisplayFixMode { get; set; }
		public GLI_DrawMask DisplayMode { get; set; }
		public bool SkipMainMenu { get; set; }

		public GAM_EngineTimerStructure EngineTimer { get; set; }

		// CPA_3 v
		public byte MultiModePlayersCount { get; set; }
		public byte MultiModeColumnsCount { get; set; }
		public byte MultiModeMiniScreenRatio { get; set; }
		public Pointer<HIE_SuperObject>[] CurrentMainPlayers { get; set; }
		// CPA_3 ^

		// Viewports
		public short GLDDevice { get; set; }
		public short[] GLDViewports { get; set; }
		public GLD_ViewportAttributes[] ViewportAttributes { get; set; }
		public Pointer<GLI_Camera>[] GameViewportCameras { get; set; }
		public uint DrawSemaphore { get; set; }
		public short[] GLDFixViewports { get; set; }
		public GLD_ViewportAttributes[] FixViewportAttributes { get; set; }
		public GLI_SpecificAttributesFor3D[] FixAttributes3D { get; set; }
		public Pointer<GLI_Camera>[] FixCameras { get; set; }
		public GLI_SpecificAttributesFor3D[] GameAttributes3D { get; set; }
		public Pointer<GAM_ViewportManagement[]> ViewportArray { get; set; }

		public LST2_DynamicList<GAM_CameraNode> CameraList { get; set; }
		public LST2_DynamicList<GAM_Family> FamilyList { get; set; }
		public LST2_DynamicList<GAM_MainCharacter> MainCharacterList { get; set; }
		public LST2_DynamicList<GAM_AlwaysActiveCharacter> AlwaysActiveCharacterList { get; set; }
		public GAM_MainCharacterSingle MainCharacter { get; set; }

		public Pointer<HIE_SuperObject> WorldCharacter { get; set; }
		public Pointer<HIE_SuperObject> StdCamCharacter { get; set; }
		public Pointer<HIE_SuperObject> DbgCamCharacter { get; set; }
		public Pointer<HIE_SuperObject> Inventory { get; set; }

		public Pointer<GAM_LanguageStructure[]> LanguageTable { get; set; }
		public Pointer<FIL_FileNameList> GameSavedNameList { get; set; }
		public Pointer<FIL_FileNameList> LevelNameList { get; set; }

		public MAT_Transformation MainCharacterPosition { get; set; }
		public MAT_Transformation MainCameraPosition { get; set; }

		public ushort SubMap { get; set; }
		public ushort SubMapEntry { get; set; }

		public bool EngineIsInPaused { get; set; }
		public bool EngineHasInPaused { get; set; }
		public bool EngineIsInSaveGamePart { get; set; }

		public string[] LevelNames { get; set; }
		public string[] DemoNames { get; set; }
		public string[] DemoLevelNames { get; set; }
		public byte DemosCount { get; set; }
		public byte LevelsCount { get; set; }
		public byte CurrentLevelIndex { get; set; }
		public byte PreviousLevelIndex { get; set; }
		public byte ExitIdToQuitPrevLevel { get; set; }
		public byte LevelGlobalCounter { get; set; }

		public bool DemoMode { get; set; }

		public byte CurrentLanguage { get; set; }
		public byte LanguagesCount { get; set; }

		public bool EngineFrozen { get; set; }
		public bool Resurrection { get; set; }

		public GAM_CameraMode CameraMode { get; set; }

		public Pointer DonaldDuckUnknown { get; set; }

		// CPA_3
		public byte CurrentImportance { get; set; }
		public uint SOLAllocatedCount { get; set; }
		public uint SOLLoadedCount { get; set; }
		public uint NonPersistentLinksPerSOLCount { get; set; }
		public Pointer<GAM_Placeholder> SuperObjectLinks { get; set; }
		public Pointer<GAM_Placeholder> GraphChainedList { get; set; }
		public Pointer<GAM_Placeholder> CineManager { get; set; }


		private int LevelNameSize =>
			Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2) ? 30 : 260;
		private int DemoNameSize => 12;
		private int MaxSubLevelsCount => 2;
		private int MaxViewportCount =>
			Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3) ? 5 : 1;
		private int MaxLevelsCount =>
			Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3) ? 150 : 80;
		private int MaxDemosCount => 30;

		public override void SerializeImpl(SerializerObject s) {
			EngineMode = s.Serialize<GAM_EngineMode>(EngineMode, name: nameof(EngineMode));
			LevelName = s.SerializeString(LevelName, length: LevelNameSize, name: nameof(LevelName));
			NextLevelName = s.SerializeString(NextLevelName, length: LevelNameSize, name: nameof(NextLevelName));
			FirstLevelName = s.SerializeString(FirstLevelName, length: LevelNameSize, name: nameof(FirstLevelName));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				SubLevelNames = s.SerializeStringArray(SubLevelNames, MaxSubLevelsCount, length: LevelNameSize, name: nameof(SubLevelNames));
				CreditsLevelName = s.SerializeString(CreditsLevelName, length: LevelNameSize, name: nameof(CreditsLevelName));
				NextLevelPositionPersoName = s.SerializeString(NextLevelPositionPersoName, length: 50, name: nameof(NextLevelPositionPersoName));
				NextLevelPositionCameraName = s.SerializeString(NextLevelPositionCameraName, length: 50, name: nameof(NextLevelPositionCameraName));
				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
			}
			InputMode = s.Serialize<IPT_InputMode>(InputMode, name: nameof(InputMode));
			DisplayFixMode = s.Serialize<GAM_DisplayFixMode>(DisplayFixMode, name: nameof(DisplayFixMode));
			s.Align(4, Offset);
			DisplayMode = s.Serialize<GLI_DrawMask>(DisplayMode, name: nameof(DisplayMode));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				SkipMainMenu = s.Serialize<bool>(SkipMainMenu, name: nameof(SkipMainMenu));
				s.Align(4, Offset);
			}
			EngineTimer = s.SerializeObject<GAM_EngineTimerStructure>(EngineTimer, name: nameof(EngineTimer));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				MultiModePlayersCount = s.Serialize<byte>(MultiModePlayersCount, name: nameof(MultiModePlayersCount));
				MultiModeColumnsCount = s.Serialize<byte>(MultiModeColumnsCount, name: nameof(MultiModeColumnsCount));
				MultiModeMiniScreenRatio = s.Serialize<byte>(MultiModeMiniScreenRatio, name: nameof(MultiModeMiniScreenRatio));
				s.Align(4, Offset);
				CurrentMainPlayers = s.SerializePointerArray<HIE_SuperObject>(CurrentMainPlayers, 4, name: nameof(CurrentMainPlayers))
					?.ResolveObject(s);
			}
			GLDDevice = s.Serialize<short>(GLDDevice, name: nameof(GLDDevice));
			GLDViewports = s.SerializeArray<short>(GLDViewports, MaxViewportCount, name: nameof(GLDViewports));
			ViewportAttributes = s.SerializeObjectArray<GLD_ViewportAttributes>(ViewportAttributes, MaxViewportCount, name: nameof(ViewportAttributes));
			GameViewportCameras = s.SerializePointerArray<GLI_Camera>(GameViewportCameras, MaxViewportCount, name: nameof(GameViewportCameras))?.ResolveObject(s);

			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				DrawSemaphore = s.Serialize<uint>(DrawSemaphore, name: nameof(DrawSemaphore));

			GLDFixViewports = s.SerializeArray<short>(GLDFixViewports, MaxViewportCount, name: nameof(GLDFixViewports));
			s.Align(4, Offset);
			FixViewportAttributes = s.SerializeObjectArray<GLD_ViewportAttributes>(FixViewportAttributes, MaxViewportCount, name: nameof(FixViewportAttributes));
			
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				FixAttributes3D = s.SerializeObjectArray<GLI_SpecificAttributesFor3D>(FixAttributes3D, MaxViewportCount, name: nameof(FixAttributes3D));
			
			FixCameras = s.SerializePointerArray<GLI_Camera>(FixCameras, MaxViewportCount, name: nameof(FixCameras))?.ResolveObject(s);

			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				GameAttributes3D = s.SerializeObjectArray<GLI_SpecificAttributesFor3D>(GameAttributes3D, MaxViewportCount, name: nameof(GameAttributes3D));

			ViewportArray = s.SerializePointer<GAM_ViewportManagement[]>(ViewportArray, name: nameof(ViewportArray))
				?.ResolveObjectArray(s, MaxViewportCount * 2);

			CameraList = s.SerializeObject<LST2_DynamicList<GAM_CameraNode>>(CameraList, name: nameof(CameraList));
			CameraList?.Validate(s, nextOffset: 4, prevOffset: 8, fatherOffset: 12);
			CameraList?.Resolve(s, name: nameof(CameraList));
			FamilyList = s.SerializeObject<LST2_DynamicList<GAM_Family>>(FamilyList, name: nameof(FamilyList));
			FamilyList?.Validate(s);
			FamilyList?.Resolve(s, name: nameof(FamilyList));

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2)
				|| s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				MainCharacterList = s.SerializeObject<LST2_DynamicList<GAM_MainCharacter>>(MainCharacterList, name: nameof(MainCharacterList));
				MainCharacterList?.Validate(s, nextOffset: 8, prevOffset: 12, fatherOffset: 16);
				MainCharacterList?.Resolve(s, name: nameof(MainCharacterList));
			}

			AlwaysActiveCharacterList = s.SerializeObject<LST2_DynamicList<GAM_AlwaysActiveCharacter>>(AlwaysActiveCharacterList, name: nameof(AlwaysActiveCharacterList));
			uint elOffset = s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3) ? (uint)0 : 4;
			AlwaysActiveCharacterList?.Validate(s, nextOffset: elOffset, prevOffset: elOffset + 4, fatherOffset: elOffset + 8);
			AlwaysActiveCharacterList?.Resolve(s, name: nameof(AlwaysActiveCharacterList));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2)
				&& !s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				MainCharacter = s.SerializeObject<GAM_MainCharacterSingle>(MainCharacter, name: nameof(MainCharacter));
			}

			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				WorldCharacter = s.SerializePointer<HIE_SuperObject>(WorldCharacter, name: nameof(WorldCharacter))?.ResolveObject(s);
			StdCamCharacter = s.SerializePointer<HIE_SuperObject>(StdCamCharacter, name: nameof(StdCamCharacter))?.ResolveObject(s);
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				DbgCamCharacter = s.SerializePointer<HIE_SuperObject>(DbgCamCharacter, name: nameof(DbgCamCharacter))?.ResolveObject(s);
			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				Inventory = s.SerializePointer<HIE_SuperObject>(Inventory, name: nameof(Inventory)).ResolveObject(s);

			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				LanguageTable = s.SerializePointer<GAM_LanguageStructure[]>(LanguageTable, name: nameof(LanguageTable));

			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				GameSavedNameList = s.SerializePointer<FIL_FileNameList>(GameSavedNameList, name: nameof(GameSavedNameList));
			LevelNameList = s.SerializePointer<FIL_FileNameList>(LevelNameList, name: nameof(LevelNameList)); // TMP pointer. Don't resolve

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				MainCharacterPosition = s.SerializeObject<MAT_Transformation>(MainCharacterPosition, name: nameof(MainCharacterPosition));
				MainCameraPosition = s.SerializeObject<MAT_Transformation>(MainCameraPosition, name: nameof(MainCameraPosition));

				// SubMapNumber
				s.DoBits<uint>(b => {
					SubMapEntry = b.SerializeBits<ushort>(SubMapEntry, 16, name: nameof(SubMapEntry));
					SubMap = b.SerializeBits<ushort>(SubMap, 16, name: nameof(SubMap));
				});
			}

			EngineIsInPaused = s.Serialize<bool>(EngineIsInPaused, name: nameof(EngineIsInPaused));
			EngineHasInPaused = s.Serialize<bool>(EngineHasInPaused, name: nameof(EngineHasInPaused));
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				EngineIsInSaveGamePart = s.Serialize<bool>(EngineIsInSaveGamePart, name: nameof(EngineIsInSaveGamePart));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				LevelNames = s.SerializeStringArray(LevelNames, count: MaxLevelsCount, length: LevelNameSize, name: nameof(LevelNames));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					DemoNames = s.SerializeStringArray(DemoNames, count: MaxDemosCount, length: DemoNameSize, name: nameof(DemoNames));
					DemoLevelNames = s.SerializeStringArray(DemoLevelNames, count: MaxDemosCount, length: DemoNameSize, name: nameof(DemoLevelNames));
					DemosCount = s.Serialize<byte>(DemosCount, name: nameof(DemosCount));
				}
				LevelsCount = s.Serialize<byte>(LevelsCount, name: nameof(LevelsCount));
				CurrentLevelIndex = s.Serialize<byte>(CurrentLevelIndex, name: nameof(CurrentLevelIndex));
				PreviousLevelIndex = s.Serialize<byte>(PreviousLevelIndex, name: nameof(PreviousLevelIndex));
				ExitIdToQuitPrevLevel = s.Serialize<byte>(ExitIdToQuitPrevLevel, name: nameof(ExitIdToQuitPrevLevel));
				LevelGlobalCounter = s.Serialize<byte>(LevelGlobalCounter, name: nameof(LevelGlobalCounter));
				DemoMode = s.Serialize<bool>(DemoMode, name: nameof(DemoMode));

				CurrentLanguage = s.Serialize<byte>(CurrentLanguage, name: nameof(CurrentLanguage));
				LanguagesCount = s.Serialize<byte>(LanguagesCount, name: nameof(LanguagesCount));
				LanguageTable?.ResolveObjectArray(s, LanguagesCount);

				EngineFrozen = s.Serialize<bool>(EngineFrozen, name: nameof(EngineFrozen));
				Resurrection = s.Serialize<bool>(Resurrection, name: nameof(Resurrection));

				CameraMode = s.Serialize<GAM_CameraMode>(CameraMode, name: nameof(CameraMode));

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.DonaldDuckQuackAttackDemo)) {
					s.Align(4, Offset);
					DonaldDuckUnknown = s.SerializePointer(DonaldDuckUnknown, name: nameof(DonaldDuckUnknown));
				}

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					CurrentImportance = s.Serialize<byte>(CurrentImportance, name: nameof(CurrentImportance));
					s.Align(4, Offset);
					SOLAllocatedCount = s.Serialize<uint>(SOLAllocatedCount, name: nameof(SOLAllocatedCount));
					SOLLoadedCount = s.Serialize<uint>(SOLLoadedCount, name: nameof(SOLLoadedCount));
					NonPersistentLinksPerSOLCount = s.Serialize<uint>(NonPersistentLinksPerSOLCount, name: nameof(NonPersistentLinksPerSOLCount));
					SuperObjectLinks = s.SerializePointer<GAM_Placeholder>(SuperObjectLinks, name: nameof(SuperObjectLinks))?.ResolveObject(s);
					GraphChainedList = s.SerializePointer<GAM_Placeholder>(GraphChainedList, name: nameof(GraphChainedList))?.ResolveObject(s);
					CineManager = s.SerializePointer<GAM_Placeholder>(CineManager, name: nameof(CineManager))?.ResolveObject(s);
				}
			}
			s.Align(4, Offset);
		}
	}
}
