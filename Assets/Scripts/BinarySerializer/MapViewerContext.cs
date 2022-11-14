using BinarySerializer;
using BinarySerializer.Unity;

namespace Raymap
{
	public class MapViewerContext : Context {

		public MapViewerContext(string basePath, MapViewerSettings settings, bool log = true) : base(
			basePath: basePath, // Pass in the base path
			settings: new MapViewerSerializerSettings(), // Pass in the settings
			serializerLogger: log ? new MapViewerSerializerLogger() : null, // Use map viewer serializer log for logging to a file
			fileManager: new MapViewerFileManager(), // Use map viewer file manager for use with FileSystem
			systemLogger: new UnitySystemLogger()) // Use Unity logger
		{
			// Add the game settings
			AddSettings(settings);

			// Add settings and pointers from the current manager
			settings.GetGameManager.AddContextSettings(this);
			settings.GetGameManager.AddContextPointers(this);
		}
		public MapViewerContext(MapViewerSettings settings, bool log = true) : this(settings.GameDirectory, settings, log: log) { }

		public MapViewerSettings GameSettings => GetRequiredSettings<MapViewerSettings>();
	}
}