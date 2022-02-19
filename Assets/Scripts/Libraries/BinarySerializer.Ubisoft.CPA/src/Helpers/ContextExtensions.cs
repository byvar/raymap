namespace BinarySerializer.Ubisoft.CPA {
    public static class ContextExtensions {
        public static CPA_Settings GetCPASettings(this SerializerObject s) => s.Context.GetCPASettings();
        public static CPA_Settings GetCPASettings(this Context c) => c.GetSettings<CPA_Settings>();
    }
}
