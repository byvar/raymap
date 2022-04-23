namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PO_ObjectsTable : BinarySerializable
	{
		public uint? Pre_Length { get; set; }

		public uint Uint_00 { get; set; }
		public uint Uint_04 { get; set; }
		public PO_PhysicalObject[] Entries { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
			Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));

			if (Pre_Length != null)
				Entries = s.SerializeObjectArray<PO_PhysicalObject>(Entries, Pre_Length.Value, name: nameof(Entries));
			else
				Entries = s.SerializeObjectArrayUntil<PO_PhysicalObject>(Entries, x => x.GeometricObjectPointer == null, name: nameof(Entries));
		}
	}
}