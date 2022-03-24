namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_QuadLOD : BinarySerializable
	{
		public ushort V0 { get; set; }
		public ushort V1 { get; set; }
		public ushort V2 { get; set; }
		public ushort V3 { get; set; }
		public ushort Ushort_08 { get; set; }
		public ushort Ushort_0A { get; set; }
		public ushort Ushort_0C { get; set; }
		public ushort Ushort_0E { get; set; }
		public ushort Ushort_10 { get; set; }
		public ushort Ushort_12 { get; set; }
		public ushort Ushort_14 { get; set; }
		public ushort Ushort_16 { get; set; }
		public Pointer QuadsPointer { get; set; }

		// Serialized from pointers
		public uint QuadsCount { get; set; }
		public GEO_Quad[] Quads { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			V0 = s.Serialize<ushort>(V0, name: nameof(V0));
			V1 = s.Serialize<ushort>(V1, name: nameof(V1));
			V2 = s.Serialize<ushort>(V2, name: nameof(V2));
			V3 = s.Serialize<ushort>(V3, name: nameof(V3));
			Ushort_08 = s.Serialize<ushort>(Ushort_08, name: nameof(Ushort_08));
			Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
			Ushort_0C = s.Serialize<ushort>(Ushort_0C, name: nameof(Ushort_0C));
			Ushort_0E = s.Serialize<ushort>(Ushort_0E, name: nameof(Ushort_0E));
			Ushort_10 = s.Serialize<ushort>(Ushort_10, name: nameof(Ushort_10));
			Ushort_12 = s.Serialize<ushort>(Ushort_12, name: nameof(Ushort_12));
			Ushort_14 = s.Serialize<ushort>(Ushort_14, name: nameof(Ushort_14));
			Ushort_16 = s.Serialize<ushort>(Ushort_16, name: nameof(Ushort_16));
			QuadsPointer = s.SerializePointer(QuadsPointer, name: nameof(QuadsPointer));

			// Serialize data from pointers
			s.DoAt(QuadsPointer, () =>
			{
				QuadsCount = s.Serialize<uint>(QuadsCount, name: nameof(QuadsCount));
				Quads = s.SerializeObjectArray<GEO_Quad>(Quads, QuadsCount, name: nameof(Quads));
			});
		}
	}
}