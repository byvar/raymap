namespace BinarySerializer.Ubisoft.CPA {
	public class MMG_Allocation : BinarySerializable {
		public uint AllocationSize { get; set; }
		public byte[] Allocation { get; set; }
		
		public override void SerializeImpl(SerializerObject s) {
			AllocationSize = s.Serialize<uint>(AllocationSize, name: nameof(AllocationSize));
			Allocation = s.SerializeArray<byte>(Allocation, AllocationSize * 4 - 4, name: nameof(Allocation));
		}
	}
}
