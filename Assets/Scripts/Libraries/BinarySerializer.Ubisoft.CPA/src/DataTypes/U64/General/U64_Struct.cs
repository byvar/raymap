namespace BinarySerializer.Ubisoft.CPA.U64 {
	public abstract class U64_Struct : BinarySerializable {
		public ushort CPA_Index { get; set; }
		public string CPA_IndexString => string.Format("{0:X4}", CPA_Index);
		public int CPA_ArrayIndex { get; set; }
		public U64_StructType? CPA_Type {
			get {
				var mapping = U64_StructType_Defines.TypeMapping;
				var type = GetType();
				if(mapping.ContainsKey(type)) return mapping[type];
				return null;
			}
		}
		public LDR_Loader CPA_Loader => Context.GetLoader();
	}
}
