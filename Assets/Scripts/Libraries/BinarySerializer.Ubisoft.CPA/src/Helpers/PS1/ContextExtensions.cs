namespace BinarySerializer.Ubisoft.CPA.PS1 {
    public static class ContextExtensions {
        public static GAM_Level_PS1 GetLevel(this SerializerObject s) => s.Context.GetLevel();
        public static GAM_Level_PS1 GetLevel(this Context c, bool throwIfNotFound = true) => 
            c.GetStoredObject<GAM_Level_PS1>(GAM_Level_PS1.ContextKey, throwIfNotFound);
    }
}
