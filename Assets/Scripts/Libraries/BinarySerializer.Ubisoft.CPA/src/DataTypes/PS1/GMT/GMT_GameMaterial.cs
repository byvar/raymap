namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GMT_GameMaterial : BinarySerializable
	{
		public ushort Ushort_00 { get; set; }
		public ushort Ushort_02 { get; set; }
		public Pointer<GMT_CollisionMaterial> CollisionMaterial { get; set; }
		public int Int_08 { get; set; }
		public uint Uint_0C { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
			Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
			CollisionMaterial = s.SerializePointer(CollisionMaterial, name: nameof(CollisionMaterial))?.Resolve(s);
			Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
			Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
		}
	}
}