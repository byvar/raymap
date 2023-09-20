namespace BinarySerializer.Ubisoft.CPA {
	public class SND_Real : BinarySerializable, ISerializerShortLog {
		public FixedPointInt32 Value { get; set; }

		public string ShortLog => ((ISerializerShortLog)Value)?.ShortLog;

		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeObject<FixedPointInt32>(Value, name: nameof(Value));
		}
	}
}
