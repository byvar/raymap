namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_CommandElement : BinarySerializable, LST2_IEntry<IPT_CommandElement> {
		public Pointer<string> CommandName { get; set; }
		public Pointer<string> Command { get; set; }
		public Pointer<string> HelpLine { get; set; }
		public byte ParamsCountMin { get; set; }
		public byte ParamsCountMax { get; set; }
		public byte State { get; set; }

		public const int StructSize = 16;
		public Pointer<IPT_CommandElement> LST2_Next => new Pointer<IPT_CommandElement>(Offset + StructSize);
		public Pointer<IPT_CommandElement> LST2_Previous => new Pointer<IPT_CommandElement>(Offset - StructSize);

		public override void SerializeImpl(SerializerObject s) {
			CommandName = s.SerializePointer<string>(CommandName, name: nameof(CommandName))?.ResolveString(s);
			Command = s.SerializePointer<string>(Command, name: nameof(Command))?.ResolveString(s);
			HelpLine = s.SerializePointer<string>(HelpLine, name: nameof(HelpLine))?.ResolveString(s);
			ParamsCountMin = s.Serialize<byte>(ParamsCountMin, name: nameof(ParamsCountMin));
			ParamsCountMax = s.Serialize<byte>(ParamsCountMax, name: nameof(ParamsCountMax));
			State = s.Serialize<byte>(State, name: nameof(State));
			s.SerializePadding(1, logIfNotNull: true);
		}
	}
}
