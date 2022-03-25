namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CAM_CameraGraphNode : BinarySerializable
	{
		public Pointer PreviousPointer { get; set; }
		public Pointer NextPointer { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public uint Uint_14 { get; set; }
		public uint Uint_18 { get; set; }

		// Serialized from pointers
		public CAM_CameraGraphNode Previous { get; set; }
		public CAM_CameraGraphNode Next { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			PreviousPointer = s.SerializePointer(PreviousPointer, name: nameof(PreviousPointer));
			NextPointer = s.SerializePointer(NextPointer, name: nameof(NextPointer));
			X = s.Serialize<int>(X, name: nameof(X));
			Y = s.Serialize<int>(Y, name: nameof(Y));
			Z = s.Serialize<int>(Z, name: nameof(Z));
			Uint_14 = s.Serialize<uint>(Uint_14, name: nameof(Uint_14));
			Uint_18 = s.Serialize<uint>(Uint_18, name: nameof(Uint_18));

			// Serialize data from pointers
			s.DoAt(PreviousPointer, () => Previous = s.SerializeObject<CAM_CameraGraphNode>(Previous, name: nameof(Previous)));
			s.DoAt(NextPointer, () => Next = s.SerializeObject<CAM_CameraGraphNode>(Next, name: nameof(Next)));
		}
	}
}