using BinarySerializer;

namespace Raymap {
    public static class ContextExtensions {
        public static MapViewerSettings GetMapViewerSettings(this SerializerObject s) => s.Context.GetMapViewerSettings();
        public static MapViewerSettings GetMapViewerSettings(this Context c) => c.GetSettings<MapViewerSettings>();
    }
}
