namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_KeyAndPadDefine : BinarySerializable {
		public ushort BasedKey { get; set; } = 0x7FFF;
		public bool IsPad { get; set; } = true;
		public Pointer FrenchKeyPointer { get; set; }
		public Pointer AmericanKeyPointer { get; set; }

		public string FrenchKey { get; set; }
		public string AmericanKey { get; set; }


		public bool IsInvalid => BasedKey == 0x7FFF && IsPad; // BasedKey value is 0xFFFF

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<ushort>(b => {
				BasedKey = b.SerializeBits<ushort>(BasedKey, 15, name: nameof(BasedKey));
				IsPad = b.SerializeBits<bool>(IsPad, 1, name: nameof(IsPad));
			});
			s.SerializePadding(2, logIfNotNull: true);
			FrenchKeyPointer = s.SerializePointer(FrenchKeyPointer, allowInvalid: true, name: nameof(FrenchKeyPointer)); // Allow invalid: these strings are often not present in the SNA
			AmericanKeyPointer = s.SerializePointer(AmericanKeyPointer, allowInvalid: true, name: nameof(AmericanKeyPointer));

			s.DoAt(FrenchKeyPointer, () => {
				FrenchKey = s.SerializeString(FrenchKey, name: nameof(FrenchKey));
			});
			s.DoAt(AmericanKeyPointer, () => {
				AmericanKey = s.SerializeString(AmericanKey, name: nameof(AmericanKey));
			});
		}
	}
}
