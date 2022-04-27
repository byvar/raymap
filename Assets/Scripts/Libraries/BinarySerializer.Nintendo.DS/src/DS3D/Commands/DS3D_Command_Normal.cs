namespace BinarySerializer.Nintendo.DS {
	public class DS3D_Command_Normal : DS3D_CommandData {
		public short XShort { get; set; }
		public short YShort { get; set; }
		public short ZShort { get; set; }

		private const float Divisor = (1 << 9);

		public float X {
			get => XShort / Divisor;
			set => XShort = (short)(value * Divisor);
		}
		public float Y {
			get => YShort / Divisor;
			set => YShort = (short)(value * Divisor);
		}
		public float Z {
			get => ZShort / Divisor;
			set => ZShort = (short)(value * Divisor);
		}

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<uint>(b => {
				XShort = b.SerializeBits<short>(XShort, 10, sign: SignedNumberRepresentation.TwosComplement, name: nameof(XShort));
				YShort = b.SerializeBits<short>(YShort, 10, sign: SignedNumberRepresentation.TwosComplement, name: nameof(YShort));
				ZShort = b.SerializeBits<short>(ZShort, 10, sign: SignedNumberRepresentation.TwosComplement, name: nameof(ZShort));
			});
		}
		public override bool UseShortLog => true;
		public override string ToString() => $"{GetType()}({X}, {Y}, {Z})";
	}
}
