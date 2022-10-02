namespace BinarySerializer.Ubisoft.CPA {
    public static class ContextExtensions {
        public static CPA_Settings GetCPASettings(this SerializerObject s) => s.Context.GetCPASettings();
        public static CPA_Settings GetCPASettings(this Context c) => c.GetRequiredSettings<CPA_Settings>();

		public static CPA_Globals GetCPAGlobals(this SerializerObject s) => s.Context.GetCPAGlobals();
		public static CPA_Globals GetCPAGlobals(this Context c, bool throwIfNotFound = true)
		{
			if (throwIfNotFound)
				return c.GetRequiredStoredObject<CPA_Globals>(CPA_Globals.ContextKey);
			else
				return c.GetStoredObject<CPA_Globals>(CPA_Globals.ContextKey);
		}
    }
}
