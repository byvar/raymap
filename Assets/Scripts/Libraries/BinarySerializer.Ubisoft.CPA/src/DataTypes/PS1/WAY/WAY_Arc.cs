namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class WAY_Arc : BinarySerializable
	{
		public Pointer Node1Pointer { get; set; }
		public Pointer Node2Pointer { get; set; }
		public uint Uint_08 { get; set; }
		public ushort Ushort_0C { get; set; }
		public ushort Ushort_0E { get; set; }

		// Serialized from pointers
		public WAY_WayPoint Node1 { get; set; }
		public WAY_WayPoint Node2 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Node1Pointer = s.SerializePointer(Node1Pointer, name: nameof(Node1Pointer));
			Node2Pointer = s.SerializePointer(Node2Pointer, name: nameof(Node2Pointer));
			Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
			Ushort_0C = s.Serialize<ushort>(Ushort_0C, name: nameof(Ushort_0C));
			Ushort_0E = s.Serialize<ushort>(Ushort_0E, name: nameof(Ushort_0E));

			// Serialize data from pointers
			s.DoAt(Node1Pointer, () => Node1 = s.SerializeObject<WAY_WayPoint>(Node1, name: nameof(Node1)));
			s.DoAt(Node2Pointer, () => Node2 = s.SerializeObject<WAY_WayPoint>(Node2, name: nameof(Node2)));
		}
	}
}