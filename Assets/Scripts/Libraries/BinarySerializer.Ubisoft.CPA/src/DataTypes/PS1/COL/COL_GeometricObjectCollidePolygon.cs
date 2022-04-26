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
		public COL_GeometricObjectCollideVector Vector1 { get; set; }
		public COL_GeometricObjectCollideVector Vector2 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Flag0 = s.Serialize<byte>(Flag0, name: nameof(Flag0));
			GameMaterial = s.Serialize<byte>(GameMaterial, name: nameof(GameMaterial));
			Normal = s.Serialize<ushort>(Normal, name: nameof(Normal));

			V0 = s.Serialize<ushort>(V0, name: nameof(V0));
			V1 = s.Serialize<ushort>(V1, name: nameof(V1));
			V2 = s.Serialize<ushort>(V2, name: nameof(V2));
			V3 = s.Serialize<ushort>(V3, name: nameof(V3));

			Vector1 = s.SerializeObject<COL_GeometricObjectCollideVector>(Vector1, name: nameof(Vector1));
			Vector2 = s.SerializeObject<COL_GeometricObjectCollideVector>(Vector2, name: nameof(Vector2));
		}
	}
}