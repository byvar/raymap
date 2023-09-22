using System;

namespace BinarySerializer.Ubisoft.CPA {
	[Flags]
    public enum SAI_Flags : ushort {
		None                    = 0,

		// Init types
		InitWhenNewGameStart           = 0x0001,
		InitWhenMapLoaded              = 0x0002,
		InitWhenPlayerGameSavedLoaded  = 0x0004,
		InitWhenLevelGameSavedLoaded   = 0x0008,
		InitWhenReinitTheMap           = 0x0010,
		InitWhenPlayerDead             = 0x0020,
 
		// Save types
		PlayerSaveTableValue           = 0x0040,
		PlayerSaveCurrentValue         = 0x0080,
		PlayerSaveMask                 = PlayerSaveTableValue | PlayerSaveCurrentValue,
		LevelSaveTableValue            = 0x0100,
		LevelSaveCurrentValue          = 0x0200,
		LevelSaveMask                  = LevelSaveTableValue | LevelSaveCurrentValue,
		AllSaveMask                    = LevelSaveMask | PlayerSaveMask,
 
		// Data's types
		DataType8                      = 0x1000,
		DataType16                     = 0x2000,
		DataType32                     = 0x3000,
		DataType64                     = 0x4000,
		DataTypeXX                     = 0x5000,
		DataTypePointer                = 0x7000,
		DataType1                      = 0x8000,
	}
}