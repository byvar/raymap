namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GMT_CollideMaterial : BinarySerializable
	{
		public ushort Type { get; set; }
		public ushort Identifier { get; set; } // TODO: Flags

		public override void SerializeImpl(SerializerObject s)
		{
			Type = s.Serialize<ushort>(Type, name: nameof(Type));
			Identifier = s.Serialize<ushort>(Identifier, name: nameof(Identifier));
		}
	}
}