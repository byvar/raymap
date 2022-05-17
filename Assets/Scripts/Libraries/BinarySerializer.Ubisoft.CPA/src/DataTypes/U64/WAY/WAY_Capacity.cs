namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class WAY_Capacity : U64_Struct {
		public uint Capacity { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Capacity = s.Serialize<uint>(Capacity, name: nameof(Capacity));
		}
	}
}
