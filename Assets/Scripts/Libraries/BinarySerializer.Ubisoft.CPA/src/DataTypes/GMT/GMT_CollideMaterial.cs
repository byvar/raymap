namespace BinarySerializer.Ubisoft.CPA
{
	public class GMT_CollideMaterial : BinarySerializable
	{
		public GMT_ZoneType ZoneType { get; set; }
		public ushort Identifier { get; set; } // TODO: Flags

		public MTH3D_Vector Direction { get; set; }
		public float Coeff { get; set; }
		public ushort TypeForAI { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			ZoneType = s.Serialize<GMT_ZoneType>(ZoneType, name: nameof(ZoneType));
			Identifier = s.Serialize<ushort>(Identifier, name: nameof(Identifier));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Direction = s.SerializeObject<MTH3D_Vector>(Direction, name: nameof(Direction));
				Coeff = s.Serialize<float>(Coeff, name: nameof(Coeff));
				TypeForAI = s.Serialize<ushort>(TypeForAI, name: nameof(TypeForAI));
				s.SerializePadding(2, logIfNotNull: true);
			}
		}
	}
}