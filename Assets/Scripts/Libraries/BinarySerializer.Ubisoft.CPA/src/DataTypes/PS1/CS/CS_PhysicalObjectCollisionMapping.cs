namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_PhysicalObjectCollisionMapping : BinarySerializable
	{
		public Pointer CollisionPointer { get; set; }
		public Pointer POListEntryPointer { get; set; }
		public byte[] Bytes_08 { get; set; }

		// Serialized from pointers
		public CS_PhysicalObjectCollision Collision { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CollisionPointer = s.SerializePointer(CollisionPointer, name: nameof(CollisionPointer));
			POListEntryPointer = s.SerializePointer(POListEntryPointer, name: nameof(POListEntryPointer));
			Bytes_08 = s.SerializeArray<byte>(Bytes_08, 0x24, name: nameof(Bytes_08));

			// Serialize data from pointers
			s.DoAt(CollisionPointer, () => Collision = s.SerializeObject<CS_PhysicalObjectCollision>(Collision, name: nameof(Collision)));
		}
	}
}