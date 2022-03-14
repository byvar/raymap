namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class WAY_WayPoint : BinarySerializable
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public short Short_0C { get; set; }
		public short Short_0E { get; set; }
		public short Radius { get; set; }
		public short Short_12 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			X = s.Serialize<int>(X, name: nameof(X));
			Y = s.Serialize<int>(Y, name: nameof(Y));
			Z = s.Serialize<int>(Z, name: nameof(Z));
			Short_0C = s.Serialize<short>(Short_0C, name: nameof(Short_0C));
			Short_0E = s.Serialize<short>(Short_0E, name: nameof(Short_0E));
			Radius = s.Serialize<short>(Radius, name: nameof(Radius));
			Short_12 = s.Serialize<short>(Short_12, name: nameof(Short_12));
		}
	}
}