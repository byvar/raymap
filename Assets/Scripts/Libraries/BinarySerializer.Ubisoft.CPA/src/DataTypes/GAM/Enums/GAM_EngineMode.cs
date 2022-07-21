using System;

namespace BinarySerializer.Ubisoft.CPA {
	public enum GAM_EngineMode : byte {
		Invalid = 0,
		StartingProgram,                  // first init
		StoppingProgram,                  // last desinit
		EnterGame,                        // init game loop
		QuitGame,                         // desinit game loop
		EnterLevel,                       // init level loop
		ChangeLevel,                      // desinit level loop
		DeadLoop,                         // init dead loop
		PlayerDead,                       // desinit dead loop
		Playing,                          // playing game

		// Not in R2
		EnterMenu,                        // Init the menu
		Menu,                             // In the start menu
		QuitMenuToEnterGame,              // Go to the game from the menu
		EnterMenuWhenPlaying,             // Init the menu when playing
		EnterMenuWhenPlayingWithoutPause, // Init the menu by AI
		MenuWhenPlaying,                  // In the menu when playing
		MenuWhenPlayingWithoutPause,      // In the menu when playing without pause
		ResumeGameFromMenu,               // Return to the game from the menu
	}
}
