namespace BinarySerializer.Ubisoft.CPA.U64 {
	public abstract class U64_Struct : BinarySerializable {
		public ushort CPA_Index { get; set; }
		public string CPA_IndexString => string.Format("{0:X4}", CPA_Index);
		public ushort CPA_TypeIndex { get; set; }
		public U64_StructType? CPA_Type => U64_StructType_Defines.GetType(Context, CPA_TypeIndex);
		public LDR_Loader CPA_Loader => Context.GetLoader();
	}
}
