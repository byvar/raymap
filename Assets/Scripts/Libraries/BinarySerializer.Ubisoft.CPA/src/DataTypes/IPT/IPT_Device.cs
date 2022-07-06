namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_Device : BinarySerializable {
		public IPT_OnePadActivate OnePadActivate { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			OnePadActivate = s.Serialize<IPT_OnePadActivate>(OnePadActivate, name: nameof(OnePadActivate));
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
