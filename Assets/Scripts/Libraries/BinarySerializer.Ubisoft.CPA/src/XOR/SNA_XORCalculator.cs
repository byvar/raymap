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

		public static uint GetCryptKey(long value)      => GetPseudoRandomKey(value, 16807); // 7*7*7*7*7
		public static uint GetProtectionKey(long value) => GetPseudoRandomKey(value, 48271); // prime

		/// <summary>
		/// Pseudo-random generator based on Minimal Standard by Lewis, Goodman, and Miller in 1969.
		/// Algorithm presented in "Random Number Generators: Good Ones Are Hard To Find".
		/// </summary>
		/// <param name="seed">Starting value</param>
		/// <param name="a">Factor of low component after division</param>
		/// <returns></returns>
		private static uint GetPseudoRandomKey(long seed, long a) {
			const long seedXOR = 123459876;
			long q = int.MaxValue / a; // For CryptKey: 127773
			long r = int.MaxValue % a; // For CryptKey: 2836

			seed ^= seedXOR;
			long hi = seed / q;
			long lo = seed % q;
			seed = a * lo - r * hi;
			return (uint)BitHelpers.ExtractBits64(seed, 32, 0);
		}
	}
}