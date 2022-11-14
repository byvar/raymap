namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_PhysicalObjectCollision : BinarySerializable
	{
		public byte[] Bytes_00 { get; set; }
		public uint CollisionCount { get; set; }
		public Pointer GeoCollidePointer { get; set; }

		// Serialized from pointers
		public COL_GeometricObjectCollide GeoCollide { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Bytes_00 = s.SerializeArray<byte>(Bytes_00, 0x10, name: nameof(Bytes_00));
			CollisionCount = s.Serialize<uint>(CollisionCount, name: nameof(CollisionCount));
			GeoCollidePointer = s.SerializePointer(GeoCollidePointer, allowInvalid: CollisionCount == 0, name: nameof(GeoCollidePointer));

			if(CollisionCount > 1)
				s.SystemLogger?.LogWarning($"{Offset}: Encountered a {nameof(CS_PhysicalObjectCollision)} with {nameof(CollisionCount)} > 1! Check the data to find out how this is serialized");
			// Serialize data from pointers
			s.DoAt(GeoCollidePointer, () => 
				GeoCollide = s.SerializeObject<COL_GeometricObjectCollide>(GeoCollide, name: nameof(GeoCollide)));
		}
	}
}