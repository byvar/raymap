namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ANIM_AnimationIndex : BinarySerializable
	{
		public uint Index { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Index = s.Serialize<uint>(Index, name: nameof(Index));
		}
	}
}