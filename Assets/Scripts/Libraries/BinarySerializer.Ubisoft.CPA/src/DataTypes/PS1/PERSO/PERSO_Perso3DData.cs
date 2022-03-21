using System;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PERSO_Perso3DData : BinarySerializable
	{
		// TODO: Serialize as booleans with DoBits?
		public uint Flags { get; set; }
		public DataFlags_RR Flags_RR { get; set; }
		public DataFlags_JB Flags_JB { get; set; }

		public Pointer FamilyPointer { get; set; }
		public byte[] Bytes_08 { get; set; }
		public Pointer Pointer_58 { get; set; } // 3 structs of size 0x8 (no pointers)
		public ushort Ushort_5C { get; set; }
		public short Short_5E { get; set; }
		public Pointer AnimationBufferPointer { get; set; }
		public Pointer Pointer_64 { get; set; } // Short_5E * 0x4
		public Pointer Pointer_68 { get; set; } // Short_5E * 0x2
		public Pointer Pointer_6C { get; set; }
		public int CollisionObjectsCount { get; set; }
		public Pointer CollisionObjectsPointer { get; set; }
		public uint StateIndex { get; set; }
		public Pointer Runtime_CurrentStatePointer { get; set; }
		public byte[] Bytes_80 { get; set; } // At 0x92: scale shorts

		// Serialized from pointers
		public PERSO_Family Family { get; set; }
		// TODO: Serialize collision

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			if (settings.EngineVersion == EngineVersion.RaymanRush_PS1)
				Flags_RR = s.Serialize<DataFlags_RR>(Flags_RR, name: nameof(Flags_RR));
			else if (settings.EngineVersion == EngineVersion.JungleBook_PS1)
				Flags_JB = s.Serialize<DataFlags_JB>(Flags_JB, name: nameof(Flags_JB));
			else
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));

			FamilyPointer = s.SerializePointer(FamilyPointer, name: nameof(FamilyPointer));
			Bytes_08 = s.SerializeArray<byte>(Bytes_08, 0x50, name: nameof(Bytes_08));
			Pointer_58 = s.SerializePointer(Pointer_58, name: nameof(Pointer_58));
			Ushort_5C = s.Serialize<ushort>(Ushort_5C, name: nameof(Ushort_5C));
			
			Short_5E = s.Serialize<short>(Short_5E, name: nameof(Short_5E));
			AnimationBufferPointer = s.SerializePointer(AnimationBufferPointer, name: nameof(AnimationBufferPointer));
			Pointer_64 = s.SerializePointer(Pointer_64, name: nameof(Pointer_64));
			Pointer_68 = s.SerializePointer(Pointer_68, name: nameof(Pointer_68));

			Pointer_6C = s.SerializePointer(Pointer_6C, name: nameof(Pointer_6C));
			CollisionObjectsCount = s.Serialize<int>(CollisionObjectsCount, name: nameof(CollisionObjectsCount));
			CollisionObjectsPointer = s.SerializePointer(CollisionObjectsPointer, allowInvalid: CollisionObjectsCount == 0, name: nameof(CollisionObjectsPointer));
			
			StateIndex = s.Serialize<uint>(StateIndex, name: nameof(StateIndex));
			Runtime_CurrentStatePointer = s.SerializePointer(Runtime_CurrentStatePointer, name: nameof(Runtime_CurrentStatePointer));

			Bytes_80 = s.SerializeArray<byte>(Bytes_80, 0x24, name: nameof(Bytes_80));

			// Serialize data from pointers
			s.DoAt(FamilyPointer, () => Family = s.SerializeObject<PERSO_Family>(Family, name: nameof(Family)));
		}

		[Flags]
		public enum DataFlags_RR : uint
		{
			None = 0,
			Actor1 = 1 << 16,
			Actor2 = 1 << 17,
		}

		[Flags]
		public enum DataFlags_JB : uint
		{
			None = 0,
			Actor1 = 1 << 15,
			Actor2 = 1 << 16,
		}
	}
}