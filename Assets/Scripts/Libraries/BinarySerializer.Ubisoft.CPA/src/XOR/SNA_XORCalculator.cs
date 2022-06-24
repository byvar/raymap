namespace BinarySerializer.Ubisoft.CPA
{
    /// <summary>
    /// Class for XOR operations with a multi-byte key
    /// </summary>
    public class SNA_XORCalculator : IXORCalculator 
    {
        public SNA_XORCalculator(uint key = 0x6AB5CC79, DecodeMode mode = DecodeMode.Rayman2)
        {
            CryptKey = key;
			Mode = mode;
        }

        public uint CryptKey { get; protected set; }
		public byte GetKeyByte(int i) => (byte)BitHelpers.ExtractBits64(CryptKey, 8, i*8);
		public void SetKeyByte(int i, byte b) {
			CryptKey = (uint)BitHelpers.SetBits64(CryptKey, b, 8, i * 8);
		}

        public byte XORByte(byte b) 
        {
			b = DecodeByte(b);
			UpdateCryptKey();
			return b;
        }

		public enum DecodeMode {
			RedPlanet, // Pre-Rayman2
			Rayman2,
		}

		public DecodeMode Mode { get; protected set; } = DecodeMode.Rayman2;

		private byte DecodeByte(byte toDecode) {
			if (Mode == DecodeMode.RedPlanet) {
				return (byte)((GetKeyByte(1) ^ ((toDecode + 0x100) - GetKeyByte(0))) & 0xFF);
			} else {
				return (byte)(toDecode ^ GetKeyByte(1));
			}
		}

		private void UpdateCryptKey() {
			if (Mode == DecodeMode.RedPlanet) {
				SetKeyByte(0, (byte)((GetKeyByte(0) + GetKeyByte(2)) & 0xFF));
				SetKeyByte(1, (byte)((GetKeyByte(1) + GetKeyByte(3)) & 0xFF));
			} else {
				CryptKey = GetCryptKey(CryptKey);
			}
		}

		public static uint GetCryptKey(long value)      => GetPseudoRandomKey(value, 123459876, 127773, 16807, 2836);
		public static uint GetProtectionKey(long value) => GetPseudoRandomKey(value, 123459876, 44488,  48271, 3399);

		/// <summary>
		/// Pseudo-random generator based on Minimal Standard by Lewis, Goodman, and Miller in 1969.
		/// </summary>
		/// <param name="value">Starting value</param>
		/// <param name="seed">Seed</param>
		/// <param name="divisor">Divisor</param>
		/// <param name="loFactor">Factor of low component after division</param>
		/// <param name="hiFactor">Factor of high component after division</param>
		/// <returns></returns>
		private static uint GetPseudoRandomKey(long value, long seed, long divisor, long loFactor, long hiFactor) {
			value ^= seed;
			long hi = value / divisor;
			long lo = value % divisor;
			value = loFactor * lo - hiFactor * hi;
			return (uint)BitHelpers.ExtractBits64(value, 32, 0);
			//if (value < 0) value += 0x7FFFFFFF;
			//return (uint)value;
		}
	}
}