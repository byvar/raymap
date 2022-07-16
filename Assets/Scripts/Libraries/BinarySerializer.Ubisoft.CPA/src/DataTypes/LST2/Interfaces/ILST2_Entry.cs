namespace BinarySerializer.Ubisoft.CPA {
    public interface ILST2_Entry<T>
		where T : BinarySerializable, ILST2_Entry<T>, new() {
		public Pointer<T> LST2_Next { get; }
		public Pointer<T> LST2_Previous { get; }
	}
}
