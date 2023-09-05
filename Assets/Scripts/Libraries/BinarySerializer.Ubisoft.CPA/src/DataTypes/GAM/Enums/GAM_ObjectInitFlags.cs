namespace BinarySerializer.Ubisoft.CPA {
	public enum GAM_ObjectInitFlags : byte {
		WhenPlayerGoOutOfActionZone = 0,
		Always = 1,
		WhenPlayerIsDead = 2,
		WhenMapJustLoaded = 3,
		WhenSavedGameJustLoaded = 4,
		NeverBackWhenTaken = 5,
		NumberOfObjectInit = 6,
		WhenGeneratorIsDesactivated = 7
	}
}
