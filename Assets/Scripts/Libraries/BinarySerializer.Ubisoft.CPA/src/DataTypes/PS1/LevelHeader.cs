using System;
using System.Linq;
using BinarySerializer.PS1;

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

		public Pointer<UITextureName>[] UITexturesNames { get; set; }
		public ushort[] UITexturesWidths { get; set; }
		public ushort[] UITexturesHeights { get; set; }
		public PS1_TSB[] UITexturesTSB { get; set; }
		public PS1_CBA[] UITexturesCBA { get; set; }
		public byte[] UITexturesX { get; set; }
		public byte[] UITexturesY { get; set; }

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

				// TODO: Serialize remaining data

			}

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
		}
	}
}