namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_QuadNoTexture : BinarySerializable
	{
		public ushort V0 { get; set; }
		public ushort V1 { get; set; }
		public ushort V2 { get; set; }
		public ushort V3 { get; set; }
		public ushort Ushort_08 { get; set; }
		public ushort Ushort_0A { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			V0 = s.Serialize<ushort>(V0, name: nameof(V0));
			V1 = s.Serialize<ushort>(V1, name: nameof(V1));
			V2 = s.Serialize<ushort>(V2, name: nameof(V2));
			V3 = s.Serialize<ushort>(V3, name: nameof(V3));
			Ushort_08 = s.Serialize<ushort>(Ushort_08, name: nameof(Ushort_08));
			Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
		}
	}
}