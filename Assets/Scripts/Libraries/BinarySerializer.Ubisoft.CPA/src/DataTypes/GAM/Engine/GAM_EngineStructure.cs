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

		private int LevelNameSize =>
			Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2) ? 30 : 260;
		private int MaxSubLevelsCount => 2;
		private int MaxViewportCount =>
			Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3) ? 5 : 1;

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




			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
