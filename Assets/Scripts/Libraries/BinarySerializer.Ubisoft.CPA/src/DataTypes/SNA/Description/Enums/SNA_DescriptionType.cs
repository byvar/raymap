namespace BinarySerializer.Ubisoft.CPA {
	public enum SNA_DescriptionType : uint {
		// From Game.mem
		MemoryDescTitle = 0,
		GAMFixMemory = 1,
		ACPFixMemory = 2,
		ACPTextMemory = 3,
		AIFixMemory = 4,
		TMPFixMemory = 5,
		IPTMemory = 6,
		SAIFixMemory = 7,
		FontMemory = 8,
		PositionMemory = 9,

		// From level.mem
		GAMLevelMemory = 11,
		AILevelMemory = 12,
		ACPLevelMemory = 13,
		SAILevelMemory = 14,
		TMPLevelMemory = 15,

		// From both
		ScriptMemory = 16,

		// From game.dsc: level description
		LevelNameTitle = 30,
		LevelName = 31,

		// From game.dsc and game.rnd: random description
		RandomDescTitle = 32,
		RandomComputeTable = 33,
		RandomReadTable = 34,

		// From game.dsc : Directories description
		DirectoryDescTitle = 40,
		DirectoryOfEngineDLL = 41,
		DirectoryOfGameData = 42,
		DirectoryOfTexts = 43,
		DirectoryOfWorld = 44,
		DirectoryOfLevels = 45,
		DirectoryOfFamilies = 46,
		DirectoryOfCharacters = 47,
		DirectoryOfAnimations = 48,
		DirectoryOfGraphicsClasses = 49,
		DirectoryOfGraphicsBanks = 50,
		DirectoryOfMechanics = 51,
		DirectoryOfSound = 52,
		DirectoryOfVisuals = 53,
		DirectoryOfEnvironment = 54,
		DirectoryOfMaterials = 55,
		DirectoryOfSaveGame = 56,
		DirectoryOfExtras = 57,
		DirectoryOfTexture = 58,
		DirectoryOfVignettes = 59,
		DirectoryOfOptions = 60,
		DirectoryOfLipsSync = 61,
		DirectoryOfZdx = 62,
		DirectoryOfEffects = 63,

		// From game.dsc: big file description
		BigFileDescTitle = 64,
		BigFileVignettes = 65,
		BigFileTextures = 66,

		// From game.pbg & level.pbg: Vignette description
		VignetteDescTitle = 70,
		LoadVignette = 71,
		LoadLevelVignette = 72,
		InitVignette = 73,
		FreeVignette = 74,
		DisplayVignette = 75,
		InitBarOutlineColor = 76,
		InitBarInsideColor = 77,
		InitBarColor = 78,
		CreateBar = 79,
		AddBar = 80,
		MaxValueBar = 81,

		// From level.dsc
		LevelDscTitle = 90,
		NumberOfAlways = 91,
		LevelDscLevelSoundBanks = 92,
		LevelLoadMap = 93,
		LevelLoadSoundBank = 94,

		// From game.dsc: game options description
		GameOptionDescTitle = 100,
		DefaultFile = 101,
		CurrentFile = 102,
		FrameSynchro = 103,

		// from game.dsc, input description
		InitInputDeviceManager = 110,

		// from Device.ipt, active devices description
		ActivateDeviceTitle = 120,
		ActivatePadAction = 121,
		ActivateJoystickAction = 122,
		ActivateKeyboardAction = 123,
		ActivateMouseAction = 124,

		// Final section
		EndOfDescSection = 0xffff,

		// Tonic Trouble
		TT_MenuMemory = 0x10000 + 2,
		TT_FontMemory = 0x10000 + 3,
		TT_InventoryMemory = 0x10000 + 13,
		TT_LipsSynchMemory = 0x10000 + 16,
		TT_BigFileCredits = 0x10000 + 19,
		TT_CreditsLevelName = 0x10000 + 22,
		TT_SkipMainMenu = 0x10000 + 23,
	}
}
