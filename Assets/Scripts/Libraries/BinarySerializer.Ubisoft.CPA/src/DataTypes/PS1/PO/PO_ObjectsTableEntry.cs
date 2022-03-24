namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PO_ObjectsTableEntry : BinarySerializable
	{
		public Pointer Pointer_00 { get; set; } // Object of 0x50, 5 rows of 0x10
		public Pointer GeometricObjectPointer { get; set; }

		// Serialized from pointers
		public GEO_GeometricObject GeometricObject { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Pointer_00 = s.SerializePointer(Pointer_00, name: nameof(Pointer_00));
			GeometricObjectPointer = s.SerializePointer(GeometricObjectPointer, name: nameof(GeometricObjectPointer));

			// Serialize data from pointers
			s.DoAt(GeometricObjectPointer, () => 
				GeometricObject = s.SerializeObject<GEO_GeometricObject>(GeometricObject, name: nameof(GeometricObject)));
		}
	}
}