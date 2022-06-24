namespace BinarySerializer.Ubisoft.CPA {
	public enum SNA_RelocationType {
		SNA = 0,
		GlobalPointers = 1, // Global Pointer file (GPT)
		Sound = 2,
		Textures = 3, // Texture file (PTX)
		// ^ in Rayman 2 | v Not in Rayman 2
		LipsSync = 4,
		Dialog = 5, // Language pointer file (DLG)
		RTG = 6, // Language-specific SNA blocks (lng)
		Video = 7,
	}
}
