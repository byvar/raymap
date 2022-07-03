namespace BinarySerializer.Ubisoft.CPA {
	public enum SNA_Module : byte {
		ERM =  0, // Error Manager
		MMG =  1, // Memory Manager
		INT =  2, // Interface (also ITF)
		GMT =  3, // GameMaterial
		SCR =  4, // Script
		GAM =  5, // Game
		GEO =  6, // Geometry Common
		IPT =  7, // Input
		RND =  8, // Random
		CMP =  9, // Compress and expand
		SAI = 10, // Save and initialization
		TMP = 11, // Temporary memory
		FIL = 12, // Files and paths management
		VIG = 13, // Vignette
		PO  = 14, // Physical objects
		AI  = 15, // Artificial intelligence
		POS = 16, // Position
		FON = 17, // 2D-FONT
		GLD = 18, // Graphic Library for Display
		TMR = 19, // Timer
		AIDebug = 20, // AI Debug
		SND = 21, // Sound
		Unassigned = 0xFF,

		// Tonic Trouble
		INV = 50, // Inventory
		MNU = 51, // Menus
		LS  = 52, // LipsSync
		INO = 53, // Input/Output
		BIN = 54, // Binary script module
		MTH = 55, // Mathematic for AI
	}
}
