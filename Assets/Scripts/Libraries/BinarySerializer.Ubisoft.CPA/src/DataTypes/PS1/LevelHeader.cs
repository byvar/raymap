namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class LevelHeader : BinarySerializable
	{
		public byte[] UnknownBytes1 { get; set; }
		public uint GeometricObjectsDynamicCount_Cine { get; set; }
		public byte[] UnknownBytes2 { get; set; }
		public Pointer DynamicWorldPointer { get; set; }
		public Pointer FatherSectorPointer { get; set; }
		public Pointer InactiveDynamicWorldPointer { get; set; }
		public int AlwaysCount { get; set; }
		public Pointer AlwaysPointer { get; set; }

		// Parsed from pointers
		public HIE_SuperObject DynamicWorld { get; set; }
		public HIE_SuperObject FatherSector { get; set; }
		public HIE_SuperObject InactiveDynamicWorld { get; set; }
		public ALW_AlwaysList[] Always { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			if (settings.EngineVersion == EngineVersion.RaymanRush_PS1)
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0x40, name: nameof(UnknownBytes1));
			}
			else if (settings.EngineVersion == EngineVersion.DonaldDuckQuackAttack_PS1)
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0x58, name: nameof(UnknownBytes1));
			}
			else if (settings.EngineVersion == EngineVersion.VIP_PS1)
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0x28, name: nameof(UnknownBytes1));
			}
			else if (settings.EngineVersion == EngineVersion.JungleBook_PS1)
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0xEC, name: nameof(UnknownBytes1));
			}
			else
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0xCC, name: nameof(UnknownBytes1));
				GeometricObjectsDynamicCount_Cine = s.Serialize<uint>(GeometricObjectsDynamicCount_Cine, name: nameof(GeometricObjectsDynamicCount_Cine));
				UnknownBytes2 = s.SerializeArray<byte>(UnknownBytes2, 0x20, name: nameof(UnknownBytes2));
			}

			DynamicWorldPointer = s.SerializePointer(DynamicWorldPointer, name: nameof(DynamicWorldPointer));
			FatherSectorPointer = s.SerializePointer(FatherSectorPointer, name: nameof(FatherSectorPointer));
			InactiveDynamicWorldPointer = s.SerializePointer(InactiveDynamicWorldPointer, name: nameof(InactiveDynamicWorldPointer));
			AlwaysCount = s.Serialize<int>(AlwaysCount, name: nameof(AlwaysCount));
			AlwaysPointer = s.SerializePointer(AlwaysPointer, name: nameof(AlwaysPointer));
			// TODO: Serialize remaining data


			// Serialize data from pointers
			s.DoAt(DynamicWorldPointer, () => 
				DynamicWorld = s.SerializeObject<HIE_SuperObject>(DynamicWorld, x => x.Pre_IsDynamic = true, name: nameof(DynamicWorld)));
			s.DoAt(FatherSectorPointer, () =>
				FatherSector = s.SerializeObject<HIE_SuperObject>(FatherSector, x => x.Pre_IsDynamic = false, name: nameof(FatherSector)));
			s.DoAt(InactiveDynamicWorldPointer, () =>
				InactiveDynamicWorld = s.SerializeObject<HIE_SuperObject>(InactiveDynamicWorld, x => x.Pre_IsDynamic = true, name: nameof(InactiveDynamicWorld)));
			s.DoAt(AlwaysPointer, () => Always = s.SerializeObjectArray<ALW_AlwaysList>(Always, AlwaysCount, name: nameof(Always)));
		}
	}
}