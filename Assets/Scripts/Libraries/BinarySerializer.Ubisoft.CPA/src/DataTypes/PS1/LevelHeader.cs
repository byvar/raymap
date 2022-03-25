using System;
using System.Linq;
using BinarySerializer.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class LevelHeader : BinarySerializable
	{
		public byte[] UnknownBytes1 { get; set; }
		public uint DynamicGeometricObjectsCount_Cine { get; set; }
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
		public uint Ushort_0124 { get; set; }
		public uint Ushort_0128 { get; set; }
		public Pointer Pointer_012C { get; set; } // This + 0x10 = main character
		public Pointer Pointer_0130 { get; set; }
		public uint Uint_134 { get; set; } 
		public uint Uint_138 { get; set; }
		public uint Uint_13C { get; set; }
		public uint Uint_140 { get; set; }
		public uint Uint_144 { get; set; }
		public int InitialCinematicStreamID { get; set; }
		public Pointer AnimationPositionsPointer { get; set; }
		public Pointer AnimationRotationsPointer { get; set; }
		public Pointer AnimationScalesPointer { get; set; }
		
		// UI textures
		public byte UITexturesCount { get; set; }
		public byte Byte_0159 { get; set; }
		public ushort Ushort_015A { get; set; }
		public Pointer UITexturesNamesPointer { get; set; }
		public Pointer UITexturesWidthsPointer { get; set; }
		public Pointer UITexturesHeightsPointer { get; set; }
		public Pointer UITexturesTSBPointer { get; set; }
		public Pointer UITexturesCBAPointer { get; set; }
		public Pointer UITexturesXPointer { get; set; }
		public Pointer UITexturesYPointer { get; set; }

		public Pointer Pointer_0178 { get; set; }
		public int Int_017C { get; set; }
		public Pointer DynamicGeometricObjectsPointer { get; set; }
		public Pointer StaticGeometricObjectsPointer { get; set; }
		public uint DynamicGeometricObjectsCount { get; set; }
		public ushort Ushort_018C { get; set; }
		public ushort Ushort_018E { get; set; }
		public short Short_0190 { get; set; }
		public ushort Ushort_0192 { get; set; }
		public uint IpoCollisionCount { get; set; }
		public Pointer IpoCollisionPointer { get; set; }
		public uint MeshCollisionCount { get; set; }
		public uint MeshCollisionPointer { get; set; }
		public Pointer SectorsPointer { get; set; }
		public ushort SectorsCount { get; set; }
		public ushort Ushort_01AA { get; set; }
		public uint CameraModifiersCount { get; set; }
		public uint Uint_01B0 { get; set; }
		public uint Uint_01B4 { get; set; }
		public uint Uint_01B8 { get; set; }
		public uint VIP_Uint_01BC { get; set; }
		public uint VIP_Uint_01C0 { get; set; }
		public Pointer CameraModifiersVolumesPointer { get; set; }
		public Pointer CameraModifiersPointer { get; set; }
		public Pointer Rush_Pointer_0114 { get; set; }
		public ushort Rush_Ushort_0118 { get; set; }
		public ushort Rush_Ushort_011A { get; set; }
		public Pointer GameMaterialsPointer { get; set; }
		public uint GameMaterialsCount { get; set; }
		public uint Uint_1CC { get; set; }
		public ushort Ushort_1D0 { get; set; }
		public ushort Ushort_1D2 { get; set; }
		public Pointer Pointer_01D4 { get; set; }

		// AGO textures
		public uint AGOTexturesCount { get; set; }
		public Pointer AGOTexturesTSBPointer { get; set; }
		public Pointer AGOTexturesCBAPointer { get; set; }
		public Pointer AGOTexturesXPointer { get; set; }
		public Pointer AGOTexturesYPointer { get; set; }
		public Pointer AGOTexturesAbsoluteXPointer { get; set; }
		public Pointer AGOTexturesAbsoluteYPointer { get; set; }
		public uint Uint_01F4 { get; set; }
		public uint Uint_01F8 { get; set; }
		public uint Uint_01FC { get; set; }
		public Pointer Rush_AGOTexturesWidthsPointer { get; set; }
		public Pointer Rush_AGOTexturesHeightsPointer { get; set; }

		// Serialized from pointers
		public HIE_SuperObject DynamicWorld { get; set; }
		public HIE_SuperObject FatherSector { get; set; }
		public HIE_SuperObject InactiveDynamicWorld { get; set; }
		public ALW_AlwaysList[] Always { get; set; }
		public WAY_WayPoint[] WayPoints { get; set; }
		public WAY_Graph[] Graphs { get; set; }
		public PERSO_Perso[] Persos { get; set; }
		public Pointer<PLA_State>[] States { get; set; }
		public ANIM_Vector[] AnimationPositions { get; set; }
		public ANIM_Quaternion[] AnimationRotations { get; set; }
		public ANIM_Vector[] AnimationScales { get; set; }

		// UI textures
		public Pointer<UITextureName>[] UITexturesNames { get; set; }
		public ushort[] UITexturesWidths { get; set; }
		public ushort[] UITexturesHeights { get; set; }
		public PS1_TSB[] UITexturesTSB { get; set; }
		public PS1_CBA[] UITexturesCBA { get; set; }
		public byte[] UITexturesX { get; set; }
		public byte[] UITexturesY { get; set; }

		public PO_ObjectsTable DynamicGeometricObjects { get; set; }
		public PO_ObjectsTable StaticGeometricObjects { get; set; }
		public COL_GeometricObjectCollide[] IpoCollision { get; set; }
		public SECT_Sector[] Sectors { get; set; }
		public CAM_CameraModifierVolume[] CameraModifierVolumes { get; set; }
		public CAM_CameraModifier[] CameraModifiers { get; set; }
		public GMT_GameMaterial[] GameMaterials { get; set; }

		// AGO textures
		public PS1_TSB[] AGOTexturesTSB { get; set; }
		public PS1_CBA[] AGOTexturesCBA { get; set; }
		public byte[] AGOTexturesX { get; set; }
		public byte[] AGOTexturesY { get; set; }
		public ushort[] AGOTexturesAbsoluteX { get; set; }
		public ushort[] AGOTexturesAbsoluteY { get; set; }
		public ushort[] Rush_AGOTexturesWidths { get; set; }
		public ushort[] Rush_AGOTexturesHeights { get; set; }

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
				DynamicGeometricObjectsCount_Cine = s.Serialize<uint>(DynamicGeometricObjectsCount_Cine, name: nameof(DynamicGeometricObjectsCount_Cine));
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
			Ushort_0124 = s.Serialize<uint>(Ushort_0124, name: nameof(Ushort_0124));
			Ushort_0128 = s.Serialize<uint>(Ushort_0128, name: nameof(Ushort_0128));
			Pointer_012C = s.SerializePointer(Pointer_012C, name: nameof(Pointer_012C));
			Pointer_0130 = s.SerializePointer(Pointer_0130, name: nameof(Pointer_0130));
			Uint_134 = s.Serialize<uint>(Uint_134, name: nameof(Uint_134));
			Uint_138 = s.Serialize<uint>(Uint_138, name: nameof(Uint_138));
			Uint_13C = s.Serialize<uint>(Uint_13C, name: nameof(Uint_13C));
			Uint_140 = s.Serialize<uint>(Uint_140, name: nameof(Uint_140));
			Uint_144 = s.Serialize<uint>(Uint_144, name: nameof(Uint_144));
			InitialCinematicStreamID = s.Serialize<int>(InitialCinematicStreamID, name: nameof(InitialCinematicStreamID));
			AnimationPositionsPointer = s.SerializePointer(AnimationPositionsPointer, name: nameof(AnimationPositionsPointer));
			AnimationRotationsPointer = s.SerializePointer(AnimationRotationsPointer, name: nameof(AnimationRotationsPointer));
			AnimationScalesPointer = s.SerializePointer(AnimationScalesPointer, name: nameof(AnimationScalesPointer));

			if (settings.EngineVersion == EngineVersion.DonaldDuckQuackAttack_PS1)
			{
				// TODO: Implement
				throw new NotImplementedException();
			}
			else if (settings.EngineVersion == EngineVersion.VIP_PS1 || settings.EngineVersion == EngineVersion.JungleBook_PS1)
			{
				// TODO: Implement
				throw new NotImplementedException();
			}
			else
			{
				UITexturesCount = s.Serialize<byte>(UITexturesCount, name: nameof(UITexturesCount));
				Byte_0159 = s.Serialize<byte>(Byte_0159, name: nameof(Byte_0159));
				Ushort_015A = s.Serialize<ushort>(Ushort_015A, name: nameof(Ushort_015A));

				UITexturesNamesPointer = s.SerializePointer(UITexturesNamesPointer, name: nameof(UITexturesNamesPointer));
				UITexturesWidthsPointer = s.SerializePointer(UITexturesWidthsPointer, name: nameof(UITexturesWidthsPointer));
				UITexturesHeightsPointer = s.SerializePointer(UITexturesHeightsPointer, name: nameof(UITexturesHeightsPointer));
				UITexturesTSBPointer = s.SerializePointer(UITexturesTSBPointer, name: nameof(UITexturesTSBPointer));
				UITexturesCBAPointer = s.SerializePointer(UITexturesCBAPointer, name: nameof(UITexturesCBAPointer));
				UITexturesXPointer = s.SerializePointer(UITexturesXPointer, name: nameof(UITexturesXPointer));
				UITexturesYPointer = s.SerializePointer(UITexturesYPointer, name: nameof(UITexturesYPointer));

				Pointer_0178 = s.SerializePointer(Pointer_0178, name: nameof(Pointer_0178));
				Int_017C = s.Serialize<int>(Int_017C, name: nameof(Int_017C));

				DynamicGeometricObjectsPointer = s.SerializePointer(DynamicGeometricObjectsPointer, name: nameof(DynamicGeometricObjectsPointer));
				StaticGeometricObjectsPointer = s.SerializePointer(StaticGeometricObjectsPointer, name: nameof(StaticGeometricObjectsPointer));
				DynamicGeometricObjectsCount = s.Serialize<uint>(DynamicGeometricObjectsCount, name: nameof(DynamicGeometricObjectsCount));
			}

			if (settings.EngineVersion != EngineVersion.VIP_PS1)
			{
				Ushort_018C = s.Serialize<ushort>(Ushort_018C, name: nameof(Ushort_018C));
				Ushort_018E = s.Serialize<ushort>(Ushort_018E, name: nameof(Ushort_018E));
				Short_0190 = s.Serialize<short>(Short_0190, name: nameof(Short_0190));
				Ushort_0192 = s.Serialize<ushort>(Ushort_0192, name: nameof(Ushort_0192));

				IpoCollisionCount = s.Serialize<uint>(IpoCollisionCount, name: nameof(IpoCollisionCount));
				IpoCollisionPointer = s.SerializePointer(IpoCollisionPointer, name: nameof(IpoCollisionPointer));
				MeshCollisionCount = s.Serialize<uint>(MeshCollisionCount, name: nameof(MeshCollisionCount));
				MeshCollisionPointer = s.Serialize<uint>(MeshCollisionPointer, name: nameof(MeshCollisionPointer));
			}
			else
			{
				// TODO: Implement
				throw new NotImplementedException();
			}

			SectorsPointer = s.SerializePointer(SectorsPointer, name: nameof(SectorsPointer));
			SectorsCount = s.Serialize<ushort>(SectorsCount, name: nameof(SectorsCount));
			Ushort_01AA = s.Serialize<ushort>(Ushort_01AA, name: nameof(Ushort_01AA));

			if (settings.EngineVersion != EngineVersion.DonaldDuckQuackAttack_PS1)
			{
				CameraModifiersCount = s.Serialize<uint>(CameraModifiersCount, name: nameof(CameraModifiersCount));
				Uint_01B0 = s.Serialize<uint>(Uint_01B0, name: nameof(Uint_01B0));
				Uint_01B4 = s.Serialize<uint>(Uint_01B4, name: nameof(Uint_01B4));
				Uint_01B8 = s.Serialize<uint>(Uint_01B8, name: nameof(Uint_01B8));

				if (settings.EngineVersion != EngineVersion.VIP_PS1)
				{
					CameraModifiersVolumesPointer = s.SerializePointer(CameraModifiersVolumesPointer, name: nameof(CameraModifiersVolumesPointer));
					CameraModifiersPointer = s.SerializePointer(CameraModifiersPointer, name: nameof(CameraModifiersPointer));
				}
				else
				{
					VIP_Uint_01BC = s.Serialize<uint>(VIP_Uint_01BC, name: nameof(VIP_Uint_01BC));
					VIP_Uint_01C0 = s.Serialize<uint>(VIP_Uint_01C0, name: nameof(VIP_Uint_01C0));
				}

				if (settings.EngineVersion == EngineVersion.RaymanRush_PS1)
				{
					Rush_Pointer_0114 = s.SerializePointer(Rush_Pointer_0114, name: nameof(Rush_Pointer_0114));
					Rush_Ushort_0118 = s.Serialize<ushort>(Rush_Ushort_0118, name: nameof(Rush_Ushort_0118));
					Rush_Ushort_011A = s.Serialize<ushort>(Rush_Ushort_011A, name: nameof(Rush_Ushort_011A));
				}
			}

			GameMaterialsPointer = s.SerializePointer(GameMaterialsPointer, name: nameof(GameMaterialsPointer));
			GameMaterialsCount = s.Serialize<uint>(GameMaterialsCount, name: nameof(GameMaterialsCount));
			Uint_1CC = s.Serialize<uint>(Uint_1CC, name: nameof(Uint_1CC));
			Ushort_1D0 = s.Serialize<ushort>(Ushort_1D0, name: nameof(Ushort_1D0));
			Ushort_1D2 = s.Serialize<ushort>(Ushort_1D2, name: nameof(Ushort_1D2));
			Pointer_01D4 = s.SerializePointer(Pointer_01D4, name: nameof(Pointer_01D4));

			AGOTexturesCount = s.Serialize<uint>(AGOTexturesCount, name: nameof(AGOTexturesCount));
			AGOTexturesTSBPointer = s.SerializePointer(AGOTexturesTSBPointer, name: nameof(AGOTexturesTSBPointer));
			AGOTexturesCBAPointer = s.SerializePointer(AGOTexturesCBAPointer, name: nameof(AGOTexturesCBAPointer));
			AGOTexturesXPointer = s.SerializePointer(AGOTexturesXPointer, name: nameof(AGOTexturesXPointer));
			AGOTexturesYPointer = s.SerializePointer(AGOTexturesYPointer, name: nameof(AGOTexturesYPointer));
			AGOTexturesAbsoluteXPointer = s.SerializePointer(AGOTexturesAbsoluteXPointer, name: nameof(AGOTexturesAbsoluteXPointer));
			AGOTexturesAbsoluteYPointer = s.SerializePointer(AGOTexturesAbsoluteYPointer, name: nameof(AGOTexturesAbsoluteYPointer));

			if (settings.EngineVersion == EngineVersion.RaymanRush_PS1)
			{
				Rush_AGOTexturesWidthsPointer = s.SerializePointer(Rush_AGOTexturesWidthsPointer, name: nameof(Rush_AGOTexturesWidthsPointer));
				Rush_AGOTexturesHeightsPointer = s.SerializePointer(Rush_AGOTexturesHeightsPointer, name: nameof(Rush_AGOTexturesHeightsPointer));
			}
			else if (settings.EngineVersion == EngineVersion.Rayman2_PS1)
			{
				Uint_01F4 = s.Serialize<uint>(Uint_01F4, name: nameof(Uint_01F4));
				Uint_01F8 = s.Serialize<uint>(Uint_01F8, name: nameof(Uint_01F8));
				Uint_01FC = s.Serialize<uint>(Uint_01FC, name: nameof(Uint_01FC));
			}

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

			long animPosCount = (AnimationRotationsPointer.FileOffset - AnimationPositionsPointer.FileOffset) / 6;
			long animRotCount = (AnimationScalesPointer.FileOffset - AnimationRotationsPointer.FileOffset) / 8;
			long animScaleCount = Persos?.
				Where(x => x.Perso3DData?.Family?.Animations != null).
				SelectMany(x => x.Perso3DData.Family.Animations).
				Where(x => x.Channels != null).
				SelectMany(x => x.Channels).
				Where(x => x.Frames != null).
				SelectMany(x => x.Frames).
				Max(x => x.Scale) + 1 ?? 0;

			s.DoAt(AnimationPositionsPointer, () =>
				AnimationPositions = s.SerializeObjectArray<ANIM_Vector>(AnimationPositions, animPosCount, name: nameof(AnimationPositions)));
			s.DoAt(AnimationRotationsPointer, () =>
				AnimationRotations = s.SerializeObjectArray<ANIM_Quaternion>(AnimationRotations, animRotCount, name: nameof(AnimationRotations)));
			s.DoAt(AnimationScalesPointer, () =>
				AnimationScales = s.SerializeObjectArray<ANIM_Vector>(AnimationScales, animScaleCount, name: nameof(AnimationScales)));

			s.DoAt(UITexturesNamesPointer, () =>
				UITexturesNames = s.SerializePointerArray(UITexturesNames, UITexturesCount, resolve: true, name: nameof(UITexturesNames)));
			s.DoAt(UITexturesWidthsPointer, () =>
				UITexturesWidths = s.SerializeArray<ushort>(UITexturesWidths, UITexturesCount, name: nameof(UITexturesWidths)));
			s.DoAt(UITexturesHeightsPointer, () =>
				UITexturesHeights = s.SerializeArray<ushort>(UITexturesHeights, UITexturesCount, name: nameof(UITexturesHeights)));
			s.DoAt(UITexturesTSBPointer, () =>
				UITexturesTSB = s.SerializeObjectArray<PS1_TSB>(UITexturesTSB, UITexturesCount, name: nameof(UITexturesTSB)));
			s.DoAt(UITexturesCBAPointer, () =>
				UITexturesCBA = s.SerializeObjectArray<PS1_CBA>(UITexturesCBA, UITexturesCount, name: nameof(UITexturesCBA)));
			s.DoAt(UITexturesXPointer, () =>
				UITexturesX = s.SerializeArray<byte>(UITexturesX, UITexturesCount, name: nameof(UITexturesX)));
			s.DoAt(UITexturesYPointer, () =>
				UITexturesY = s.SerializeArray<byte>(UITexturesY, UITexturesCount, name: nameof(UITexturesY)));

			uint dynamicGeoCount = DynamicGeometricObjectsCount - 2;
			uint? staticGeoCount = settings.EngineVersion == EngineVersion.Rayman2_PS1 ? IpoCollisionCount : (uint?)null;

			s.DoAt(DynamicGeometricObjectsPointer, () =>
				DynamicGeometricObjects = s.SerializeObject<PO_ObjectsTable>(DynamicGeometricObjects, x => x.Pre_Length = dynamicGeoCount, name: nameof(DynamicGeometricObjects)));
			s.DoAt(StaticGeometricObjectsPointer, () =>
				StaticGeometricObjects = s.SerializeObject<PO_ObjectsTable>(StaticGeometricObjects, x => x.Pre_Length = staticGeoCount, name: nameof(StaticGeometricObjects)));

			s.DoAt(IpoCollisionPointer, () => 
				IpoCollision = s.SerializeObjectArray<COL_GeometricObjectCollide>(IpoCollision, IpoCollisionCount, name: nameof(IpoCollision)));

			s.DoAt(SectorsPointer, () => 
				Sectors = s.SerializeObjectArray<SECT_Sector>(Sectors, SectorsCount, name: nameof(Sectors)));

			s.DoAt(CameraModifiersVolumesPointer, () => 
				CameraModifierVolumes = s.SerializeObjectArray<CAM_CameraModifierVolume>(CameraModifierVolumes, CameraModifiersCount, name: nameof(CameraModifierVolumes)));
			s.DoAt(CameraModifiersPointer, () =>
				CameraModifiers = s.SerializeObjectArray<CAM_CameraModifier>(CameraModifiers, CameraModifiersCount, name: nameof(CameraModifiers)));

			s.DoAt(GameMaterialsPointer, () => 
				GameMaterials = s.SerializeObjectArray<GMT_GameMaterial>(GameMaterials, GameMaterialsCount, name: nameof(GameMaterials)));

			s.DoAt(AGOTexturesTSBPointer, () =>
				AGOTexturesTSB = s.SerializeObjectArray<PS1_TSB>(AGOTexturesTSB, AGOTexturesCount, name: nameof(AGOTexturesTSB)));
			s.DoAt(AGOTexturesCBAPointer, () =>
				AGOTexturesCBA = s.SerializeObjectArray<PS1_CBA>(AGOTexturesCBA, AGOTexturesCount, name: nameof(AGOTexturesCBA)));
			s.DoAt(AGOTexturesXPointer, () =>
				AGOTexturesX = s.SerializeArray<byte>(AGOTexturesX, AGOTexturesCount, name: nameof(AGOTexturesX)));
			s.DoAt(AGOTexturesYPointer, () =>
				AGOTexturesY = s.SerializeArray<byte>(AGOTexturesY, AGOTexturesCount, name: nameof(AGOTexturesY)));
			s.DoAt(AGOTexturesAbsoluteXPointer, () =>
				AGOTexturesAbsoluteX = s.SerializeArray<ushort>(AGOTexturesAbsoluteX, AGOTexturesCount, name: nameof(AGOTexturesAbsoluteX)));
			s.DoAt(AGOTexturesAbsoluteYPointer, () =>
				AGOTexturesAbsoluteY = s.SerializeArray<ushort>(AGOTexturesAbsoluteY, AGOTexturesCount, name: nameof(AGOTexturesAbsoluteY)));
			s.DoAt(Rush_AGOTexturesWidthsPointer, () =>
				Rush_AGOTexturesWidths = s.SerializeArray<ushort>(Rush_AGOTexturesWidths, AGOTexturesCount, name: nameof(Rush_AGOTexturesWidths)));
			s.DoAt(Rush_AGOTexturesHeightsPointer, () =>
				Rush_AGOTexturesHeights = s.SerializeArray<ushort>(Rush_AGOTexturesHeights, AGOTexturesCount, name: nameof(Rush_AGOTexturesHeights)));
		}
	}
}