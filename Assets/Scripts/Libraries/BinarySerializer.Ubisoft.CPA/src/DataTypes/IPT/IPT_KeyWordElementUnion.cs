namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_KeyWordElementUnion : BinarySerializable {
		public short Value { get; set; }
		public uint ValueUInt { get; set; }
		public Pointer<IPT_EntryElement> ValuePointer { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.Serialize<short>(Value, name: nameof(Value));
			s.Goto(Offset);
			ValueUInt = s.Serialize<uint>(ValueUInt, name: nameof(ValueUInt));
			s.Goto(Offset);
			ValuePointer = s.SerializePointer<IPT_EntryElement>(ValuePointer, allowInvalid: true, name: nameof(ValuePointer));
		}
	}
}
