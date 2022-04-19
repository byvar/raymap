namespace BinarySerializer.Ubisoft.CPA {
    public interface LST2_IEntry<T> where T : BinarySerializable, LST2_IEntry<T>, new() {
        public Pointer<T> LST2_Next { get; }
		public Pointer<T> LST2_Previous { get; }
    }
}
