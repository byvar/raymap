using BinarySerializer.Unity;

namespace Raymap 
{
	/// <summary>
	/// The common map viewer settings
	/// </summary>
	public class MapViewerSettings 
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="gameModeSelection">The game mode selection</param>
		/// <param name="gameDirectory">The game directory</param>
		/// <param name="map">String describing the map to be loaded</param>
		public MapViewerSettings(GameModeSelection gameModeSelection, string gameDirectory, string map) 
		{
			// Get the attribute data
			GameModeAttribute atr = gameModeSelection.GetAttribute<GameModeAttribute>();

			GameModeSelection = gameModeSelection;
			Engine = atr.Category.GetAttribute<EngineCategoryAttribute>().Engine;
			GameDirectory = Util.NormalizePath(gameDirectory, isFolder: true);
			Map = map;
		}

		private BaseGameManager _manager;

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

		public BaseGameManager GetGameManager => _manager ??= GameModeSelection.GetManager();
		public T GetGameManagerOfType<T>() where T : BaseGameManager => (T)GetGameManager;

	}
}