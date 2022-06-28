namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_TT_Level : BinarySerializable {
		public SNA_Description_Section Memory { get; set; }
		public uint MemorySnapShot { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Memory = s.SerializeObject<SNA_Description_Section>(Memory, name: nameof(Memory));
			MemorySnapShot = s.Serialize<uint>(MemorySnapShot, name: nameof(MemorySnapShot));
		}
	}
}
