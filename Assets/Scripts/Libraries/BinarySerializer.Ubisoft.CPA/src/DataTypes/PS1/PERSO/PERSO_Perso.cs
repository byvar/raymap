namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PERSO_Perso : BinarySerializable
	{
		public Pointer P3DDataPointer { get; set; }
		public Pointer SuperObjectReferencePointer { get; set; }
		public Pointer Pointer_08 { get; set; } // Dynamics
		public Pointer Pointer_0C { get; set; } // Struct with size 0x18
		public Pointer CollSetPointer { get; set; }
		public Pointer SectorSuperObjectPointer { get; set; }

		// Serialized from pointers
		public PERSO_Perso3DData Perso3DData { get; set; }
		public Pointer SuperObjectPointer { get; set; }
		public Pointer NamePointer { get; set; }
		public string Name { get; set; }
		public HIE_SuperObject SuperObject { get; set; }
		public CS_CollSet CollSet { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			P3DDataPointer = s.SerializePointer(P3DDataPointer, name: nameof(P3DDataPointer));
			SuperObjectReferencePointer = s.SerializePointer(SuperObjectReferencePointer, name: nameof(SuperObjectReferencePointer));
			Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
			Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
			CollSetPointer = s.SerializePointer(CollSetPointer, name: nameof(CollSetPointer));
			SectorSuperObjectPointer = s.SerializePointer(SectorSuperObjectPointer, name: nameof(SectorSuperObjectPointer));

			// Serialize data from pointers
			s.DoAt(P3DDataPointer, () => Perso3DData = s.SerializeObject<PERSO_Perso3DData>(Perso3DData, name: nameof(Perso3DData)));
			s.DoAt(SuperObjectReferencePointer, () =>
			{
				SuperObjectPointer = s.SerializePointer(SuperObjectPointer, name: nameof(SuperObjectPointer));

				CPA_Settings settings = s.GetCPASettings();

				if (settings.EngineVersion == EngineVersion.RaymanRush_PS1)
				{
					NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

					s.DoAt(NamePointer, () => Name = s.SerializeString(Name, name: nameof(Name)));
				}
				else
				{
					Name = s.SerializeString(Name, name: nameof(Name));
				}

				s.DoAt(SuperObjectPointer, () => 
					SuperObject = s.SerializeObject<HIE_SuperObject>(SuperObject, name: nameof(SuperObject)));
			});
			s.DoAt(CollSetPointer, () => CollSet = s.SerializeObject<CS_CollSet>(CollSet, name: nameof(CollSet)));
		}
	}
}