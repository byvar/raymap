namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_ZDXBox : BinarySerializable
	{
		public short X0 { get; set; }
		public short Y0 { get; set; }
		public short Z0 { get; set; }
		public short X1 { get; set; }
		public short Y1 { get; set; }
		public short Z1 { get; set; }
		public uint GameMaterial { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			X0 = s.Serialize<short>(X0, name: nameof(X0));
			Y0 = s.Serialize<short>(Y0, name: nameof(Y0));
			Z0 = s.Serialize<short>(Z0, name: nameof(Z0));
			s.SerializePadding(2, logIfNotNull: true);
			X1 = s.Serialize<short>(X1, name: nameof(X1));
			Y1 = s.Serialize<short>(Y1, name: nameof(Y1));
			Z1 = s.Serialize<short>(Z1, name: nameof(Z1));
			s.SerializePadding(2, logIfNotNull: true);
			GameMaterial = s.Serialize<uint>(GameMaterial, name: nameof(GameMaterial));
		}
	}
}