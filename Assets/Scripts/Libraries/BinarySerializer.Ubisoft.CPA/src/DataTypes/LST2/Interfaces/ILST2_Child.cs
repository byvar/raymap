namespace BinarySerializer.Ubisoft.CPA {
    public interface ILST2_Child<T, U> : ILST2_Entry<T>
		where T : BinarySerializable, ILST2_Child<T, U>, new() {
		public Pointer<U> LST2_Parent { get; }
	}
}
