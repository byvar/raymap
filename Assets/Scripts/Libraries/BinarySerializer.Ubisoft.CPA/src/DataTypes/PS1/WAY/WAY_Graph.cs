namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class WAY_Graph : BinarySerializable
	{
		public int ArcsCount { get; set; }
		public Pointer FirstNodePointer { get; set; }
		public Pointer ArcsPointer { get; set; }
		public int NodesCount { get; set; }
		public byte[] Bytes_10 { get; set; }

		// Serialized from pointers
		public WAY_Arc[] Arcs { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			ArcsCount = s.Serialize<int>(ArcsCount, name: nameof(ArcsCount));
			FirstNodePointer = s.SerializePointer(FirstNodePointer, name: nameof(FirstNodePointer));
			ArcsPointer = s.SerializePointer(ArcsPointer, name: nameof(ArcsPointer));
			NodesCount = s.Serialize<int>(NodesCount, name: nameof(NodesCount));
			Bytes_10 = s.SerializeArray<byte>(Bytes_10, 0x58, name: nameof(Bytes_10));

			// Serialize data from pointers
			s.DoAt(ArcsPointer, () => Arcs = s.SerializeObjectArray<WAY_Arc>(Arcs, ArcsCount, name: nameof(Arcs)));
		}
	}
}