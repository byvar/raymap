namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_ZDXSphere : BinarySerializable
	{
		public uint Radius { get; set; }
		public short X { get; set; }
		public short Y { get; set; }
		public short Z { get; set; }
		public uint GameMaterial { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Radius = s.Serialize<uint>(Radius, name: nameof(Radius));
			X = s.Serialize<short>(X, name: nameof(X));
			Y = s.Serialize<short>(Y, name: nameof(Y));
			Z = s.Serialize<short>(Z, name: nameof(Z));
			s.SerializePadding(2, logIfNotNull: true);
			GameMaterial = s.Serialize<uint>(GameMaterial, name: nameof(GameMaterial));
		}
	}
}