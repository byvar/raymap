namespace BinarySerializer.Ubisoft.CPA {
    public interface ILST2_StaticEntry<T> : ILST2_Entry<T>
		where T : BinarySerializable, ILST2_StaticEntry<T>, new() {
		public Pointer<LST2_StaticList<T>> LST2_Parent { get; }
	}
}
