namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class LST_List<T> : U64_Struct where T : U64_Struct, new() {
		public U64_ArrayReference<T> List { get; set; }
		public ushort Count { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			List = s.SerializeObject<U64_ArrayReference<T>>(List, name: nameof(List));
			Count = s.Serialize<ushort>(Count, name: nameof(Count));
		}

		public LST_List<T> Resolve(SerializerObject s) {
			List?.Resolve(s, Count);
			return this;
		}
	}
}
