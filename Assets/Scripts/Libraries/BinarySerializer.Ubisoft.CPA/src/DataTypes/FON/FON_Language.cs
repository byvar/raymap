namespace BinarySerializer.Ubisoft.CPA {
	public class FON_Language : BinarySerializable {
		Pointer<Pointer<string>[]> Text { get; set; }
		public ushort MaxTextCount { get; set; }
		public ushort TextCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Text = s.SerializePointer<Pointer<string>[]>(Text, name: nameof(Text));
			MaxTextCount = s.Serialize<ushort>(MaxTextCount, name: nameof(MaxTextCount));
			TextCount = s.Serialize<ushort>(TextCount, name: nameof(TextCount));

			Text?.ResolvePointerArray(s, MaxTextCount);
			Text.Value.ResolveString(s);
		}
	}
}
