namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_TriangleNoTexture : BinarySerializable, GEO_IPS1Polygon
	{
		public ushort V0 { get; set; }
		public ushort V1 { get; set; }
		public ushort V2 { get; set; }
		public ushort Ushort_06 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			V0 = s.Serialize<ushort>(V0, name: nameof(V0));
			V1 = s.Serialize<ushort>(V1, name: nameof(V1));
			V2 = s.Serialize<ushort>(V2, name: nameof(V2));
			Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
		}

		#region GEO_IPS1Polygon implementation
		public GLI_Texture Texture => null;

		public GLI_VisualMaterial Material => null;

		public void RegisterTexture() {
			throw new System.NotImplementedException();
		}
		#endregion
	}
}