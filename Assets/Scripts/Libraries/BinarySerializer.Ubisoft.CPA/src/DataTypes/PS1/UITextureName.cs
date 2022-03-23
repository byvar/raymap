namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class UITextureName : BinarySerializable
	{
		public string Name { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Name = s.SerializeString(Name, 0x1C, name: nameof(Name));
		}
	}
}