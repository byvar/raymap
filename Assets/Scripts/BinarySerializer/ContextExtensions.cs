using BinarySerializer;

namespace Raymap {
    public static class ContextExtensions {
        public static MapViewerSettings GetMapViewerSettings(this SerializerObject s) => s.Context.GetMapViewerSettings();
        public static MapViewerSettings GetMapViewerSettings(this Context c) => c.GetSettings<MapViewerSettings>();

		public static Unity_Level GetUnityLevel(this SerializerObject s) => s.Context.GetUnityLevel();
		public static Unity_Level GetUnityLevel(this Context c) => c.GetStoredObject<Unity_Level>(Unity_Level.ContextKey, throwIfNotFound: true);
	}
}
