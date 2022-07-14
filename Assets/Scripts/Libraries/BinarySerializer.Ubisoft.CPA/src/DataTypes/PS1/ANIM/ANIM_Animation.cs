namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ANIM_Animation : BinarySerializable
	{
		public uint Speed { get; set; }
		public Pointer ChannelsPointer { get; set; }
		public uint ChannelsCount { get; set; }
		public ushort FramesCount { get; set; }
		public ushort Ushort_0E { get; set; }
		public uint HierarchiesCount { get; set; }
		public Pointer HierarchiesPointer { get; set; }
		public uint Uint_18 { get; set; }

		public uint FileIndex { get; set; }
		public uint Flags { get; set; }
		public ushort Ushort_18 { get; set; }
		public ushort BonesCount { get; set; }
		public Pointer BonesPointer { get; set; }
		public ushort Ushort_1A { get; set; }
		public uint Uint_20 { get; set; }
		public uint Uint_24 { get; set; }
		public uint Uint_28 { get; set; }

		// Serialized from pointers
		public ANIM_AnimationChannel[] Channels { get; set; }
		public ANIM_Hierarchy[] Hierarchies { get; set; }
		public string Name { get; set; }
		public ANIM_AnimationBoneChannelLinks[] Bones { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			if (settings.EngineVersion == EngineVersion.Rayman2_PS1)
			{
				Speed = s.Serialize<uint>(Speed, name: nameof(Speed));
				ChannelsPointer = s.SerializePointer(ChannelsPointer, name: nameof(ChannelsPointer));
				ChannelsCount = s.Serialize<uint>(ChannelsCount, name: nameof(ChannelsCount));
				FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
				Ushort_0E = s.Serialize<ushort>(Ushort_0E, name: nameof(Ushort_0E));
				HierarchiesCount = s.Serialize<uint>(HierarchiesCount, name: nameof(HierarchiesCount));
				HierarchiesPointer = s.SerializePointer(HierarchiesPointer, name: nameof(HierarchiesPointer));
				Uint_18 = s.Serialize<uint>(Uint_18, name: nameof(Uint_18));
			}
			else if (settings.EngineVersion == EngineVersion.RaymanRush_PS1)
			{
				ChannelsPointer = s.SerializePointer(ChannelsPointer, name: nameof(ChannelsPointer));
				HierarchiesPointer = s.SerializePointer(HierarchiesPointer, name: nameof(HierarchiesPointer));
				HierarchiesCount = s.Serialize<ushort>((ushort)HierarchiesCount, name: nameof(HierarchiesCount));
				FileIndex = s.Serialize<ushort>((ushort)FileIndex, name: nameof(FileIndex));
				Speed = s.Serialize<byte>((byte)Speed, name: nameof(Speed));
				ChannelsCount = s.Serialize<byte>((byte)ChannelsCount, name: nameof(ChannelsCount));
				FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			}
			else
			{
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				ChannelsPointer = s.SerializePointer(ChannelsPointer, name: nameof(ChannelsPointer));
				ChannelsCount = s.Serialize<uint>(ChannelsCount, name: nameof(ChannelsCount));
				FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
				Ushort_0E = s.Serialize<ushort>(Ushort_0E, name: nameof(Ushort_0E));
				HierarchiesCount = s.Serialize<uint>(HierarchiesCount, name: nameof(HierarchiesCount));

				if (settings.EngineVersion == EngineVersion.VIP_PS1 || settings.EngineVersion == EngineVersion.JungleBook_PS1)
				{
					HierarchiesPointer = s.SerializePointer(HierarchiesPointer, name: nameof(HierarchiesPointer));
					Ushort_18 = s.Serialize<ushort>(Ushort_18, name: nameof(Ushort_18));
					BonesCount = s.Serialize<ushort>(BonesCount, name: nameof(BonesCount));
					BonesPointer = s.SerializePointer(BonesPointer, name: nameof(BonesPointer));
					FileIndex = s.Serialize<uint>(FileIndex, name: nameof(FileIndex));
					Speed = 30;
				}
				else
				{
					Speed = s.Serialize<ushort>((ushort)Speed, name: nameof(Speed));
					Ushort_1A = s.Serialize<ushort>(Ushort_1A, name: nameof(Ushort_1A));
					HierarchiesPointer = s.SerializePointer(HierarchiesPointer, name: nameof(HierarchiesPointer));
					Uint_20 = s.Serialize<uint>(Uint_20, name: nameof(Uint_20));
					Uint_24 = s.Serialize<uint>(Uint_24, name: nameof(Uint_24));
					Uint_28 = s.Serialize<uint>(Uint_28, name: nameof(Uint_28));
				}
			}

			// Serialize data from pointers
			s.DoAt(ChannelsPointer, () => 
				Channels = s.SerializeObjectArray<ANIM_AnimationChannel>(Channels, ChannelsCount, name: nameof(Channels)));

			s.DoAt(HierarchiesPointer, () =>
				Hierarchies = s.SerializeObjectArray<ANIM_Hierarchy>(Hierarchies, HierarchiesCount, name: nameof(Hierarchies)));

			if (HierarchiesPointer != null)
				s.DoAt(HierarchiesPointer - (settings.EngineVersion == EngineVersion.DonaldDuckQuackAttack_PS1 ? 0x14 : 0x10), () =>
					Name = s.SerializeString(Name, 0x10, name: nameof(Name)));

			s.DoAt(BonesPointer, () =>
				Bones = s.SerializeObjectArray<ANIM_AnimationBoneChannelLinks>(Bones, BonesCount, name: nameof(Bones)));
		}
	}
}