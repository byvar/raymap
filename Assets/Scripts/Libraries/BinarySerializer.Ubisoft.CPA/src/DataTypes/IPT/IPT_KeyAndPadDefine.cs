namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_KeyAndPadDefine : BinarySerializable {
		public ushort BasedKey { get; set; } = 0x7FFF;
		public bool IsPad { get; set; } = true;
		public Pointer<string> FrenchKey { get; set; }
		public Pointer<string> AmericanKey { get; set; }


		public bool IsInvalid => BasedKey == 0x7FFF && IsPad; // BasedKey value is 0xFFFF

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<ushort>(b => {
				BasedKey = b.SerializeBits<ushort>(BasedKey, 15, name: nameof(BasedKey));
				IsPad = b.SerializeBits<bool>(IsPad, 1, name: nameof(IsPad));
			});
			s.SerializePadding(2, logIfNotNull: true);
			FrenchKey = s.SerializePointer<string>(FrenchKey, allowInvalid: true, name: nameof(FrenchKey))?.ResolveString(s); // Allow invalid: these strings are often not present in the SNA
			AmericanKey = s.SerializePointer<string>(AmericanKey, allowInvalid: true, name: nameof(AmericanKey))?.ResolveString(s);
		}
	}
}
