namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class COL_GeometricObjectCollidePolygon : BinarySerializable
	{
		public bool Pre_IsQuad { get; set; }

		public byte Flag0 { get; set; }
		public byte GameMaterial { get; set; }
		public ushort Normal { get; set; }

		// Vertices
		public ushort V0 { get; set; }
		public ushort V1 { get; set; }
		public ushort V2 { get; set; }
		public ushort V3 { get; set; }

		// Unknown
		public short X0 { get; set; }
		public short Y0 { get; set; }
		public short Z0 { get; set; }
		public short X1 { get; set; }
		public short Y1 { get; set; }
		public short Z1 { get; set; }
		public short X2 { get; set; }
		public short Y2 { get; set; }
		public short Z2 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Flag0 = s.Serialize<byte>(Flag0, name: nameof(Flag0));
			GameMaterial = s.Serialize<byte>(GameMaterial, name: nameof(GameMaterial));
			Normal = s.Serialize<ushort>(Normal, name: nameof(Normal));

			V0 = s.Serialize<ushort>(V0, name: nameof(V0));
			V1 = s.Serialize<ushort>(V1, name: nameof(V1));
			V2 = s.Serialize<ushort>(V2, name: nameof(V2));

			if (Pre_IsQuad)
				V3 = s.Serialize<ushort>(V3, name: nameof(V3));

			X0 = s.Serialize<short>(X0, name: nameof(X0));
			Y0 = s.Serialize<short>(Y0, name: nameof(Y0));
			Z0 = s.Serialize<short>(Z0, name: nameof(Z0));
			X1 = s.Serialize<short>(X1, name: nameof(X1));
			Y1 = s.Serialize<short>(Y1, name: nameof(Y1));
			Z1 = s.Serialize<short>(Z1, name: nameof(Z1));
			X2 = s.Serialize<short>(X2, name: nameof(X2));
			Y2 = s.Serialize<short>(Y2, name: nameof(Y2));
			
			if (!Pre_IsQuad)
				Z2 = s.Serialize<short>(Z2, name: nameof(Z2));
		}
	}
}