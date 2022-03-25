namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CAM_CameraGraph : BinarySerializable
	{
		public uint Uint_00 { get; set; }
		public Pointer CurrentPointer { get; set; }
		public Pointer LastPointer { get; set; }
		public Pointer FirstPointer { get; set; }
		public uint Flags { get; set; }

		// Serialized from pointers
		public CAM_CameraGraphNode Current { get; set; }
		public CAM_CameraGraphNode First { get; set; }
		public CAM_CameraGraphNode Last { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
			CurrentPointer = s.SerializePointer(CurrentPointer, name: nameof(CurrentPointer));
			LastPointer = s.SerializePointer(LastPointer, name: nameof(LastPointer));
			FirstPointer = s.SerializePointer(FirstPointer, name: nameof(FirstPointer));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));

			// Serialize data from pointers
			s.DoAt(CurrentPointer, () => Current = s.SerializeObject<CAM_CameraGraphNode>(Current, name: nameof(Current)));
			s.DoAt(FirstPointer, () => First = s.SerializeObject<CAM_CameraGraphNode>(First, name: nameof(First)));
			s.DoAt(LastPointer, () => Last = s.SerializeObject<CAM_CameraGraphNode>(Last, name: nameof(Last)));
		}
	}
}