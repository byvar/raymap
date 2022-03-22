namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_CollSet : BinarySerializable
	{
		// TODO: Handle R2 data - depends on states
		public uint R2_Uint_00 { get; set; }
		public uint R2_Uint_04 { get; set; }
		public uint R2_Uint_08 { get; set; }
		public uint R2_Uint_0C { get; set; }

		public Pointer ZDDPointer { get; set; }
		public Pointer ZDEPointer { get; set; }
		public Pointer ZDMPointer { get; set; }
		public Pointer ZDRPointer { get; set; }

		public Pointer ActivationListPointer { get; set; }

		public byte Byte_14 { get; set; }
		public byte Byte_15 { get; set; }
		public byte Byte_16 { get; set; }
		public byte Byte_17 { get; set; }

		// Serialized from pointers
		public CS_ZDXList ZDD { get; set; }
		public CS_ZDXList ZDE { get; set; }
		public CS_ZDXList ZDM { get; set; }
		public CS_ZDXList ZDR { get; set; }
		public CS_ActivationList ActivationList { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			if (settings.EngineVersion == EngineVersion.Rayman2_PS1)
			{
				R2_Uint_00 = s.Serialize<uint>(R2_Uint_00, name: nameof(R2_Uint_00));
				R2_Uint_04 = s.Serialize<uint>(R2_Uint_04, name: nameof(R2_Uint_04));
				R2_Uint_08 = s.Serialize<uint>(R2_Uint_08, name: nameof(R2_Uint_08));
				R2_Uint_0C = s.Serialize<uint>(R2_Uint_0C, name: nameof(R2_Uint_0C));
			}
			else
			{
				ZDDPointer = s.SerializePointer(ZDDPointer, name: nameof(ZDDPointer));
				ZDEPointer = s.SerializePointer(ZDEPointer, name: nameof(ZDEPointer));
				ZDMPointer = s.SerializePointer(ZDMPointer, name: nameof(ZDMPointer));
				ZDRPointer = s.SerializePointer(ZDRPointer, name: nameof(ZDRPointer));
			}

			ActivationListPointer = s.SerializePointer(ActivationListPointer, name: nameof(ActivationListPointer));

			Byte_14 = s.Serialize<byte>(Byte_14, name: nameof(Byte_14));
			Byte_15 = s.Serialize<byte>(Byte_15, name: nameof(Byte_15));
			Byte_16 = s.Serialize<byte>(Byte_16, name: nameof(Byte_16));
			Byte_17 = s.Serialize<byte>(Byte_17, name: nameof(Byte_17));

			// Serialize data from pointers
			s.DoAt(ZDDPointer, () => ZDD = s.SerializeObject<CS_ZDXList>(ZDD, name: nameof(ZDD)));
			s.DoAt(ZDEPointer, () => ZDE = s.SerializeObject<CS_ZDXList>(ZDE, name: nameof(ZDE)));
			s.DoAt(ZDMPointer, () => ZDM = s.SerializeObject<CS_ZDXList>(ZDM, name: nameof(ZDM)));
			s.DoAt(ZDRPointer, () => ZDR = s.SerializeObject<CS_ZDXList>(ZDR, name: nameof(ZDR)));
			s.DoAt(ActivationListPointer, () => ActivationList = s.SerializeObject<CS_ActivationList>(ActivationList, name: nameof(ActivationList)));
		}
	}
}