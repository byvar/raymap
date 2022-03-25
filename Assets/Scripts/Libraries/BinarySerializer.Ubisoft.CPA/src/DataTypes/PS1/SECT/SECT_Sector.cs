namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class SECT_Sector : BinarySerializable
	{
		public Pointer PersosPointer { get; set; }
		public Pointer GraphicSectorsPointer { get; set; }
		public Pointer CollisionSectorsPointer { get; set; }
		public Pointer ActivitySectorsPointer { get; set; }
		public Pointer Pointer_10 { get; set; } // Sound sectors?
		public Pointer IPOsPointer { get; set; }
		public Pointer Pointer_18 { get; set; }
		public int Int_18 { get; set; }
		public int MinX { get; set; }
		public int MinY { get; set; }
		public int MinZ { get; set; }
		public int Int_28 { get; set; }
		public int MaxX { get; set; }
		public int MaxY { get; set; }
		public int MaxZ { get; set; }
		public int Int_38 { get; set; }
		public int Int_3C { get; set; }
		public short Short_40 { get; set; }
		public short Short_42 { get; set; }
		public short Short_44 { get; set; }
		public short Short_46 { get; set; }
		public short Short_48 { get; set; }
		public short Short_4A { get; set; }
		public int Int_4C { get; set; }
		public int Int_50 { get; set; }

		public ushort Vip_Ushort_08 { get; set; }
		public ushort Vip_Ushort_0A { get; set; }
		public uint Vip_Uint_0C { get; set; }
		public Pointer SoPointer { get; set; }

		// Serialized from pointers
		//public SectorArray<PERSO_Perso> Persos { get; set; }
		public SectorArray<SECT_NeighborSector> GraphicSectors { get; set; }
		public SectorArray<SECT_NeighborSector> CollisionSectors { get; set; }
		public SectorArray<SECT_NeighborSector> ActivitySectors { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			PersosPointer = s.SerializePointer(PersosPointer, name: nameof(PersosPointer));
			GraphicSectorsPointer = s.SerializePointer(GraphicSectorsPointer, name: nameof(GraphicSectorsPointer));

			if (settings.EngineVersion != EngineVersion.Rayman2_PS1 && settings.EngineVersion != EngineVersion.RaymanRush_PS1)
			{
				Vip_Ushort_08 = s.Serialize<ushort>(Vip_Ushort_08, name: nameof(Vip_Ushort_08));
				Vip_Ushort_0A = s.Serialize<ushort>(Vip_Ushort_0A, name: nameof(Vip_Ushort_0A));
				Vip_Uint_0C = s.Serialize<uint>(Vip_Uint_0C, name: nameof(Vip_Uint_0C));
			}

			CollisionSectorsPointer = s.SerializePointer(CollisionSectorsPointer, name: nameof(CollisionSectorsPointer));
			ActivitySectorsPointer = s.SerializePointer(ActivitySectorsPointer, name: nameof(ActivitySectorsPointer));
			Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));
			IPOsPointer = s.SerializePointer(IPOsPointer, name: nameof(IPOsPointer));

			if (settings.EngineVersion != EngineVersion.Rayman2_PS1 && settings.EngineVersion != EngineVersion.RaymanRush_PS1)
				Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
			else
				Pointer_18 = s.SerializePointer(Pointer_18, name: nameof(Pointer_18));

			MinX = s.Serialize<int>(MinX, name: nameof(MinX));
			MinY = s.Serialize<int>(MinY, name: nameof(MinY));
			MinZ = s.Serialize<int>(MinZ, name: nameof(MinZ));

			Int_28 = s.Serialize<int>(Int_28, name: nameof(Int_28));

			MaxX = s.Serialize<int>(MaxX, name: nameof(MaxX));
			MaxY = s.Serialize<int>(MaxY, name: nameof(MaxY));
			MaxZ = s.Serialize<int>(MaxZ, name: nameof(MaxZ));

			Int_38 = s.Serialize<int>(Int_38, name: nameof(Int_38));
			Int_3C = s.Serialize<int>(Int_3C, name: nameof(Int_3C));
			Short_40 = s.Serialize<short>(Short_40, name: nameof(Short_40));
			Short_42 = s.Serialize<short>(Short_42, name: nameof(Short_42));
			Short_44 = s.Serialize<short>(Short_44, name: nameof(Short_44));
			Short_46 = s.Serialize<short>(Short_46, name: nameof(Short_46));

			if (settings.EngineVersion != EngineVersion.JungleBook_PS1 && settings.EngineVersion != EngineVersion.VIP_PS1)
			{
				Short_48 = s.Serialize<short>(Short_48, name: nameof(Short_48));
				Short_4A = s.Serialize<short>(Short_4A, name: nameof(Short_4A));
				Int_4C = s.Serialize<int>(Int_4C, name: nameof(Int_4C));

				if (settings.EngineVersion == EngineVersion.Rayman2_PS1 || settings.EngineVersion == EngineVersion.RaymanRush_PS1)
					Int_50 = s.Serialize<int>(Int_50, name: nameof(Int_50));
			}
			else
			{
				Int_4C = s.Serialize<int>(Int_4C, name: nameof(Int_4C));
				Int_50 = s.Serialize<int>(Int_50, name: nameof(Int_50));
				SoPointer = s.SerializePointer(SoPointer, name: nameof(SoPointer));
			}

			// Serialize data from pointers
			//s.DoAt(PersosPointer, () => 
			//	Persos = s.SerializeObject<SectorArray<PERSO_Perso>>(Persos, name: nameof(Persos)));
			s.DoAt(GraphicSectorsPointer, () => 
				GraphicSectors = s.SerializeObject<SectorArray<SECT_NeighborSector>>(GraphicSectors, name: nameof(GraphicSectors)));
			s.DoAt(CollisionSectorsPointer, () =>
				CollisionSectors = s.SerializeObject<SectorArray<SECT_NeighborSector>>(CollisionSectors, name: nameof(CollisionSectors)));
			s.DoAt(ActivitySectorsPointer, () =>
				ActivitySectors = s.SerializeObject<SectorArray<SECT_NeighborSector>>(ActivitySectors, name: nameof(ActivitySectors)));
		}

		public class SectorArray<T> : BinarySerializable
			where T : BinarySerializable, new()
		{
			public uint Count { get; set; }
			public Pointer ItemsPointer { get; set; }

			// Serialized from pointers
			public T[] Items { get; set; }

			public override void SerializeImpl(SerializerObject s)
			{
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				ItemsPointer = s.SerializePointer(ItemsPointer, allowInvalid: Count == 0, name: nameof(ItemsPointer));

				// Serialize data from pointers
				s.DoAt(ItemsPointer, () => Items = s.SerializeObjectArray<T>(Items, Count, name: nameof(Items)));
			}
		}
	}
}