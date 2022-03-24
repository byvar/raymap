namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_Vertex : BinarySerializable
	{
		public short X { get; set; }
		public short Y { get; set; }
		public short Z { get; set; }
		public ushort Ushort_06 { get; set; }
		public RGB777Color Color { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			X = s.Serialize<short>(X, name: nameof(X));
			Y = s.Serialize<short>(Y, name: nameof(Y));
			Z = s.Serialize<short>(Z, name: nameof(Z));
			Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
			Color = s.SerializeObject<RGB777Color>(Color, name: nameof(Color));
			s.SerializePadding(1, logIfNotNull: true);
		}
	}
}