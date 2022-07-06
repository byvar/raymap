namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_EventHistoryElement : BinarySerializable {
		public HistoryState State { get; set; }
		public byte DeviceType { get; set; }
		public byte Counter { get; set; }
		public short DeviceValue { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			State = s.Serialize<HistoryState>(State, name: nameof(State));
			DeviceType = s.Serialize<byte>(DeviceType, name: nameof(DeviceType));
			Counter = s.Serialize<byte>(Counter, name: nameof(Counter));
			s.SerializePadding(1, logIfNotNull: true);
			DeviceValue = s.Serialize<short>(DeviceValue, name: nameof(DeviceValue));
		}

		public enum HistoryState : byte {
			Stopped = 0,
			Stopping = 1,
			InProgress = 2
		}
	}
}
