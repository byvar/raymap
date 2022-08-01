namespace BinarySerializer.Ubisoft.CPA {
	public class COL_OctreeIndex : BinarySerializable {
		public byte Byte0 { get; set; }
		public bool IsOverflow { get; set; }
		public byte Byte1 { get; set; }
		public ushort Value {
			get {
				if (IsOverflow) {
					return (ushort)(Byte1 | (Byte0 << 8));
				} else 
					return Byte0;
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<byte>(b => {
				Byte0 = b.SerializeBits<byte>(Byte0, 7, name: nameof(Byte0));
				IsOverflow = b.SerializeBits<bool>(IsOverflow, 1, name: nameof(IsOverflow));
			});
			if (IsOverflow) {
				Byte1 = s.Serialize<byte>(Byte1, name: nameof(Byte1));
			}
		}

		public override bool UseShortLog => true;
		public override string ToString() => Value.ToString();
	}
}
