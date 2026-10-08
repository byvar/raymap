using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class USND_UUID : BinarySerializable, ISerializerShortLog, IEquatable<USND_UUID> {
		public uint HighWord { get; set; }
		public uint LowWord { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			HighWord = s.Serialize<uint>(HighWord, name: nameof(HighWord));
			LowWord = s.Serialize<uint>(LowWord, name: nameof(LowWord));
		}

		public string ShortLog => ToString();
		public override string ToString() => $"( 0x{HighWord:X8}, 0x{LowWord:X8} )";

		public long Combined => BitHelpers.SetBits64(LowWord, HighWord, 32, 32);

		#region IEquatable
		public override bool Equals(object obj) {
			return obj is USND_UUID && this == (USND_UUID)obj;
		}
		public override int GetHashCode() {
			return Combined.GetHashCode();
		}

		public bool Equals(USND_UUID other) {
			return this == (USND_UUID)other;
		}

		public static bool operator ==(USND_UUID x, USND_UUID y) {
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			return x.HighWord == y.HighWord && x.LowWord == y.LowWord;
		}
		public static bool operator !=(USND_UUID x, USND_UUID y) {
			return !(x == y);
		}
		#endregion
	}
}