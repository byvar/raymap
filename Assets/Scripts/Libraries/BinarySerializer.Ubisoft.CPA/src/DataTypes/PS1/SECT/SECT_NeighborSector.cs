namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class SECT_NeighborSector : BinarySerializable
	{
		public Pointer SectorSuperObjectPointer { get; set; }
		public short Short_04 { get; set; }
		public short Short_06 { get; set; }

		// Serialized from pointers
		public HIE_SuperObject SectorSuperObject { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			SectorSuperObjectPointer = s.SerializePointer(SectorSuperObjectPointer, name: nameof(SectorSuperObjectPointer));

			if (settings.EngineVersion != EngineVersion.DonaldDuckQuackAttack_PS1 && 
			    settings.EngineVersion != EngineVersion.JungleBook_PS1)
			{
				Short_04 = s.Serialize<short>(Short_04, name: nameof(Short_04));
				Short_06 = s.Serialize<short>(Short_06, name: nameof(Short_06));
			}

			// Serialize data from pointers
			s.DoAt(SectorSuperObjectPointer, () => 
				SectorSuperObject = s.SerializeObject<HIE_SuperObject>(SectorSuperObject, name: nameof(SectorSuperObject)));
		}
	}
}