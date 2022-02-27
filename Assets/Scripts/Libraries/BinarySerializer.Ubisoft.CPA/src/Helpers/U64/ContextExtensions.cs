namespace BinarySerializer.Ubisoft.CPA.U64 {
    public static class ContextExtensions {
        public static LDR_Loader GetLoader(this SerializerObject s) => s.Context.GetLoader();
        public static LDR_Loader GetLoader(this Context c, bool throwIfNotFound = true) => 
            c.GetStoredObject<LDR_Loader>(LDR_Loader.ContextKey, throwIfNotFound);
    }
}
