namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ANIM_AnimationBoneChannelLinks : BinarySerializable
	{
		public ushort NTTOChannelIndex { get; set; }
		public ushort IndicesCount { get; set; }
		public Pointer<ushort[]> Indices { get; set; } // Channels

		public override void SerializeImpl(SerializerObject s)
		{
			NTTOChannelIndex = s.Serialize<ushort>(NTTOChannelIndex, name: nameof(NTTOChannelIndex));
			IndicesCount = s.Serialize<ushort>(IndicesCount, name: nameof(IndicesCount));
			Indices = s.SerializePointer(Indices, name: nameof(Indices))?.ResolveValueArray(s, IndicesCount);
		}
	}
}