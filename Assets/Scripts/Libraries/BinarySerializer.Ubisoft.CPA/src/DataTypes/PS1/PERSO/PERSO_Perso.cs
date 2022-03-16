namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PERSO_Perso : BinarySerializable
	{
		public Pointer P3DDataPointer { get; set; }
		public Pointer SuperObjectPointer { get; set; }
		public Pointer Pointer_08 { get; set; } // Dynamics
		public Pointer Pointer_0C { get; set; } // Struct with size 0x18
		public Pointer CollSetPointer { get; set; }
		public Pointer SectorSuperObjectPointer { get; set; }

		// Serialized from pointers
		public PERSO_Perso3DData Perso3DData { get; set; }
		// TODO: Serialize remaining data

		public override void SerializeImpl(SerializerObject s)
		{
			P3DDataPointer = s.SerializePointer(P3DDataPointer, name: nameof(P3DDataPointer));
			SuperObjectPointer = s.SerializePointer(SuperObjectPointer, name: nameof(SuperObjectPointer));
			Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
			Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
			CollSetPointer = s.SerializePointer(CollSetPointer, name: nameof(CollSetPointer));
			SectorSuperObjectPointer = s.SerializePointer(SectorSuperObjectPointer, name: nameof(SectorSuperObjectPointer));

			// Serialize data from pointers
			s.DoAt(P3DDataPointer, () => Perso3DData = s.SerializeObject<PERSO_Perso3DData>(Perso3DData, name: nameof(Perso3DData)));
		}
	}
}