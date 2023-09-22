namespace BinarySerializer.Ubisoft.CPA {
    public enum AI_ObjectTreeInit : uint {
		WhenGameStart = 0,
		MapLoaded = 1,
		ReinitTheMap = 2,
		LoadSavedGame = 3,
		PlayerDead = 4, WhenGoOutOfZone = 4, Always = 4,
		AlwaysCreated = 5
	}
}