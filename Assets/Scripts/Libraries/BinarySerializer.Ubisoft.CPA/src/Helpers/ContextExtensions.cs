namespace BinarySerializer.Ubisoft.CPA {
    public static class ContextExtensions {
        public static CPA_Settings GetCPASettings(this SerializerObject s) => s.Context.GetCPASettings();
        public static CPA_Settings GetCPASettings(this Context c) => c.GetSettings<CPA_Settings>();

		public static CPA_Globals GetCPAGlobals(this SerializerObject s) => s.Context.GetCPAGlobals();
		public static CPA_Globals GetCPAGlobals(this Context c, bool throwIfNotFound = true) =>
			c.GetStoredObject<CPA_Globals>(CPA_Globals.ContextKey, throwIfNotFound);
	}
}
