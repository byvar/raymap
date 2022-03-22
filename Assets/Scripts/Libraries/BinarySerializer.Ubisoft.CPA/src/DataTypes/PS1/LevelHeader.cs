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
		public int WayPointsCount { get; set; }
		public int GraphsCount { get; set; }
		public Pointer WayPointsPointer { get; set; }
		public Pointer GraphsPointer { get; set; }
		public short PersosCount { get; set; }
		public short Ushort_0116 { get; set; }
		public Pointer PersosPointer { get; set; }
		public Pointer StatesPointer { get; set; }
		public short StatesCount { get; set; }
		public short Ushort_0122 { get; set; }

		// Serialized from pointers
		public HIE_SuperObject DynamicWorld { get; set; }
		public HIE_SuperObject FatherSector { get; set; }
		public HIE_SuperObject InactiveDynamicWorld { get; set; }
		public ALW_AlwaysList[] Always { get; set; }
		public WAY_WayPoint[] WayPoints { get; set; }
		public WAY_Graph[] Graphs { get; set; }
		public PERSO_Perso[] Persos { get; set; }
		public Pointer<PLA_State>[] States { get; set; }

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

			WayPointsCount = s.Serialize<int>(WayPointsCount, name: nameof(WayPointsCount));
			GraphsCount = s.Serialize<int>(GraphsCount, name: nameof(GraphsCount));
			WayPointsPointer = s.SerializePointer(WayPointsPointer, name: nameof(WayPointsPointer));
			GraphsPointer = s.SerializePointer(GraphsPointer, name: nameof(GraphsPointer));

			PersosCount = s.Serialize<short>(PersosCount, name: nameof(PersosCount));
			Ushort_0116 = s.Serialize<short>(Ushort_0116, name: nameof(Ushort_0116));
			PersosPointer = s.SerializePointer(PersosPointer, name: nameof(PersosPointer));

			StatesPointer = s.SerializePointer(StatesPointer, name: nameof(StatesPointer));
			StatesCount = s.Serialize<short>(StatesCount, name: nameof(StatesCount));
			Ushort_0122 = s.Serialize<short>(Ushort_0122, name: nameof(Ushort_0122));

			// TODO: Serialize remaining data


			// Serialize data from pointers
			s.DoAt(DynamicWorldPointer, () => 
				DynamicWorld = s.SerializeObject<HIE_SuperObject>(DynamicWorld, x => x.Pre_IsDynamic = true, name: nameof(DynamicWorld)));
			s.DoAt(FatherSectorPointer, () =>
				FatherSector = s.SerializeObject<HIE_SuperObject>(FatherSector, x => x.Pre_IsDynamic = false, name: nameof(FatherSector)));
			s.DoAt(InactiveDynamicWorldPointer, () =>
				InactiveDynamicWorld = s.SerializeObject<HIE_SuperObject>(InactiveDynamicWorld, x => x.Pre_IsDynamic = true, name: nameof(InactiveDynamicWorld)));

			s.DoAt(AlwaysPointer, () => 
				Always = s.SerializeObjectArray<ALW_AlwaysList>(Always, AlwaysCount, name: nameof(Always)));

			s.DoAt(WayPointsPointer, () => 
				WayPoints = s.SerializeObjectArray<WAY_WayPoint>(WayPoints, WayPointsCount, name: nameof(WayPoints)));
			s.DoAt(GraphsPointer, () =>
				Graphs = s.SerializeObjectArray<WAY_Graph>(Graphs, GraphsCount, name: nameof(Graphs)));

			s.DoAt(PersosPointer, () =>
				Persos = s.SerializeObjectArray<PERSO_Perso>(Persos, PersosCount, name: nameof(Persos)));
			s.DoAt(StatesPointer, () =>
				States = s.SerializePointerArray<PLA_State>(States, StatesCount, resolve: true, name: nameof(States)));
		}
	}
}