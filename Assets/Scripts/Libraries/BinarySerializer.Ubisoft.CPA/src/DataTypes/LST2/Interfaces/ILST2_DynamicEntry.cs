namespace BinarySerializer.Ubisoft.CPA {
    public interface ILST2_DynamicEntry<T> : ILST2_Entry<T>
		where T : BinarySerializable, ILST2_DynamicEntry<T>, new() {
		public Pointer<LST2_DynamicList<T>> LST2_Parent { get; }
	}
}
