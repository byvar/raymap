namespace BinarySerializer.Ubisoft.CPA {
	public class SND_BlockEvent : BinarySerializable {
		public uint Id { get; set; }
		public SND_EventType Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Id = s.Serialize<uint>(Id, name: nameof(Id));
			Type = s.Serialize<SND_EventType>(Type, name: nameof(Type));
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
