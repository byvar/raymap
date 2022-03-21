namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ANIM_AnimationBoneChannelLinks : BinarySerializable
	{
		public ushort NTTOChannelIndex { get; set; }
		public ushort IndicesCount { get; set; }
		public Pointer IndicesPointer { get; set; } // Channels

		// Serialized from pointers
		public ushort[] Indices { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			NTTOChannelIndex = s.Serialize<ushort>(NTTOChannelIndex, name: nameof(NTTOChannelIndex));
			IndicesCount = s.Serialize<ushort>(IndicesCount, name: nameof(IndicesCount));
			IndicesPointer = s.SerializePointer(IndicesPointer, name: nameof(IndicesPointer));

			// Serialize data from pointers
			s.DoAt(IndicesPointer, () => Indices = s.SerializeArray<ushort>(Indices, IndicesCount, name: nameof(Indices)));
		}
	}
}