namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class MTH3D_ShortVector : U64_Struct, ISerializerShortLog
    {
		public short X { get; set; } // Divide by a custom scale to get float
		public short Y { get; set; }
		public short Z { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			X = s.Serialize<short>(X, name: nameof(X));
			Y = s.Serialize<short>(Y, name: nameof(Y));
			Z = s.Serialize<short>(Z, name: nameof(Z));
		}
		public string ShortLog => ToString();
		public override string ToString() => $"ShortVector3D({X}, {Y}, {Z})";
	}
}