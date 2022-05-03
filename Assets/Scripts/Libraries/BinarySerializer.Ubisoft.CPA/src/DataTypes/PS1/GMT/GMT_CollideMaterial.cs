namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GMT_CollideMaterial : BinarySerializable
	{
		public GMT_ZoneType ZoneType { get; set; }
		public ushort Identifier { get; set; } // TODO: Flags

		public override void SerializeImpl(SerializerObject s)
		{
			ZoneType = s.Serialize<GMT_ZoneType>(ZoneType, name: nameof(ZoneType));
			Identifier = s.Serialize<ushort>(Identifier, name: nameof(Identifier));
		}
	}
}