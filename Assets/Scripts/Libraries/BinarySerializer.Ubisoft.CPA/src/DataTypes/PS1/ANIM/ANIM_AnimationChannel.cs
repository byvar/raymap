namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ANIM_AnimationChannel : BinarySerializable
	{
		public Pointer<ANIM_AnimationKeyframe[]> Frames { get; set; }
		public ushort FramesCount { get; set; }
		public ushort ID { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Frames = s.SerializePointer<ANIM_AnimationKeyframe[]>(Frames, name: nameof(Frames));
			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			ID = s.Serialize<ushort>(ID, name: nameof(ID));

			// Serialize data from pointers
			Frames?.ResolveObjectArray(s, FramesCount);
		}
	}
}