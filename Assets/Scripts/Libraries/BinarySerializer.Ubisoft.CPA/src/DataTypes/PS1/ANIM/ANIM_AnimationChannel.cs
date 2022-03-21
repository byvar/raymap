namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ANIM_AnimationChannel : BinarySerializable
	{
		public Pointer FramesPointer { get; set; }
		public ushort FramesCount { get; set; }
		public ushort ID { get; set; }

		// Serialized from pointers
		public ANIM_AnimationKeyframe[] Frames { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			ID = s.Serialize<ushort>(ID, name: nameof(ID));

			// Serialize data from pointers
			s.DoAt(FramesPointer, () => 
				Frames = s.SerializeObjectArray<ANIM_AnimationKeyframe>(Frames, FramesCount, name: nameof(Frames)));
		}
	}
}