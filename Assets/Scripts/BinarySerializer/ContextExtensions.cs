using BinarySerializer;

namespace Raymap {
    public static class ContextExtensions {
        public static MapViewerSettings GetMapViewerSettings(this SerializerObject s) => s.Context.GetMapViewerSettings();
        public static MapViewerSettings GetMapViewerSettings(this Context c) => c.GetRequiredSettings<MapViewerSettings>();

		public static Unity_Level GetUnityLevel(this SerializerObject s) => s.Context.GetUnityLevel();
		public static Unity_Level GetUnityLevel(this Context c) => c.GetRequiredStoredObject<Unity_Level>(Unity_Level.ContextKey);

		public static Unity_Environment GetUnityEnvironment(this SerializerObject s) => s.Context.GetUnityEnvironment();
		public static Unity_Environment GetUnityEnvironment(this Context c) => c.GetRequiredStoredObject<Unity_Environment>(Unity_Environment.ContextKey);
	}
}
