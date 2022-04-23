namespace BinarySerializer.Ubisoft.CPA.PS1 {
    public static class ContextExtensions {
        public static LevelHeader GetLevelHeader(this SerializerObject s) => s.Context.GetLevelHeader();
        public static LevelHeader GetLevelHeader(this Context c, bool throwIfNotFound = true) => 
            c.GetStoredObject<LevelHeader>(LevelHeader.ContextKey, throwIfNotFound);
    }
}
