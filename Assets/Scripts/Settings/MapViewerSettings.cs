using BinarySerializer.Unity;

namespace Raymap {
    /// <summary>
    /// Common game settings
    /// </summary>
    public class MapViewerSettings {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="gameModeSelection">The game mode selection</param>
        /// <param name="gameDirectory">The game directory</param>
        /// <param name="world">The game world</param>
        /// <param name="level">The game level, starting at 1</param>
        public MapViewerSettings(GameModeSelection gameModeSelection, string gameDirectory, string map) {
            // Get the attribute data
            var atr = gameModeSelection.GetAttribute<GameModeAttribute>();

            GameModeSelection = gameModeSelection;
            Engine = atr.Category.GetAttribute<EngineCategoryAttribute>().Engine;
            GameDirectory = Util.NormalizePath(gameDirectory, isFolder: true);
            Map = map;
        }

        // Global settings

        /// <summary>
        /// The game mode selection
        /// </summary>
        public GameModeSelection GameModeSelection { get; }

        /// <summary>
        /// The major engine version
        /// </summary>
        public Engine Engine { get; }

        /// <summary>
        /// The game directory
        /// </summary>
        public string GameDirectory { get; set; }

        /// <summary>
        /// String describing the map to be loaded
        /// </summary>
        public string Map { get; set; }

        // Helpers

        public BaseGameManager GetGameManager => GameModeSelection.GetManager();
        public T GetGameManagerOfType<T>() where T : BaseGameManager => (T)GetGameManager;

    }
}